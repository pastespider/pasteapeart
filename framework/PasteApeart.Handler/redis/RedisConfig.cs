namespace PasteApeart.Handler.redis
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Enable { get; set; } = false;

        /// <summary>
        /// 主链接
        /// </summary>
        public string MainConnection { get; set; } = "127.0.0.1:6379";

        /// <summary>
        /// 前缀 xxx:
        /// </summary>
        public string Prefix { get; set; } = "pt:";

        /// <summary>
        /// 链接名称
        /// </summary>
        public string ClientName { get; set; } = "paste";

    }
}
