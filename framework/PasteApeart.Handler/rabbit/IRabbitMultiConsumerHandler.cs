namespace PasteApeart.Handler.rabbit
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRabbitMultiConsumerHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ValueTask DisposeAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task InitAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task ShutdownAsync();
    }
}