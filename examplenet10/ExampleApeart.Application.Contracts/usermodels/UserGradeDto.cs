using PasteApeart;
using PasteApeart.Applciation.Contracts.EntityDto;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;

namespace ExampleApeart.usermodels
{

    ///<summary>
    ///用户绑定角色 解绑的直接删除数据
    ///</summary>
    [Description("用户绑定角色 解绑的直接删除数据")]
    public class UserGradeAddDto
    {
        ///<summary>
        ///用户ID
        ///</summary>
        [PasteOuter("userInfo", "extendUser", "id", "userName")]
        [Description("用户ID")]
        public int UserId { get; set; }

        ///<summary>
        ///角色ID
        ///</summary>
        [PasteOuter("gradeInfo", "extendGrade", "id", "name")]
        [Description("角色ID")]
        public int GradeId { get; set; }

    }

    ///<summary>
    ///用户绑定角色 解绑的直接删除数据
    ///</summary>
    [Description("用户绑定角色 解绑的直接删除数据")]
    public class UserGradeUpdateDto : EntityDto<int>
    {
        ///<summary>
        ///用户ID
        ///</summary>
        [PasteOuter("userInfo", "extendUser", "id", "userName")]
        [Description("用户ID")]
        public int UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [PasteHidden]
        [Description("用户")]
        public UserShortModel ExtendUser { get; set; }

        ///<summary>
        ///角色ID
        ///</summary>
        [PasteOuter("gradeInfo", "extendGrade", "id", "name")]
        [Description("角色ID")]
        public int GradeId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [PasteHidden]
        [Description("角色")]
        public GradeShort ExtendGrade { get; set; }

    }

    ///<summary>
    ///用户绑定角色 解绑的直接删除数据
    ///</summary>
    [Description("用户绑定角色 解绑的直接删除数据")]
    public class UserGradeDto : EntityDto<int>
    {
        ///<summary>
        ///用户ID
        ///</summary>
        [Description("用户ID")]
        public int UserId { get; set; }

        ///<summary>
        ///角色ID
        ///</summary>
        [Description("角色ID")]
        public int GradeId { get; set; }

    }

    ///<summary>
    ///用户绑定角色 解绑的直接删除数据
    ///</summary>
    [PasteEnable(0, 0, 1, 1)]
    [Description("用户绑定角色 解绑的直接删除数据")]
    public class UserGradeListDto : EntityDto<int>
    {
        ///<summary>
        ///用户ID
        ///</summary>
        [PasteShort("UserInfo", "ExtendUser", "ToUserShort()")]
        [Description("用户ID")]
        public int UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [PasteDisplay]
        [Description("用户")]
        public UserShort ExtendUser { get; set; }

        ///<summary>
        ///角色ID
        ///</summary>
        [PasteShort("GradeInfo", "ExtendGrade", "ToGradeShort()")]
        [Description("角色ID")]
        public int GradeId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [PasteDisplay]
        [Description("角色")]
        public GradeShort ExtendGrade { get; set; }
    }

    ///<summary>
    /// 查询
    ///</summary>
    [Description("查询")]
    public class InputQueryUserGrade : InputSearchBase
    {

    }
}