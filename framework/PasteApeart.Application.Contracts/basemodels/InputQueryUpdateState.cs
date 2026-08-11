using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteApeart.Application.Contracts
{
    /// <summary>
    /// 一般发生于列表页面的switch开关触发的事件
    /// </summary>
    public class InputQueryUpdateState
    {
        /// <summary>
        /// 数据ID 需要基于需求转化比如主表是Guid的则要对应转
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 将要的状态
        /// </summary>
        public bool state { get; set; }

        /// <summary>
        /// location.search
        /// </summary>
        public string search { get; set; }

    }
}
