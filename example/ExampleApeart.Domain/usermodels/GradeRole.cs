using Microsoft.EntityFrameworkCore;
using PasteApeart;
using PasteApeart.Domain.Entity;
using PasteApeart.Helper;

namespace ExampleApeart.usermodels
{
    /// <summary>
    /// 角色权限
    /// </summary>
    [Comment("角色权限")]
    public class GradeRole : Entity<int>
    {
        /// <summary>
        /// 角色ID 点击选择角色
        /// </summary>
        [PasteShort("GradeInfo", "ExtendGrade", "ToShortGrade()")]
        public int GradeId { get; set; }

        /// <summary>
        /// 权限ID 点击选择权限
        /// </summary>
        [PasteShort("RoleInfo", "ExtendRole", "ToShortRole()")]
        public int RoleId { get; set; }
    }
}
