using PasteApeart.Helper;
using Volo.Abp.Domain.Entities;

namespace ExampleForm.usermodels
{
    /// <summary>
    /// 用户绑定角色 解绑的直接删除数据
    /// </summary>
    public class UserGrade : Entity<int>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [PasteLink("UserInfo")]
        public int UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [PasteLink("GradeInfo")]
        public int GradeId { get; set; }

    }
}
