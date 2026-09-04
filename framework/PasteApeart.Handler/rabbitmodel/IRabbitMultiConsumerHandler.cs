namespace PasteApeart.Handler.rabbit
{
    public interface IRabbitMultiConsumerHandler
    {
        ValueTask DisposeAsync();
        Task InitAsync();
        Task ShutdownAsync();
    }
}