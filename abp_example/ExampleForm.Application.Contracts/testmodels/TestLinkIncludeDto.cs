using ExampleForm.Application.Contracts;
using ExampleForm.usermodels;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;
using Volo.Abp.Application.Dtos;

namespace ExampleForm.testmodels
{

    ///<summary>
    ///测试链接表
    ///</summary>
    public class TestLinkIncludeAddDto
    {

        ///<summary>
        ///外表
        ///</summary>
        [PasteInclude("MyTable")]
        [Description("外表")]
        [PasteDirectsun]
        public MyTableAddDto MyTable { get; set; }

        ///<summary>
        ///用户
        ///</summary>
        [Description("用户")]
        [PasteOuter("userInfo", "extendUser", "id", "userName")]
        [PasteShort("UserInfo", "ExtendUser", "ToUserShort()")]
        public int UserId { get; set; }

        ///<summary>
        ///角色
        ///</summary>
        [Description("角色")]
        [PasteOuter("gradeInfo", "extendGrade", "id", "name")]
        [PasteShort("GradeInfo", "ExtendGrade", "ToGradeShort()")]
        public int GradeId { get; set; }
    }

    ///<summary>
    ///测试链接表
    ///</summary>
    public class TestLinkIncludeUpdateDto : EntityDto<int>
    {

        ///<summary>
        ///外表
        ///</summary>
        [PasteInclude("MyTable")]
        [Description("外表")]
        [PasteDirectsun]
        public MyTableUpdateDto MyTable { get; set; }

        ///<summary>
        ///用户
        ///</summary>
        [Description("用户")]
        [PasteOuter("userInfo", "extendUser", "id", "userName")]
        [PasteShort("UserInfo", "ExtendUser", "ToUserShort()")]
        public int UserId { get; set; }

        ///<summary>
        ///角色
        ///</summary>
        [Description("角色")]
        [PasteOuter("gradeInfo", "extendGrade", "id", "name")]
        [PasteShort("GradeInfo", "ExtendGrade", "ToGradeShort()")]
        public int GradeId { get; set; }

        ///<summary>
        ///用户 表示管理者
        ///</summary>
        [PasteDisplay]
        public UserShort ExtendUser { get; set; }

        ///<summary>
        ///角色 表示权限的一个分组
        ///</summary>
        [PasteDisplay]
        public GradeShort ExtendGrade { get; set; }

    }

    ///<summary>
    ///测试链接表
    ///</summary>
    public class TestLinkIncludeDto : EntityDto<int>
    {
        ///<summary>
        ///外表
        ///</summary>
        [PasteInclude("MyTable")]
        [Description("外表")]
        [PasteDirectsun]
        public MyTableDto MyTable { get; set; }

        ///<summary>
        ///用户
        ///</summary>
        [Description("用户")]
        [PasteOuter("userInfo", "extendUser", "id", "userName")]
        [PasteShort("UserInfo", "ExtendUser", "ToUserShort()")]
        public int UserId { get; set; }

        ///<summary>
        ///角色
        ///</summary>
        [Description("角色")]
        [PasteOuter("gradeInfo", "extendGrade", "id", "name")]
        [PasteShort("GradeInfo", "ExtendGrade", "ToGradeShort()")]
        public int GradeId { get; set; }

        ///<summary>
        ///用户 表示管理者
        ///</summary>
        [PasteDisplay]
        public UserShort ExtendUser { get; set; }

        ///<summary>
        ///角色 表示权限的一个分组
        ///</summary>
        [PasteDisplay]
        public GradeShort ExtendGrade { get; set; }

    }

    ///<summary>
    ///测试链接表
    ///</summary>
    public class TestLinkIncludeListDto : EntityDto<int>
    {
        ///<summary>
        ///外表
        ///</summary>
        [PasteInclude("MyTable")]
        [Description("外表")]
        [PasteDirectsun]
        public MyTableListDto MyTable { get; set; }

        ///<summary>
        ///用户
        ///</summary>
        [Description("用户")]
        [PasteShort("UserInfo", "ExtendUser", "ToUserShort()")]
        [PasteHidden]
        public int UserId { get; set; }

        ///<summary>
        ///角色
        ///</summary>
        [Description("角色")]
        [PasteShort("GradeInfo", "ExtendGrade", "ToGradeShort()")]
        [PasteHidden]
        public int GradeId { get; set; }

        ///<summary>
        ///用户 表示管理者
        ///</summary>
        [PasteDisplay]
        public UserShort ExtendUser { get; set; }

        ///<summary>
        ///角色 表示权限的一个分组
        ///</summary>
        [PasteDisplay]
        public GradeShort ExtendGrade { get; set; }

    }

    ///<summary>
    /// 查询
    ///</summary>
    public class InputQueryTestLinkInclude : InputSearchBase
    {

    }

}