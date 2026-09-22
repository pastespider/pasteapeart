using PasteApeart;
using PasteApeart.Applciation.Contracts.EntityDto;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;

namespace ExampleApeart.usermodels
{

    ///<summary>
    ///角色绑定权限
    ///</summary>
    [Description("角色绑定权限")]
    public class GradeRoleAddDto
    {
        ///<summary>
        ///角色ID 点击选择角色
        ///</summary>
        [PasteQuery("grade_id", false)]
        [Description("角色ID 点击选择角色")]
        [PasteOuter("gradeInfo", "extendGrade", "id", "name")]
        [PasteShort("GradeInfo", "ExtendGrade", "ToGradeShort()")]
        public int GradeId { get; set; }
        ///<summary>
        ///权限ID 点击选择权限
        ///</summary>
        [PasteQuery("role_id", false)]
        [Description("权限ID 点击选择权限")]
        [PasteOuter("roleInfo", "extendRole", "id", "name")]
        [PasteShort("RoleInfo", "ExtendRole", "ToRoleShort()")]
        public int RoleId { get; set; }
    }

    ///<summary>
    ///角色绑定权限
    ///</summary>
    [Description("角色绑定权限")]
    public class GradeRoleUpdateDto : GradeRoleAddDto
    {

        ///<summary>
        ///ID
        ///</summary>
        [PasteHidden]
        [Description("ID")]
        public int Id { get; set; }


        ///<summary>
        /// 权限 表示接口的权限
        ///</summary>
        [PasteDisplay]
        [Description("权限 表示接口的权限")]
        public RoleShort ExtendRole { get; set; }


        ///<summary>
        /// 角色 表示权限的一个分组
        ///</summary>
        [PasteDisplay]
        [Description("角色 表示权限的一个分组")]
        public GradeShort ExtendGrade { get; set; }

    }

    ///<summary>
    ///角色绑定权限
    ///</summary>
    [Description("角色绑定权限")]
    public class GradeRoleDto : GradeRoleUpdateDto
    {



    }

    ///<summary>
    ///角色绑定权限
    ///</summary>
    [Description("角色绑定权限")]
    [PasteLinkQuery("role_id,grade_id")]
    public class GradeRoleListDto : EntityDto<int>
    {
        ///<summary>
        ///角色ID 点击选择角色
        ///</summary>
        [Description("角色ID 点击选择角色")]
        [PasteShort("GradeInfo", "ExtendGrade", "ToGradeShort()")]
        [PasteHidden()]
        public int GradeId { get; set; }
        ///<summary>
        ///权限ID 点击选择权限
        ///</summary>
        [Description("权限ID 点击选择权限")]
        [PasteShort("RoleInfo", "ExtendRole", "ToRoleShort()")]
        [PasteHidden()]
        public int RoleId { get; set; }


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
    public class InputQueryGradeRole : InputSearchBase
    {
        ///<summary>
        ///角色ID 点击选择角色
        ///</summary>
        [PasteQuery("grade_id", false)]
        [PasteOuter("gradeInfo", "extendGrade", "id", "name")]
        [Description("角色ID 点击选择角色")]
        public int GradeId { get; set; }

        ///<summary>
        ///权限ID 点击选择权限
        ///</summary>
        [PasteQuery("role_id", false)]
        [PasteOuter("roleInfo", "extendRole", "id", "name")]
        [Description("权限ID 点击选择权限")]
        public int RoleId { get; set; }
    }

}