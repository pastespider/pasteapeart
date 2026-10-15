namespace PasteApeart.Handler.rabbit
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRabbitMQProducer
    {
        /// <summary>
        /// 
        /// </summary>
        void Close();
        /// <summary>
        /// 
        /// </summary>
        void Dispose();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="message"></param>
        /// <param name="routingkey"></param>
        /// <param name="maxRetries"></param>
        /// <returns></returns>
        Task<bool> SendWithRetryAsync(string exchange, string message, string routingkey, int maxRetries = 3);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task WriteSubscribeAsync(string message);
    }
}