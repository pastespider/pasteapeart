using PasteApeart.Helper;
using Volo.Abp.Domain.Entities;

namespace ExampleForm.testmodels
{
    /// <summary>
    /// 测试链接表
    /// </summary>
    public class TestLinkInclude : Entity<int>
    {
        /// <summary>
        /// 外表
        /// </summary>
        [PasteInclude("MyTable")]
        public MyTable MyTable { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public int GradeId { get; set; }
    }
}
