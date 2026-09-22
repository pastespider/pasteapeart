using PasteApeart.Helper;
using PasteApeart;
using PasteApeart.Domain.Entity;

namespace ExampleApeart.usermodels
{
    /// <summary>
    /// 用户绑定角色 解绑的直接删除数据
    /// </summary>
    public class UserGrade : Entity<int>, IUserIdEntiry
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
