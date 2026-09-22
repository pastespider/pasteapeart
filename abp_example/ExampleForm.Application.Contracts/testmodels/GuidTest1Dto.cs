using ExampleForm.Application.Contracts;
using ExampleForm.usermodels;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace ExampleForm.testmodels
{

    ///<summary>
    ///
    ///</summary>
    public class GuidTest1AddDto
    {

        ///<summary>
        ///名称
        ///</summary>
        [MaxLength(16)]
        [Description("名称")]
        public string Name { get; set; }

        ///<summary>
        ///说明
        ///</summary>
        [MaxLength(64)]
        [Description("说明")]
        public string Desc { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [PasteShort("UserInfo", "ExtendUser", "ToUserShort()")]
        [Description("用户")]
        [PasteOuter( "userInfo", "extendUser", "id", "userName")]
        public int UserId { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        [PasteOuter("roleInfo", "extendRole", "id", "name")]
        [PasteShort("RoleInfo", "ExtendRole", "ToRoleShort()")]
        [Description("权限")]
        public int RoleId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [PasteOuter("gradeInfo", "extendGrade", "id", "name")]
        [PasteShort("GradeInfo", "ExtendGrade", "ToGradeShort()")]
        [Description("角色")]
        public int GradeId { get; set; }
    }

    ///<summary>
    ///
    ///</summary>
    public class GuidTest1UpdateDto : GuidTest1AddDto
    {

        ///<summary>
        ///ID
        ///</summary>
        [PasteHidden]
        [Description("ID")]
        public Guid Id { get; set; }

    }

    ///<summary>
    ///
    ///</summary>
    public class GuidTest1Dto : EntityDto<Guid>
    {
        ///<summary>
        ///名称
        ///</summary>
        [MaxLength(16)]
        [Description("名称")]
        public string Name { get; set; }

        ///<summary>
        ///说明
        ///</summary>
        [MaxLength(64)]
        [Description("说明")]
        public string Desc { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        [Description("用户")]
        [PasteOuter( "userInfo", "extendUser", "id", "userName")]
        public int UserId { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        [Description("权限")]
        [PasteOuter("roleInfo", "extendRole", "id", "name")]
        public int RoleId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [Description("角色")]
        [PasteOuter("gradeInfo", "extendGrade", "id", "name")]
        public int GradeId { get; set; }


        ///<summary>
        /// 用户 表示管理者
        ///</summary>
        [PasteDisplay]
        [Description("用户 表示管理者")]
        public UserShort ExtendUser { get; set; }

        ///<summary>
        /// 角色 表示权限的一个分组
        ///</summary>
        [PasteDisplay]
        [Description("角色 表示权限的一个分组")]
        public GradeShort ExtendGrade { get; set; }

        ///<summary>
        /// 权限 表示接口的权限
        ///</summary>
        [PasteDisplay]
        [Description("权限 表示接口的权限")]
        public RoleShort ExtendRole { get; set; }

    }

    ///<summary>
    ///
    ///</summary>
    public class GuidTest1ListDto : EntityDto<Guid>
    {
        ///<summary>
        ///名称
        ///</summary>
        [MaxLength(16)]
        [Description("名称")]
        public string Name { get; set; }

        ///<summary>
        ///说明
        ///</summary>
        [MaxLength(64)]
        [Description("说明")]
        public string Desc { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        [Description("用户")]
        [PasteOuter( "userInfo", "extendUser", "id", "userName")]
        [PasteHidden()]
        public int UserId { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        [Description("权限")]
        [PasteOuter("roleInfo", "extendRole", "id", "name")]
        [PasteHidden()]
        public int RoleId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [Description("角色")]
        [PasteOuter("gradeInfo", "extendGrade", "id", "name")]
        [PasteHidden()]
        public int GradeId { get; set; }

        ///<summary>
        /// 用户 表示管理者
        ///</summary>
        [PasteDisplay]
        [Description("用户 表示管理者")]
        public UserShort ExtendUser { get; set; }

        ///<summary>
        /// 角色 表示权限的一个分组
        ///</summary>
        [PasteDisplay]
        [Description("角色 表示权限的一个分组")]
        public GradeShort ExtendGrade { get; set; }

        ///<summary>
        /// 权限 表示接口的权限
        ///</summary>
        [PasteDisplay]
        [Description("权限 表示接口的权限")]
        public RoleShort ExtendRole { get; set; }

    }




    ///<summary>
    /// 查询
    ///</summary>
    [Description("查询")]
    public class InputQueryGuidTest1 : InputSearchBase
    {

    }

}