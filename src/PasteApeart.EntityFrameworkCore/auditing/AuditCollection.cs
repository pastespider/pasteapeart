using PasteApeart.auditmodels;

namespace PasteApeart.EntityFrameworkCore.auditing
{
   
    /// <summary>
    /// 
    /// </summary>
    public class AuditCollection
    {
        /// <summary>
        /// 数据库变动
        /// </summary>
        public List<ApEntityChangeInfo> EntityChanges { get; set; } = new();

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 持续时间
        /// </summary>
        public long ExecutionDuration => EndTime.HasValue ? (long)(EndTime.Value - StartTime).TotalMilliseconds : 0;
    }
}
