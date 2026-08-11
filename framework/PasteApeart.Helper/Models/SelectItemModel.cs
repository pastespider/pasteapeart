using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteApeart.Helper.Models
{
    /// <summary>
    /// Select Lselect的可选格式
    /// </summary>
    public class SelectItemModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 选中值
        /// </summary>
        public int value { get; set; }
    }
}
