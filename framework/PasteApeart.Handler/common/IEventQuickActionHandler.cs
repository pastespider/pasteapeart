using PasteApeart.Application.Contracts;
using PasteApeart.Helper;

namespace PasteApeart.Handler.common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventQuickActionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postBodyStr"></param>
        /// <returns></returns>
        Task<string> CreateItemAsync(string postBodyStr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PastePagedResult<QuickHandlerPropertyInfo>> PageAsync(InputSearchBase input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model_name"></param>
        /// <returns></returns>
        VoloModelInfo ReadAddModel(string model_name);
    }
}