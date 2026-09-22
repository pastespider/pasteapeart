using ExampleForm.Application.Contracts;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExampleForm.testmodels
{

    /// <summary>
    /// 
    /// </summary>
    public class TestTableBase
    {
        ///<summary>
        ///姓名 模拟短文本输入
        ///</summary>
        [Required]
        [MaxLength(16)]
        [Description("姓名 模拟短文本输入")]
        public string Name { get; set; }
        ///<summary>
        ///头像 模拟图片上传
        ///</summary>
        [MaxLength(128)]
        [PasteImage(1, "head", "120*120")]
        [Description("头像 模拟图片上传")]
        public string Head { get; set; }
        ///<summary>
        ///年龄 模拟输入number
        ///</summary>
        [PasteUnit("周岁")]
        [PasteSort(100)]
        [Description("年龄 模拟输入number")]
        public int Age { get; set; }
    }

    ///<summary>
    ///测试表 用于测试CURD的表
    ///</summary>
    [Description("测试表 用于测试CURD的表")]
    public class TestTableAddDto : TestTableBase
    {
        /////<summary>
        /////姓名 模拟短文本输入
        /////</summary>
        //[Required]
        //[MaxLength(16)]
        //[Description("姓名 模拟短文本输入")]
        //public string Name { get; set; }
        /////<summary>
        /////头像 模拟图片上传
        /////</summary>
        //[MaxLength(128)]
        //[PasteImage(1, "head", "120*120")]
        //[Description("头像 模拟图片上传")]
        //public string Head { get; set; }
        /////<summary>
        /////年龄 模拟输入number
        /////</summary>
        //[PasteUnit("周岁")]
        //[Description("年龄 模拟输入number")]
        //public int Age { get; set; }
        ///<summary>
        ///会员周期 会员生效区间
        ///</summary>
        [PasteDaterange("dateStart", "dateEnd")]
        [Description("会员周期 会员生效区间")]
        public DateTime DateStart { get; set; }
        ///<summary>
        ///会员周期 会员生效区间
        ///</summary>
        [PasteHidden()]
        [Description("会员周期 会员生效区间")]
        public DateTime DateEnd { get; set; }
        ///<summary>
        ///余额 数据库是分为单位，UI中是以元为单位
        ///</summary>
        [PasteFenToYuan()]
        [Description("余额 数据库是分为单位，UI中是以元为单位")]
        public int Balance { get; set; }

    }

    ///<summary>
    ///测试表 用于测试CURD的表
    ///</summary>
    [Description("测试表 用于测试CURD的表")]
    public class TestTableUpdateDto : TestTableAddDto
    {

        ///<summary>
        ///ID
        ///</summary>
        [PasteHidden]
        [Description("ID")]
        public int Id { get; set; }
    }

    ///<summary>
    ///测试表 用于测试CURD的表
    ///</summary>
    [Description("测试表 用于测试CURD的表")]
    public class TestTableDto : TestTableUpdateDto
    {

    }

    ///<summary>
    ///测试表 用于测试CURD的表
    ///</summary>
    [Description("测试表 用于测试CURD的表")]
    public class TestTableListDto : TestTableUpdateDto
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Description("标识")]
        public new int Id { get; set; }

        ///<summary>
        ///余额 数据库是分为单位，UI中是以元为单位
        ///</summary>
        [PasteFenToYuan()]
        [PasteSort(-10)]
        [Description("余额 数据库是分为单位，UI中是以元为单位")]
        public new int Balance { get; set; }
    }

    ///<summary>
    ///测试表 用于测试CURD的表
    ///</summary>
    [Description("测试表 用于测试CURD的表")]
    public class TestShort
    {

        ///<summary>
        ///ID
        ///</summary>
        [Description("ID")]
        public int Id { get; set; }

        ///<summary>
        ///Name
        ///</summary>
        [Description("Name")]
        public string Name { get; set; }
    }

    ///<summary>
    /// 查询
    ///</summary>
    [Description("查询")]
    public class InputQueryTestTable : InputSearchBase
    {

        ///// <summary>
        ///// 状态 测试状态
        ///// </summary>
        //[PasteField("IsEnable")]
        //public EnumStateQuery status_val { get; set; } = EnumStateQuery.all;
    }

}