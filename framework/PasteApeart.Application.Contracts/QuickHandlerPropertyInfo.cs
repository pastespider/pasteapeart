using PasteApeart.Helper;
using System.ComponentModel;

namespace PasteApeart.Application.Contracts
{
    /// <summary>
    /// 事件归集 用于管理员模式直接执行任务
    /// </summary>
    [PasteEnable(0, 0, 0, 0)]
    [Description("事件归集 归集当前程序可以执行的AllInHandler的事件模型")]
    public class QuickHandlerPropertyInfo
    {
        /// <summary>
        /// 代码
        /// </summary>
        [PasteClass]
        [Description("代码")]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [PasteClass]
        [Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [PasteClass("fleft idesc")]
        [Description("说明")]
        public string Desc { get; set; }

        /// <summary>
        /// 字段数
        /// </summary>
        [Description("字段数")]
        public int FieldCount { get; set; }

        /// <summary>
        /// 去执行
        /// </summary>
        [PasteMenuhtml]
        [Description("去执行")]
        public string Menu1 => $"<a href=\"javascript:;\" onclick=\"open_window('去创建{Code}','./pasteform/view.html?path=quickHandler&model_name={Code}')\">去执行</a>";

    }
}
