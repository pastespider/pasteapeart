using ExampleForm.Application.Contracts;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace ExampleForm.testmodels
{

    ///<summary>
    ///测试表
    ///</summary>
    [Description("测试表")]
    public class MyTableAddDto
    {

        ///<summary>
        ///姓名
        ///</summary>
        [MaxLength(16)]
        [Required]
        [Description("姓名")]
        public string Name { get; set; }

        ///<summary>
        ///年龄
        ///</summary>
        [Description("年龄")]
        public int Age { get; set; }

        ///<summary>
        ///占比
        ///</summary>
        [Description("占比")]
        public double Rate { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        [Description("状态")]
        public bool IsEnable { get; set; } = true;
    }

    ///<summary>
    ///测试表
    ///</summary>
    [Description("测试表")]
    public class MyTableUpdateDto : MyTableAddDto
    {

        ///<summary>
        ///ID
        ///</summary>
        [PasteHidden]
        [Description("ID")]
        public int Id { get; set; }

    }

    ///<summary>
    ///测试表
    ///</summary>
    [Description("测试表")]
    public class MyTableDto : EntityDto<int>
    {
        ///<summary>
        ///姓名
        ///</summary>
        [MaxLength(16)]
        [Description("姓名")]
        public string Name { get; set; }

        ///<summary>
        ///创建日期
        ///</summary>
        [Description("创建日期")]
        public DateTime CreateDate { get; set; }

        ///<summary>
        ///年龄
        ///</summary>
        [Description("年龄")]
        public int Age { get; set; }

        ///<summary>
        ///占比
        ///</summary>
        [Description("占比")]
        public double Rate { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        [Description("状态")]
        public bool IsEnable { get; set; }

    }

    ///<summary>
    ///测试表
    ///</summary>
    [Description("测试表")]
    public class MyTableListDto : EntityDto<int>
    {
        ///<summary>
        ///姓名
        ///</summary>
        [MaxLength(16)]
        [PasteSearch]//允许关键字查询，这个要标记在哪里呢...
        [Description("姓名")]
        public string Name { get; set; }

        ///<summary>
        ///创建日期
        ///</summary>
        [Description("创建日期")]
        public DateTime CreateDate { get; set; }

        ///<summary>
        ///年龄
        ///</summary>
        [Description("年龄")]
        [PasteSum]
        public int Age { get; set; }

        ///<summary>
        ///扩展年龄
        ///</summary>
        [Description("扩展年龄")]
        [PasteHtml("<div>扩展:{{:=item.extendAge}}-{{:=item.age}}</div>")]
        public int ExtendAge { get { return Age; } }

        /// <summary>
        /// 类型
        /// </summary>
        [Description("类型")]
        [PasteHtml("<span class=\"rclass rtype{{:=item.role_val}}\">{{:=item.roleType}}</span>")]
        public EnumRoleType RoleType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [PasteHidden]
        public int role_val { get { return (int)RoleType; } }

        /// <summary>
        /// 扩展状态 所以要申明一个新的字段，转化成比如int，然后调用，否则会被UI默认转化成友好名称
        /// </summary>
        [PasteHtml("<div>{{:=item.id}}-{{:=item.extendStatus}}</div>")]
        [Description("扩展状态")]
        public EnumIsEnableState ExtendStatus { get { return Age % 2 == 0 ? EnumIsEnableState.True : EnumIsEnableState.False; } }

        ///<summary>
        ///占比
        ///</summary>
        [PasteFenToYuan]
        [Description("占比")]
        [PasteSum]
        public double Rate { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        [Description("状态")]
        public bool IsEnable { get; set; }

    }




    ///<summary>
    /// 查询
    ///</summary>
    [Description("查询")]
    public class InputQueryMyTable : InputSearchBase
    {

        /// <summary>
        /// 状态 测试状态
        /// </summary>
        [PasteField("IsEnable")]
        [Description("状态 测试状态")]
        [PasteLselect("", "", 1)]
        public EnumIsEnableState status_val { get; set; } = EnumIsEnableState.All;

        /// <summary>
        /// 年龄
        /// </summary>
        [PasteField("Age")]
        [Description("年龄")]
        public EnumAgeValue age_val { get; set; } = EnumAgeValue.All;

        /// <summary>
        /// 名称查询
        /// </summary>
        [Description("名称查询")]
        public string Name { get; set; }

        /// <summary>
        /// 小于 小于输入值
        /// </summary>
        [PasteField("Age", "<=")]
        [Description("小于")]
        public int less_range { get; set; } = 0;

        /// <summary>
        /// 大于 大于输入值
        /// </summary>
        [PasteField("Age", ">=")]
        [Description("大于")]
        public int max_range { get; set; } = 0;

        /// <summary>
        /// 区间开始
        /// </summary>
        [PasteSearchRange]
        [PasteField("Age")]
        [Description("区间开始")]
        public int minLimit { get; set; }

        /// <summary>
        /// 区间截止
        /// </summary>
        [Description("区间截止")]
        [PasteHidden]
        public int maxLimit { get; set; }

        /// <summary>
        /// 可选多个
        /// </summary>
        [PasteLselect("", "0")]
        [PasteField("Age", "=")]
        [Description("多个")]
        public EnumAgeValue[] ages { get; set; }

        /// <summary>
        /// 年龄 多个年龄查询,多个之间使用,隔开
        /// </summary>
        [PasteSplit(",")]
        [PasteField("Age", "=")]
        [Description("输入年龄")]
        public int[] age_array { get; set; }
    }

}