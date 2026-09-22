using PasteApeart.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PasteApeart.Application.Contracts
{
    /// <summary>
    /// 分页查询基础模型
    /// </summary>
    public class InputSearchBase
    {

        /// <summary>
        /// 页码
        /// </summary>
        [Description("页码")]
        [PasteQuery("page", true)]
        [Range(1, 10000, ErrorMessage = "页码范围不符合约定，最小1，最大10000，请重新输入!")]
        public int page { get; set; } = 1;

        /// <summary>
        /// 页大小
        /// </summary>
        [Description("页大小")]
        [PasteQuery("size", true)]
        [Range(5, 10000, ErrorMessage = "每页大小不符合约定，最小5，最大10000，请重新输入!")]
        public int size { get; set; } = 20;

        /// <summary>
        /// 关键字 键入关键字进行搜索
        /// </summary>
        [MaxLength(32)]
        [PasteQuery("word", false)]
        [Description("关键字 键入关键字进行搜索")]
        public string word { get; set; } = "";

        /// <summary>
        /// 前端显示方式
        /// </summary>
        [PasteHidden]
        [MaxLength(32)]
        [PasteQuery("show_style", true)]
        public string show_style { get; set; }

        /// <summary>
        /// 标记是否第一页数据和模型信息一起返回，值需要为one的时候才会启用
        /// </summary>
        [MaxLength(32)]
        [PasteHidden]
        [PasteQuery("request", true)]
        public string request { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [PasteHidden]
        [MaxLength(128)]
        [PasteQuery("orderby", true)]
        public string orderby { get; set; } = "Id desc";

    }
}
