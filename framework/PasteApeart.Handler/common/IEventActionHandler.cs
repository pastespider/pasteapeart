using PasteApeart.Application.Contracts;

namespace PasteApeart.Handler
{
    public interface IEventActionHandler
    {
        /// <summary>
        /// 基于jsonString的格式 执行事件 eventModel的类型在构造中注入
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task ActionMessageAsync(string msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [Obsolete("use ActionMessageAsync")]
        Task HanlderEventMessage(string msg);

        /// <summary>
        /// 基于jsonString的格式 执行事件 直接入参eventModel的集合
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="eventModelTypes"></param>
        /// <returns></returns>
        Task ActionMessageAsync(string msg, Type[] eventModelTypes);

        /// <summary>
        /// 一般用于本地的Channel模式
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        Task ActionMessageAsync(IEventModel msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        [Obsolete("use ActionMessageAsync")]
        Task HanlderEventMessage(IEventModel msg);
    }
}