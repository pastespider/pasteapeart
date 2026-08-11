using System.ComponentModel.DataAnnotations;

namespace PasteApeart.auditmodels
{
    /// <summary>
    /// 数据库字段变更记录
    /// </summary>
    public class ApPropertyChangeInfo : Entity<long>
    {
        /// <summary>
        /// 日志ID 用于关联查询
        /// </summary>
        [MaxLength(32)]
        public string LogId { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 原始值
        /// </summary>
        public string OriginalValue { get; set; }

        /// <summary>
        /// 当前值
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// 值类型
        /// </summary>
        public string ValueType { get; set; }

    }
}
