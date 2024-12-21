namespace ServiceA.Interfaces
{
    public interface IMessageStrategy
    {
        string FormatMessage<T>(string action, T data);
    }
}
