using Microsoft.EntityFrameworkCore;
using PasteApeart.Domain.Entity;
using PasteApeart.Helper;
using System.ComponentModel.DataAnnotations;

namespace ExampleApeart.usermodels
{
    /// <summary>
    /// 角色信息 一个账号可以绑定多个角色，角色绑定权限
    /// </summary>
    [PasteAuth("*", "root", "root")]
    [Index(nameof(Name), IsUnique = true)]
    [PasteShortModel("GradeShort", "ToGradeShort", "Id", "Name")]
    public class GradeInfo : Entity<int>
    {
        /// <summary>
        /// 名称 只要是系统内不重复即可，必填
        /// </summary>
        [MaxLength(16)]
        [Required]
        [PasteMutex]
        [PasteSearch]
        public string Name { get; set; } = "";

        /// <summary>
        /// 代码
        /// </summary>
        [MaxLength(8)]
        [PasteSearch]
        [PasteMutex]
        public string Code { get; set; }

        /// <summary>
        /// 描述 对这个角色的补充说明 64
        /// </summary>
        [MaxLength(64)]
        [PasteSearch]
        public string Desc { get; set; } = "";

        /// <summary>
        /// 备注 这个角色的内部备注，desc一般表示外部备注 64
        /// </summary>
        [MaxLength(64)]
        [PasteSearch]
        public string Mark { get; set; } = "";

        /// <summary>
        /// 状态
        /// </summary>
        public bool IsEnable { get; set; } = true;

    }
}
