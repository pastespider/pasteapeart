namespace PasteApeart.Handler.rabbit
{
    public interface IRabbitMQProducer
    {
        void Close();
        void Dispose();
        Task<bool> SendWithRetryAsync(string exchange, string message, string routingkey, int maxRetries = 3);
        Task WriteSubscribeAsync(string message);
    }
}