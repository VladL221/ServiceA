using System.Text.Json;
using ServiceA.Interfaces;
using ServiceA.Models;

namespace ServiceA.Services;

public class GraphService : IGraphService
{
    private readonly IWebSocketClient _webSocketClient;
    private readonly IMessageStrategy _messageStrategy;
    private readonly ILogger<GraphService> _logger;

    public GraphService(
        IWebSocketClient webSocketClient,
        IMessageStrategy messageStrategy,
        ILogger<GraphService> logger)
    {
        _webSocketClient = webSocketClient;
        _messageStrategy = messageStrategy;
        _logger = logger;
    }

    public async Task<Graph> CreateGraphAsync(Graph graph)
    {
        ValidateGraph(graph);

        var newGraph = graph with
        {
            Id = Guid.NewGuid(),
            Node = graph.Node with { Id = Guid.NewGuid() },
            Edges = graph.Edges.Select(e => e with { Id = Guid.NewGuid() }).ToList()
        };

        var message = _messageStrategy.FormatMessage("Create", newGraph);
        await _webSocketClient.SendMessageAsync(message);

        return newGraph;
    }

    public async Task<Graph?> GetGraphAsync(Guid id)
    {
        try
        {
            _logger.LogInformation($"Attempting to get graph with ID: {id}");
            var message = _messageStrategy.FormatMessage("Get", id);

            var response = await _webSocketClient.SendMessageAndWaitForResponseAsync(message);
            _logger.LogInformation($"Received response: {response}");

            if (string.IsNullOrEmpty(response))
            {
                _logger.LogWarning("No response received");
                return null;
            }

            var wsResponse = JsonSerializer.Deserialize<WebSocketResponse>(response);
            if (wsResponse?.Success != true || wsResponse.Data == null)
            {
                _logger.LogWarning("Invalid or unsuccessful response");
                return null;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var graphJson = JsonSerializer.Serialize(wsResponse.Data);
            var graph = JsonSerializer.Deserialize<Graph>(graphJson, options);

            _logger.LogInformation($"Deserialized graph: {JsonSerializer.Serialize(graph)}");
            return graph;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting graph");
            throw;
        }
    }

    public async Task UpdateGraphAsync(Guid id, Graph graph)
    {
        ValidateGraph(graph);
        var message = _messageStrategy.FormatMessage("Update", graph with { Id = id });
        await _webSocketClient.SendMessageAsync(message);
    }

    public async Task DeleteGraphAsync(Guid id)
    {
        var message = _messageStrategy.FormatMessage("Delete", id);
        await _webSocketClient.SendMessageAsync(message);
    }

    private void ValidateGraph(Graph graph)
    {
        if (graph.Edges.Count != 2)
        {
            throw new ArgumentException("Graph must have exactly 2 edges");
        }
    }
}