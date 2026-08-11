using System.ComponentModel.DataAnnotations;

namespace PasteApeart.auditmodels
{

    /// <summary>
    /// 审计日志
    /// </summary>
    public class ApAuditLog : Entity<long>
    {

        /// <summary>
        /// 日志ID 用于关联查询
        /// </summary>
        [MaxLength(32)]
        public string LogId { get; set; }

        /// <summary>
        /// 当前用户
        /// </summary>
        public string UserId { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public string UserName { get; set; }

        /// <summary>
        /// 客户端地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Method
        /// </summary>
        [MaxLength(16)]
        public string HttpMethod { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public int HttpStatusCode { get; set; }

        /// <summary>
        /// 请求内容
        /// </summary>
        public string RequestBody { get; set; }

        /// <summary>
        /// 地址参数
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// Headers
        /// </summary>
        public string Headers { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string ResponseBody { get; set; }

        /// <summary>
        /// 持续时长 毫秒
        /// </summary>
        public long ExecutionDuration { get; set; } // 毫秒

        /// <summary>
        /// 异常信息
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 是否异常
        /// </summary>
        public bool HasException { get; set; }

        /// <summary>
        /// 是否变更
        /// </summary>
        public bool HasEntityChange { get; set; }

        /// <summary>
        /// 字段变更 
        /// </summary>
        public List<ApEntityChangeInfo> EntityChanges { get; set; }  // JSON格式存储所有实体变更

        ///// <summary>
        ///// 没啥用
        ///// </summary>
        //[Obsolete]
        //public string CustomData { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime ExecutionTime { get; set; } = DateTime.UtcNow;
    }




}
