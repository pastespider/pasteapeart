namespace PasteApeart
{
    /// <summary>
    /// 审计日志得配置
    /// </summary>
    public class AuditingConfigOptions
    {
        /// <summary>
        /// 总开关 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// GET请求是否记录
        /// </summary>
        public bool EnableHttpGet { get; set; } = true;

        ///// <summary>
        ///// 忽略Url列表
        ///// </summary>
        //public string[] IgnoreUrls { get; set; }

        /// <summary>
        /// 是否记录Entity的变更
        /// </summary>
        public bool EnableEntityChange { get; set; } = true;

        /// <summary>
        /// 发生异常后 是否记录 无论HTTPMETHOD
        /// </summary>
        public bool EnableException { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool LogRequestBody { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool LogResponseBody { get; set; } = true;

        /// <summary>
        /// 数据存储在HttpContext的Key名称
        /// </summary>
        public string AuditItemKey { get; set; } = "__AuditData";


        /// <summary>
        /// 
        /// </summary>
        public List<string> IgnoredPaths { get; set; } = new()
            {
                "/health",
                "/swagger",
                "/metrics"
            };

        /// <summary>
        /// 
        /// </summary>
        public int MaxRequestBodySize { get; set; } = 1024 * 1024; // 1MB

        /// <summary>
        /// 
        /// </summary>
        public int MaxResponseBodySize { get; set; } = 1024 * 1024; // 1MB

        /// <summary>
        /// 
        /// </summary>
        public bool LogFileUploads { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public List<string> ExcludedContentTypes { get; set; } = new List<string>
            {
                "multipart/form-data",
                "application/octet-stream",
                "image/",
                "video/",
                "audio/"
            };
    }
}
