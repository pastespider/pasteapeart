using PasteApeart;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExampleApeart.usermodels
{

    ///<summary>
    ///权限列表
    ///</summary>
    [Description("权限列表")]
    public class RoleInfoAddDto
    {
        /// <summary>
        /// 用于备份恢复使用不做UI用
        /// </summary>
        [PasteHidden]
        [JsonIgnore]
        [Description("用于备份恢复使用不做UI用")]
        public int Id { get; set; } = 0;

        ///<summary>
        ///名称 权限的友好名称
        ///</summary>
        [MaxLength(16)]
        [Required]
        [Description("名称 权限的友好名称")]
        public string Name { get; set; }

        /// <summary>
        /// 图标名称 一般表示样式名称,示例Hui-iconfont-menu
        /// </summary>
        [MaxLength(32)]
        [Description("图标名称 一般表示样式名称,示例Hui-iconfont-menu")]
        public string Icon { get; set; }

        /// <summary>
        /// 权限类型 0权限1菜单2按钮
        /// </summary>
        [PasteSingleRow]
        [PasteLselect("", "-1", 1)]
        [PasteChangeui("0", "model,role")]
        [PasteChangeui("1,2,3", "path")]
        [Description("权限类型 0权限1菜单2按钮")]
        public EnumRoleType RoleType { get; set; }

        ///<summary>
        ///模块 权限的一个归集，示例:user,表示用户模块
        ///</summary>
        [MaxLength(16)]
        [PastePoint]
        [Description("模块 权限的一个归集，示例:user,表示用户模块")]
        public string Model { get; set; }

        ///<summary>
        ///权限 对应权限，示例:add，表示新增
        ///</summary>
        [MaxLength(16)]
        [PastePoint]
        [Description("权限 对应权限，示例:add，表示新增")]
        public string Role { get; set; }

        /// <summary>
        /// 路径 作为菜单类型的路径
        /// </summary>
        [MaxLength(128)]
        [PastePoint]
        [Description("路径 作为菜单类型的路径")]
        public string Path { get; set; }

        ///<summary>
        ///描述
        ///</summary>
        [MaxLength(64)]
        [Description("描述")]
        public string Desc { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        [Description("状态")]
        public bool IsEnable { get; set; } = true;

        ///<summary>
        ///排序 排序基于层级和父级，横向排序，使用多层级排序规则
        ///</summary>
        [Description("排序 排序基于层级和父级，横向排序，使用多层级排序规则")]
        public int Sort { get; set; }

        ///<summary>
        ///父级 上一级是哪个，按需配置！
        ///</summary>
        [PasteQuery("fatherId", true)]
        [PasteOuter("roleInfo", "extendRole", "id", "name")]
        [Description("父级 上一级是哪个，按需配置！")]
        public int FatherId { get; set; }

        /// <summary>
        /// 子集
        /// </summary>
        [PasteHidden]
        [Description("子集")]
        public List<RoleInfoAddDto> Items { get; set; }

    }

    ///<summary>
    ///权限列表
    ///</summary>
    [Description("权限列表")]
    public class RoleInfoUpdateDto : RoleInfoAddDto
    {
        /// <summary>
        /// ID
        /// </summary>
        [PasteHidden]
        [Description("ID")]
        public new int Id { get; set; }

        /// <summary>
        /// 扩展 父级信息
        /// </summary>
        [PasteHidden]
        [Description("扩展 父级信息")]
        public RoleShortModel ExtendRole { get; set; }

    }

    ///<summary>
    ///权限列表
    ///</summary>
    [Description("权限列表")]
    public class RoleInfoDto : RoleInfoAddDto
    {
        /// <summary>
        /// ID
        /// </summary>
        [Description("ID")]
        public new int Id { get; set; }

        /// <summary>
        /// 父级 为了给上一个字段FatherId显示使用，你也可以把FatherId隐藏，只显示这个字段，或者2个都显示
        /// </summary>
        [PasteOuter("roleInfo", "extendFather", "id", "name")]
        [PasteDisplay]
        [Description("父级 为了给上一个字段FatherId显示使用，你也可以把FatherId隐藏，只显示这个字段，或者2个都显示")]
        public RoleShortModel ExtendRole { get; set; }

        ///<summary>
        ///层级
        ///</summary>
        [Description("层级")]
        public int Level { get; set; }

        ///<summary>
        ///排序串
        ///</summary>
        [Description("排序串")]
        public string SortStr { get; set; }

        ///<summary>
        ///根级
        ///</summary>
        [Description("根级")]
        public int RootId { get; set; }

        ///<summary>
        ///父级串
        ///</summary>
        [Description("父级串")]
        public string FatherStr { get; set; }

    }


    ///<summary>
    ///权限列表
    ///</summary>
    [PasteLinkQuery("fatherId")]
    [PasteButton("备份", "global_role_output(this);")]
    [PasteButton("恢复", "global_role_default(this);", "", "grady")]
    [Description("权限列表")]
    public class RoleInfoListDto
    {
        /// <summary>
        /// ID
        /// </summary>
        [PasteOrderby]
        [Description("ID")]
        public int Id { get; set; }

        ///<summary>
        ///名称
        ///</summary>
        [PasteClass]
        [PasteHtml("<div class=\"level level{{:=item.level}} role{{:=item.roleTypeInt}}\">{{ if(item.icon){ }}<i class=\"Hui-iconfont {{:=item.icon}}\"></i>{{ } }} {{:=item.name}}</div>")]
        [Description("名称")]
        public string Name { get; set; }

        ///<summary>
        ///模块 升序为Model，降序为Model desc
        ///</summary>
        [PasteClass]
        [PasteHidden]
        [PasteOrderby]
        [Description("模块 升序为Model，降序为Model desc")]
        public string Model { get; set; }

        ///<summary>
        ///权限
        ///</summary>
        [PasteClass]
        [PasteHidden]
        [Description("权限")]
        public string Role { get; set; }

        /// <summary>
        /// 图标名称 一般表示样式名称
        /// </summary>
        [MaxLength(32)]
        [PasteHidden]
        [Description("图标名称 一般表示样式名称")]
        public string Icon { get; set; }

        /// <summary>
        /// 路径 作为菜单类型的路径
        /// </summary>
        [MaxLength(128)]
        [PasteClass]
        [PasteHidden]
        [Description("路径 作为菜单类型的路径")]
        public string Path { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        [PasteHtml("<div class=\"title\">{{:=item.path}}</div><div class=\"desc\">{{:=item.model}} {{:=item.role}}</div>")]
        [PasteClass]
        public string Info { get; set; }

        /// <summary>
        /// 权限类型 0权限1菜单2按钮,如何让前端支持这个显示
        /// </summary>
        [Description("权限类型 0权限1菜单2按钮,如何让前端支持这个显示")]
        public EnumRoleType RoleType { get; set; }

        /// <summary>
        /// 权限类型
        /// </summary>
        [PasteHidden]
        [Description("权限类型")]
        public int RoleTypeInt { get { return (int)RoleType; } }

        ///<summary>
        ///描述
        ///</summary>
        [PasteClass]
        [Description("描述")]
        public string Desc { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        [PasteSwitch("view")]
        [Description("状态")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 绑定 扩展基于角色的时候是否拥有这个权限
        /// </summary>
        [PasteSwitch("bind")]
        [PasteHidden("bind")]
        [PasteExtendBind("GradeRole", "RoleId", "GradeId", "grade_id")]
        [Description("绑定 扩展基于角色的时候是否拥有这个权限")]
        public bool ExtendBind { get; set; }

        ///<summary>
        ///排序
        ///</summary>
        [PasteOrderby("Sort", "Sort desc")]
        [Description("排序")]
        public int Sort { get; set; }

        ///<summary>
        ///父级
        ///</summary>
        [Description("父级")]
        [PasteHidden]
        public int FatherId { get; set; }

        /// <summary>
        /// 父级 信息，为了给上一个字段FatherId显示使用，你也可以把FatherId隐藏，只显示这个字段，或者2个都显示
        /// </summary>
        [PasteDisplay()]
        [Description("父级 信息，为了给上一个字段FatherId显示使用，你也可以把FatherId隐藏，只显示这个字段，或者2个都显示")]
        public RoleShort ExtendFather { get; set; }

        ///<summary>
        ///层级
        ///</summary>
        [PasteOrderby("Level", "Level desc")]
        [Description("层级")]
        public int Level { get; set; }

        ///<summary>
        ///排序串
        ///</summary>
        [PasteHidden]
        [Description("排序串")]
        public string SortStr { get; set; }

        ///// <summary>
        ///// 添加子权限
        ///// </summary>
        //[PasteMenu("添加子集", "open_window('添加子权限','./pasteform/view.html?path=roleInfo&fatherId={{:=item.id}}');")]
        //[Description("添加子权限")]
        //public int Menu1 { get; set; }

        /// <summary>
        /// 详情
        /// </summary>
        [PasteMenu("详情", "open_window('详细','./pasteform/detail.html?path=roleInfo&id={{:=item.id}}');")]
        [Description("详情")]
        public int Menu2 { get; set; }

        /// <summary>
        /// 子项
        /// </summary>
        [PasteMenu("子项", "open_window('子项','./pasteform/index.html?path=roleInfo&fatherId={{:=item.id}}','90%','90%');")]
        [Description("子项")]
        public int Menu3 { get; set; }

    }

    ///<summary>
    ///权限列表
    ///</summary>
    [Description("权限列表")]
    public class RoleMenuModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /////<summary>
        /////权限模块
        /////</summary>
        //public string Model { get; set; }

        /////<summary>
        /////权限
        /////</summary>
        //public string Role { get; set; }

        ///<summary>
        ///权限项
        ///</summary>
        [Description("权限项")]
        public string Name { get; set; }
        /// <summary>
        /// 路径 作为菜单类型的路径
        /// </summary>
        [MaxLength(128)]
        [Description("路径 作为菜单类型的路径")]
        public string Path { get; set; }

        /// <summary>
        /// 图标名称 一般表示样式名称
        /// </summary>
        [MaxLength(32)]
        [Description("图标名称 一般表示样式名称")]
        public string Icon { get; set; }

        ///// <summary>
        ///// 权限类型 0权限1菜单2按钮
        ///// </summary>
        //public int RoleType { get; set; }

        ///<summary>
        ///描述
        ///</summary>
        [Description("描述")]
        public string Desc { get; set; }

        ///<summary>
        ///排序
        ///</summary>
        [Description("排序")]
        public int Sort { get; set; }

        ///<summary>
        ///父级ID
        ///</summary>
        [Description("父级ID")]
        public int FatherId { get; set; }

        ///<summary>
        ///层级
        ///</summary>
        [Description("层级")]
        public int Level { get; set; }


    }

    ///<summary>
    ///权限列表 这个作为登陆后权限缓存，其他的没必要缓存，用不上
    ///</summary>
    [Description("权限列表 这个作为登陆后权限缓存，其他的没必要缓存，用不上")]
    public class RoleAuthDto : EntityDto<int>, IPasteAuthRole
    {
        ///<summary>
        ///权限模块
        ///</summary>
        [Description("权限模块")]
        public string Model { get; set; }

        ///<summary>
        ///权限
        ///</summary>
        [Description("权限")]
        public string Role { get; set; }

    }

    ///<summary>
    ///权限列表
    ///</summary>
    [Description("权限列表")]
    public class RoleShortModel
    {
        /// <summary>
        /// ID
        /// </summary>
        [Description("ID")]
        public int Id { get; set; }

        ///<summary>
        ///权限模块
        ///</summary>
        [Description("权限模块")]
        public string Model { get; set; }

        ///<summary>
        ///权限
        ///</summary>
        [Description("权限")]
        public string Role { get; set; }

        ///<summary>
        ///名称
        ///</summary>
        [Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 路径 作为菜单类型的路径
        /// </summary>
        [Description("路径 作为菜单类型的路径")]
        public string Path { get; set; }
        /// <summary>
        /// 权限类型 0权限1菜单2按钮
        /// </summary>
        [Description("权限类型 0权限1菜单2按钮")]
        public EnumRoleType RoleType { get; set; }

        /////<summary>
        /////描述
        /////</summary>
        //public string Desc { get; set; }

        /////<summary>
        /////状态
        /////</summary>
        //public bool IsEnable { get; set; }

        /////<summary>
        /////排序
        /////</summary>
        //public int Sort { get; set; }

        /////<summary>
        /////父级
        /////</summary>
        //public int FatherId { get; set; }

        /////<summary>
        /////层级
        /////</summary>
        //public int Level { get; set; }

        /////<summary>
        /////
        /////</summary>
        //public string SortStr { get; set; }

        /////<summary>
        /////
        /////</summary>
        //public int RootId { get; set; }

        /////<summary>
        /////
        /////</summary>
        //public string FatherStr { get; set; }

    }

    ///<summary>
    ///权限列表
    ///</summary>
    [Description("权限列表")]
    public class RoleShort
    {
        /// <summary>
        /// ID
        /// </summary>
        [Description("ID")]
        public int Id { get; set; }

        ///<summary>
        ///名称
        ///</summary>
        [Description("名称")]
        public string Name { get; set; }

    }

    ///<summary>
    /// 查询
    ///</summary>
    [Description("查询")]
    public class InputQueryRoleInfo : InputSearchBase
    {

        /// <summary>
        /// 父级ID 基于父级ID查询
        /// </summary>
        [PasteOuter("roleInfo")]
        [PasteQuery("fatherId", true)]
        [Description("父级ID 基于父级ID查询")]
        public int FatherId { get; set; }

        /// <summary>
        /// 角色ID 查询某一个角色拥有多少权限
        /// </summary>
        [PasteQuery("grade_id", true)]
        [PasteHidden]//默认隐藏，因为只要query触发
        [PasteExtendBind("GradeRole", "RoleId", "GradeId", "grade_id")]
        [Description("角色ID 查询某一个角色拥有多少权限")]
        public int GradeId { get; set; }

        /// <summary>
        /// 权限类型 基于权限类型查询
        /// </summary>
        [PasteLselect("", "", 1)]
        [Description("权限类型 基于权限类型查询")]
        [PasteField("RoleType")]
        public EnumRoleType role_type { get; set; } = EnumRoleType.All;

        /// <summary>
        /// 排序 这个字段是隐藏的
        /// </summary>
        [PasteHidden]
        [Description("排序 这个字段是隐藏的")]
        public new string orderby { get; set; } = "SortStr";

    }
}