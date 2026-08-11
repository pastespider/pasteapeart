using PasteApeart.Helper;
using PasteApeart;

namespace $ext_safeprojectname$.usermodels
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
