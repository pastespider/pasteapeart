using PasteApeart.Application.Contracts;
using PasteApeart.Helper;

namespace ExampleApeart.Application.Contracts.eventmodels
{
    /// <summary>
    /// 用户创建后 案例，用户创建后
    /// </summary>
    public class EventAfterUserCreateModel : AbsEventModel
    {
        /// <summary>
        /// 账号
        /// </summary>
        [PasteOuter("userInfo")]
        public int UserId { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public int CreateUserId { get; set; }
    }
}
