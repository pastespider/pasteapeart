using Microsoft.EntityFrameworkCore;
using PasteApeart.Helper;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace ExampleForm.usermodels
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Comment("登录用户表")]
    [PasteAuth("*", "root", "root")]
    [Index(nameof(Email), IsUnique = true)]
    public class UserInfo : Entity<int>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [MaxLength(32)]
        public string UserName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [MaxLength(128)]
        public string UserHead { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(64)]
        public string Desc { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(64)]
        public string Email { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(64)]
        public string PassWord { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool IsEnable { get; set; } = true;

    }
}
