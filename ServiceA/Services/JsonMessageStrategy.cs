using ServiceA.Interfaces;
using System.Text.Json;

namespace ServiceA.Services
{
    public class JsonMessageStrategy : IMessageStrategy
    {
        public string FormatMessage<T>(string action, T data)
        {
            return JsonSerializer.Serialize(new
            {
                Action = action,
                Data = data,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
