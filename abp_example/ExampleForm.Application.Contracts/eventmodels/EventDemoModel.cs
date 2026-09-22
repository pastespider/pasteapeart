using PasteApeart.Application.Contracts;
using PasteApeart.Helper;

namespace ExampleForm.Application.Contracts.eventmodels
{
    /// <summary>
    /// 案例模型 案例模型，业务实现参考这个
    /// </summary>
    public class EventDemoModel : AbsEventModel
    {
        /// <summary>
        /// 用户 这里是测试，表示输入一个用户UserId信息
        /// </summary>
        [PasteOuter("userInfo", "extendUser", "id", "userName")]
        public int UserId { get; set; }
    }
}
