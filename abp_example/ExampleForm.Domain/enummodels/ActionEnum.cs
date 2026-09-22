using System.ComponentModel;

namespace ExampleForm.testmodels
{
    /// <summary>
    /// 动作枚举
    /// </summary>
    [Description("动作枚举")]
    public enum ActionEnum
    {
        /// <summary>
        /// 查看所有
        /// </summary>
        [Description("查看所有")]
        all = -1,
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        start = 1,
        /// <summary>
        /// 停止
        /// </summary>
        [Description("停止")]
        stop = 4,
        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        cannel = 5
    }
}
