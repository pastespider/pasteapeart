using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace ExampleForm.testmodels
{
    /// <summary>
    /// 
    /// </summary>
    public class GuidTest1 : Entity<Guid>
    {
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
