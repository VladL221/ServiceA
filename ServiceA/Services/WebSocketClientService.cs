using ServiceA.Interfaces;
using System.Net.WebSockets;
using System.Text;

public class WebSocketClientService : IWebSocketClient
{
    private readonly Dictionary<string, TaskCompletionSource<string>> _pendingResponses = new();
    private readonly ClientWebSocket? _webSocket;
    private readonly string _serverUrl;
    private readonly ILogger<WebSocketClientService> _logger;
    private readonly IMessageStrategy _messageStrategy;
    private bool _disposed;

    public WebSocketClientService(
        IConfiguration configuration,
        ILogger<WebSocketClientService> logger,
        IMessageStrategy messageStrategy)
    {
        _serverUrl = configuration["WebSocket:ServerUrl"] ?? "ws://localhost:5002/ws";
        _webSocket = new ClientWebSocket();
        _logger = logger;
        _messageStrategy = messageStrategy;
    }

    public async Task ConnectAsync()
    {
        if (_webSocket?.State != WebSocketState.Open)
        {
            try
            {
                await _webSocket!.ConnectAsync(new Uri(_serverUrl), CancellationToken.None);
                _ = ReceiveMessagesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to WebSocket server");
                throw;
            }
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (_webSocket?.State != WebSocketState.Open)
        {
            await ConnectAsync();
        }

        var buffer = Encoding.UTF8.GetBytes(message);
        await _webSocket!.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }

    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024 * 4];
        try
        {
            while (_webSocket?.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        string.Empty,
                        CancellationToken.None);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _logger.LogInformation("Received message: {Message}", message);

                    foreach (var pendingResponse in _pendingResponses)
                    {
                        pendingResponse.Value.TrySetResult(message);
                    }
                    _pendingResponses.Clear();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while receiving messages");

            foreach (var pendingResponse in _pendingResponses)
            {
                pendingResponse.Value.TrySetException(ex);
            }
            _pendingResponses.Clear();
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _webSocket?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    public async Task<string> SendMessageAndWaitForResponseAsync(string message)
    {
        try
        {
            var tcs = new TaskCompletionSource<string>();
            var requestId = Guid.NewGuid().ToString();
            _pendingResponses[requestId] = tcs;

            await SendMessageAsync(message);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(-1, cts.Token));

            if (completedTask == tcs.Task)
            {
                return await tcs.Task;
            }

            throw new TimeoutException("Request timed out waiting for response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendMessageAndWaitForResponseAsync");
            throw;
        }
    }
}