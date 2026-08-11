namespace PasteApeart.Application.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "Apeart";

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; } = "26.06.01";

        /// <summary>
        /// 
        /// </summary>
        public string RootAccount { get; set; } = "admin@admin.com";

        /// <summary>
        /// 
        /// </summary>
        public string RootPassword { get; set; } = "123456";

        /// <summary>
        /// 是否使用RabbitMQ 否则使用本地的Channel
        /// </summary>
        public bool RabbitQueue { get; set; } = false;
    }
}
