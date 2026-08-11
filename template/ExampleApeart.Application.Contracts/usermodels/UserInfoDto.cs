using PasteApeart;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace $ext_safeprojectname$.usermodels
{

    ///<summary>
    ///用户信息
    ///</summary>
    [Description("用户信息")]
    public class UserInfoAddDto
    {
        ///<summary>
        ///邮箱
        ///</summary>
        [MaxLength(64)]
        [Required]
        [Description("邮箱")]
        public string Email { get; set; } = "";

        ///<summary>
        ///密码
        ///</summary>
        [MaxLength(64)]
        [PastePassword]
        [Required]
        [Description("密码")]
        public string PassWord { get; set; } = "";

        ///<summary>
        ///用户名
        ///</summary>
        [MaxLength(32)]
        [Required]
        [Description("用户名")]
        public string UserName { get; set; } = "";

        ///<summary>
        ///描述
        ///</summary>
        [MaxLength(64)]
        [Description("描述")]
        public string Desc { get; set; } = "";

        ///<summary>
        ///头像
        ///</summary>
        [MaxLength(128)]
        [PasteImage(1, "head", "60*60")]
        [Description("头像")]
        public string UserHead { get; set; } = "";

        ///<summary>
        ///状态
        ///</summary>
        [Description("状态")]
        public bool IsEnable { get; set; } = true;

        /// <summary>
        /// 角色 包含的角色，这个暂未实现，应该会归纳到outers去
        /// </summary>
        [PasteOuters("gradeInfo", "", "id", "name")]
        [Description("角色 包含的角色，这个暂未实现，应该会归纳到outers去")]
        public int[] GradeIds { get; set; }

    }

    ///<summary>
    ///用户信息
    ///</summary>
    [Description("用户信息")]
    public class UserInfoUpdateDto : UserInfoAddDto
    {
        /// <summary>
        /// 标识
        /// </summary>
        public int Id { get; set; }

        ///<summary>
        ///密码
        ///</summary>
        [MaxLength(64)]
        [PasteSort(10)]
        [PastePassword()]
        [Description("密码 不修改密码保持留空")]
        public new string PassWord { get; set; } = "";

        #region 以下是扩展信息 扩展测试

        /// <summary>
        /// 角色 测试用的
        /// </summary>
        [PasteHidden]
        [PasteShort("GradeInfo", "ExtendGrade", "ToShortGrade()")]
        [Description("角色 测试用的")]
        public int GradeId { get; set; } = 1;

        /// <summary>
        /// 角色 测试用的
        /// </summary>
        [PasteHidden]
        [Description("角色 测试用的")]
        public GradeShort ExtendGrade { get; set; }

        #endregion


        /// <summary>
        /// 扩展 包含的角色列表
        /// </summary>
        [PasteHidden]
        [Description("扩展 包含的角色列表")]
        public List<GradeInfoListDto> Grades { get; set; }

    }

    ///<summary>
    ///用户信息
    ///</summary>
    [Description("用户信息")]
    public class UserInfoDto : UserInfoUpdateDto
    {
        ///<summary>
        ///创建日期
        ///</summary>
        [Description("创建日期")]
        public DateTime CreateDate { get; set; }

    }

    ///<summary>
    ///用户信息
    ///</summary>
    [Description("用户信息")]
    [PasteEnable(0, 1, 0, 1)]
    public class UserInfoListDto : EntityDto<int>
    {
        ///<summary>
        ///用户名
        ///</summary>
        [PasteSearch]
        [Description("用户名")]
        public string UserName { get; set; }

        ///<summary>
        ///头像
        ///</summary>
        [PasteImage]
        [Description("头像")]
        public string UserHead { get; set; }

        ///<summary>
        ///创建日期
        ///</summary>
        [Description("创建日期")]
        public DateTime CreateDate { get; set; }

        ///<summary>
        ///描述
        ///</summary>
        [PasteSearch]
        [Description("描述")]
        public string Desc { get; set; }

        ///<summary>
        ///邮箱
        ///</summary>
        [PasteSearch]
        [Description("邮箱")]
        public string Email { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        [PasteSwitch]
        [Description("状态")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 菜单
        /// </summary>
        [PasteMenu("授权角色", "open_window('授权角色','pasteform/index.html?path=gradeInfo&model=bind&user_id={{:=item.id}}','80%','80%')")]
        [Description("菜单")]
        public int Menu1 { get; } = 0;

    }

    ///<summary>
    ///用户信息
    ///</summary>
    [Description("用户信息")]
    public class UserShort
    {
        /// <summary>
        /// 标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Description("用户名")]
        public string Name { get { return UserName; } }

        ///<summary>
        ///用户名
        ///</summary>
        [Description("用户名")]
        public string UserName { get; set; }

        ///<summary>
        ///头像
        ///</summary>
        [Description("头像")]
        public string UserHead { get; set; }

    }

    ///<summary>
    ///用户信息
    ///</summary>
    [Description("用户信息")]
    public class UserShortModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        ///<summary>
        ///用户名
        ///</summary>
        [Description("用户名")]
        public string UserName { get; set; }

        ///<summary>
        ///头像
        ///</summary>
        [Description("头像")]
        public string UserHead { get; set; }

    }

    ///<summary>
    /// 查询
    ///</summary>
    [Description("查询")]
    public class InputQueryUserInfo : InputSearchBase
    {
        /// <summary>
        /// 注册区间
        /// </summary>
        [PasteField("CreateDate")]
        [PasteDaterange("sdate", "edate")]
        [Description("注册区间")]
        public DateTime? sdate { get; set; }

        /// <summary>
        /// 日期区间
        /// </summary>
        [Description("日期区间")]
        public DateTime? edate { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [PasteField("Email")]
        [Description("邮箱")]
        public string email { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [PasteField("Desc")]
        [Description("描述")]
        public string Desc { get; set; }


    }
}