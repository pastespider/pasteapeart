using PasteApeart.Domain;
using PasteApeart.Domain.PasteEntity;
using System.ComponentModel.DataAnnotations;

namespace PasteApeart.auditmodels
{
    /// <summary>
    /// 数据库变更
    /// </summary>
    public class ApEntityChangeInfo : PasteEntity<long>
    {
        /// <summary>
        /// 日志ID 用于关联查询
        /// </summary>
        [MaxLength(32)]
        public string LogId { get; set; }

        /// <summary>
        /// 实体类型名称
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// 实体主键值
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// 4Added, 3Modified, 2Deleted
        /// </summary>
        public int ChangeType { get; set; }

        /// <summary>
        /// 属性变更列表
        /// </summary>
        public List<ApPropertyChangeInfo> PropertyChanges { get; set; }

        /// <summary>
        /// 变更时间
        /// </summary>
        public DateTime ChangeTime { get; set; }
    }

}
