using Microsoft.EntityFrameworkCore;
using PasteApeart.Helper;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace ExampleForm.usermodels
{
    /// <summary>
    /// 角色信息 一个账号可以绑定多个角色，角色绑定权限
    /// </summary>
    [PasteAuth("*", "root", "root")]
    [Index(nameof(Name), IsUnique = true)]
    [PasteShortModel("GradeShort", "ToShortGrade", "Id", "Name")]
    public class GradeInfo : Entity<int>
    {
        /// <summary>
        /// 角色名称 只要是系统内不重复即可，必填
        /// </summary>
        [MaxLength(16)]
        [Required]
        [PasteMutex]
        [PasteSearch]
        public string Name { get; set; } = "";

        /// <summary>
        /// 角色代码
        /// </summary>
        [MaxLength(4)]
        [PasteSearch]
        [PasteMutex]
        public string Code { get; set; }

        /// <summary>
        /// 描述 对这个角色的补充说明 64
        /// </summary>
        [MaxLength(64)]
        public string Desc { get; set; } = "";

        /// <summary>
        /// 备注 这个角色的内部备注，desc一般表示外部备注 64
        /// </summary>
        [MaxLength(64)]
        public string Mark { get; set; } = "";

        /// <summary>
        /// 状态
        /// </summary>
        public bool IsEnable { get; set; } = true;

    }
}
