namespace ServiceA.Interfaces
{
    public interface IWebSocketClient : IDisposable
    {
        Task SendMessageAsync(string message);
        Task<string> SendMessageAndWaitForResponseAsync(string message);
        Task ConnectAsync();
    }
}
