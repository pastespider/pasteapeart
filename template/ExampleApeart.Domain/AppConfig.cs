namespace $ext_safeprojectname$
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

        /// <summary>
        /// 缓存模式 默认为memory 可选 redis
        /// </summary>
        public string CacheType { get; set; } = "memory";

        /// <summary>
        /// 数据库模式 默认sqlite 可选看自己实现db比如 pgsql mysql mariadb sqlserver等
        /// </summary>
        public string DataType { get; set; } = "sqlite";

        /// <summary>
        /// 使用读写分离
        /// </summary>
        public bool UseReadWrite { get; set; } = false;
    }
}
