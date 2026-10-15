using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExampleApeart.usermodels
{

    ///<summary>
    ///角色信息 一个账号可以绑定多个角色,角色绑定权限
    ///</summary>
    [Description("角色信息 一个账号可以绑定多个角色,角色绑定权限")]
    public class GradeInfoAddDto
    {
        ///<summary>
        ///代码
        ///</summary>
        [MaxLength(8)]
        [Required]
        [PasteOrderby]
        [Description("代码 最大8个字符，用于系统内标识，标识权限模块等")]
        public string Code { get; set; }

        ///<summary>
        ///名称 只要是系统内不重复即可，必填
        ///</summary>
        [MaxLength(16)]
        [Required]
        [PasteOrderby]
        [Description("名称 只要是系统内不重复即可，必填")]
        public string Name { get; set; }

        ///<summary>
        ///描述 对这个角色的补充说明 64
        ///</summary>
        [MaxLength(64)]
        [PasteOrderby]
        [Description("描述 对这个角色的补充说明 64")]
        public string Desc { get; set; }

        ///<summary>
        ///备注 这个角色的内部备注，desc一般表示外部备注 64
        ///</summary>
        [MaxLength(64)]
        [PasteOrderby]
        [Description("备注 这个角色的内部备注，desc一般表示外部备注 64")]
        public string Mark { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        [Description("状态")]
        [PasteOrderby]
        public bool IsEnable { get; set; } = true;

        ///// <summary>
        ///// int 模拟输入多个int,使用,分割
        ///// </summary>
        //[PasteSplit(",")]
        //public int[] ids { get; set; }

        ///// <summary>
        ///// DateTime 模拟输入多个DateTime,使用;分割
        ///// </summary>
        //[PasteSplit(";")]
        //public DateTime[] dates { get; set; }

        ///// <summary>
        ///// 测试 测试全部/无的枚举功能
        ///// </summary>
        //public EnumRoleType roletype { get; set; } = EnumRoleType.Menu;

        ///// <summary>
        ///// DataList
        ///// </summary>
        //[PasteDatalist("","0")]
        //public EnumRoleType datalist { get; set; }

    }

    ///<summary>
    ///角色信息 一个账号可以绑定多个角色,角色绑定权限
    ///</summary>
    [Description("角色信息 一个账号可以绑定多个角色,角色绑定权限")]
    public class GradeInfoUpdateDto : GradeInfoAddDto
    {

        ///<summary>
        ///ID
        ///</summary>
        [PasteHidden]
        [Description("ID")]
        public int Id { get; set; }

    }

    ///<summary>
    ///角色信息 一个账号可以绑定多个角色,角色绑定权限
    ///</summary>
    [Description("角色信息 一个账号可以绑定多个角色,角色绑定权限")]
    public class GradeInfoDto : GradeInfoUpdateDto
    {

    }

    ///<summary>
    ///角色信息 一个账号可以绑定多个角色,角色绑定权限
    ///</summary>
    [Description("角色信息 一个账号可以绑定多个角色,角色绑定权限")]
    public class GradeInfoListDto : GradeInfoUpdateDto
    {
        ///<summary>
        ///状态
        ///</summary>
        [PasteSwitch("view")]
        [PasteSort(10)]
        [Description("状态")]
        public new bool IsEnable { get; set; }

        /// <summary>
        /// 绑定
        /// </summary>
        [PasteSort(12)]
        [PasteHidden("bind")]
        [PasteSwitch("bind")]
        [PasteExtendBind("UserGrade", "GradeId", "UserId", "user_id")]
        public bool ExtendBind { get; set; }

        /// <summary>
        /// 普通Menu模式
        /// </summary>
        [PasteMenu("绑定权限", "open_window('绑定权限','pasteform/index.html?path=roleInfo&model=bind&grade_id={{:=item.id}}','80%','80%')")]
        public int Menu1 { get; }

        /// <summary>
        /// 普通Html模式
        /// </summary>
        [PasteMenuhtml]
        public string Menu11 => $"<a href=\"javascript:;\" onclick=\"open_window('绑定权限','pasteform/index.html?path=roleInfo&model=bind&grade_id={Id}','80%','80%')\">绑定权限</a>";

        /// <summary>
        /// 权限模式
        /// </summary>
        [PasteHidden]
        [PasteMenu("绑定权限", "open_window('绑定权限','pasteform/index.html?path=roleInfo&model=bind&grade_id={{:=item.id}}','80%','80%')", btnCode: "bind")]
        public int Menu3 { get; }

        /// <summary>
        /// 条件模式
        /// </summary>
        [PasteIfMenu("item.id == 2", "绑定权限", "open_window('绑定权限','pasteform/index.html?path=roleInfo&model=bind&grade_id={{:=item.id}}','80%','80%')")]
        public int Menu31 { get; }

        /// <summary>
        /// 条件权限模式 附带按钮权限
        /// </summary>
        [PasteHidden]
        [PasteIfMenu("item.id == 2", "绑定权限", "open_window('绑定权限','pasteform/index.html?path=roleInfo&model=bind&grade_id={{:=item.id}}','80%','80%')", btnCode: "bind")]
        public int Menu32 { get; }

        /// <summary>
        /// 直接渲染UI 
        /// </summary>
        [PasteMenuhtml]
        public string Menu2 => IsEnable ? $"<a href=\"javascript:;\" onclick=\"open_left('{Name}的详细','./pasteform/detail.html?path=gradeInfo&id={Id}','','')\">详细</a>" : $"";
    }




    ///<summary>
    ///角色信息 一个账号可以绑定多个角色,角色绑定权限
    ///</summary>
    [Description("角色信息 一个账号可以绑定多个角色,角色绑定权限")]
    public class GradeShort
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
    public class InputQueryGradeInfo : InputSearchBase
    {
        ///<summary>
        ///名称 
        ///</summary>
        [MaxLength(16)]
        [PasteField("Name", "()")]
        [Description("名称 基于名称查询")]
        public string Name { get; set; }

        /////<summary>
        /////代码
        /////</summary>
        //[MaxLength(8)]
        //[Description("代码")]
        //public string Code { get; set; }

        /// <summary>
        /// 角色 用于为用户选择角色?
        /// </summary>
        [PasteHidden]
        [PasteQuery("user_id", true)]
        [PasteExtendBind("UserGrade", "GradeId", "UserId", "user_id")]
        public int user_id { get; set; }
    }

}