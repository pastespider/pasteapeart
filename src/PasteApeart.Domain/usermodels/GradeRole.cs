using Microsoft.EntityFrameworkCore;
using PasteForm.Helper;

namespace PasteApeart.usermodels
{
    /// <summary>
    /// 角色绑定权限
    /// </summary>
    [Comment("角色绑定权限")]
    public class GradeRole : Entity<int>
    {
        /// <summary>
        /// 角色ID 点击选择角色
        /// </summary>
        [PasteQuery("role_id", false)]
        [PasteShort("GradeInfo", "ExtendGrade", "ToShortGrade()")]
        public int GradeId { get; set; }

        /// <summary>
        /// 权限ID 点击选择权限
        /// </summary>
        [PasteQuery("role_id", false)]
        [PasteShort("RoleInfo", "ExtendRole", "ToShortRole()")]
        public int RoleId { get; set; }
    }
}
