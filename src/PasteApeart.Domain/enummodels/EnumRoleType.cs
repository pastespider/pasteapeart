using System.ComponentModel;

namespace PasteApeart
{
    /// <summary>
    /// 权限类型
    /// </summary>
    [Description("权限类型")]
    public enum EnumRoleType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All = -1,
        /// <summary>
        /// <![CDATA[<i_*_style="color:green;">权限</i>]]>
        /// </summary>
        [Description("权限")]
        Role = 0,
        /// <summary>
        /// 菜单
        /// </summary>
        [Description("<span_*_style=\"background:#f3f3f3;color:blue;\">菜单</span>")]
        Menu = 1,
        /// <summary>
        /// 权限
        /// </summary>
        [Description("<span_*_style=\"color:red;\">按钮</span>")]
        Button = 2,
        /// <summary>
        /// 小程序菜单
        /// </summary>
        [Description("小程序菜单")]
        MiniMenu = 3
    }
}
