using PasteApeart.Application.Contracts;

namespace PasteApeart.Handler
{
    public interface IEventActionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task HandlerEventMessage(string msg);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        Task HanlderEventMessage(IEventModel read);
    }
}