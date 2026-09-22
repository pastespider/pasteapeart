using Microsoft.EntityFrameworkCore;
using PasteApeart.Domain.Entity;
using PasteApeart.Helper;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace ExampleForm.testmodels
{
    /// <summary>
    /// 测试表 用于测试CURD的表
    /// </summary>
    [Index(nameof(UserId), IsUnique = true)]
    [PasteShortModel("TestShort", "ToTestShort", "Id", "Name")]
    public class TestTable : Volo.Abp.Domain.Entities.Entity<int>, IUserIdEntiry
    {
        /// <summary>
        /// 姓名 模拟短文本输入
        /// </summary>
        [Required]
        [MaxLength(16)]
        public string Name { get; set; }

        /// <summary>
        /// 头像 模拟图片上传
        /// </summary>
        [MaxLength(128)]
        [PasteImage(1, "head", "120*120")]
        public string Head { get; set; }

        /// <summary>
        /// 年龄 模拟输入number
        /// </summary>
        [PasteUnit("周岁")]
        public int Age { get; set; }

        /// <summary>
        /// 会员周期 会员生效区间
        /// </summary>
        [PasteDaterange("dateStart", "dateEnd")]
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 会员周期 会员生效区间
        /// </summary>
        [PasteHidden]
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// 余额 数据库是分为单位，UI中是以元为单位
        /// </summary>
        [PasteFenToYuan]
        public int Balance { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [PasteShort("UserInfo", "ExtendUser", "ToUserShort()")]
        public int UserId { get; set; }

    }
}
