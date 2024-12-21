namespace ServiceA.Models
{
    public class WebSocketResponse
    {
        public string? CorrelationId { get; set; }
        public object? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
