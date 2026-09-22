using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleForm.Application.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public class QuickHandlerModelDto: QuickHandlerPropertyInfo
    {

        /// <summary>
        /// 去执行
        /// </summary>
        [PasteMenuhtml]
        [Description("去执行")]
        public new string Menu1 => $"<a href=\"javascript:;\" onclick=\"open_window('去创建{Code}','./pasteform/view.html?path=quickHandler&model_name={Code}')\">去执行</a>";
    }
}
