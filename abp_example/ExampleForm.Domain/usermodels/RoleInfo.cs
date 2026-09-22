using PasteApeart.Domain.Entity;
using PasteApeart.Helper;
using System.ComponentModel.DataAnnotations;

namespace ExampleForm.usermodels
{
    /// <summary>
    /// 权限列表
    /// </summary>
    [PasteAuth("*", "root", "root")]
    [PasteShortModel("RoleShort", "ToShortRole", "Id", "Name")]
    public class RoleInfo : ILevelEntity<int>
    {
        /// <summary>
        /// 标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 模块
        /// </summary>
        [MaxLength(16)]
        public string Model { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        [MaxLength(16)]
        public string Role { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(16)]
        public string Name { get; set; }

        /// <summary>
        /// 图标名称 一般表示样式名称
        /// </summary>
        [MaxLength(32)]
        public string Icon { get; set; }

        /// <summary>
        /// 路径 作为菜单类型的路径
        /// </summary>
        [MaxLength(128)]
        public string Path { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(64)]
        public string Desc { get; set; }

        /// <summary>
        /// 权限类型 0权限1菜单2按钮
        /// </summary>
        public EnumRoleType RoleType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool IsEnable { get; set; } = true;

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; } = 100;

        /// <summary>
        /// 父级
        /// </summary>
        public int FatherId { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(20)]
        public string SortStr { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public int RootId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(64)]
        public string FatherStr { get; set; }
    }

}
