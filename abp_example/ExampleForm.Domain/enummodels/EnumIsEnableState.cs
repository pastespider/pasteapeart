using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleForm
{
    /// <summary>
    /// 状态枚举
    /// </summary>
    [Description("状态枚举")]
    public enum EnumIsEnableState
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All = 0,
        /// <summary>
        /// 是
        /// </summary>
        [Description("是")]
        True = 1,
        /// <summary>
        /// 否
        /// </summary>
        [Description("否")]
        False = 2
    }
}
