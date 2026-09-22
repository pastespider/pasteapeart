using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace ExampleForm.testmodels
{
    /// <summary>
    /// 测试UINT
    /// </summary>
    public class UintTest1 : Entity<uint>
    {
        /// <summary>
        /// 标识
        /// </summary>
        public new uint Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(16)]
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [MaxLength(64)]
        public string Desc { get; set; }
    }
}
