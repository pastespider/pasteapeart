using PasteApeart.Helper;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace ExampleForm.testmodels
{
    /// <summary>
    /// 测试表
    /// </summary>
    public class ParentTable : Entity<int>
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(16)]
        [PasteSearch]//允许word进行关键字查询
        public string Name { get; set; } = "用户昵称";

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; } = 12;

        /// <summary>
        /// 包含
        /// </summary>
        public IncludeTable Front { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool IsEnable { get; set; } = true;

    }
}
