using PasteApeart.Application.Contracts;

namespace PasteApeart.Handler
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="EventModel"></typeparam>
    public interface IEventHandler<EventModel> where EventModel : IEventModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task HandleEventAsync(EventModel input);

        ///// <summary>
        ///// 执行结果
        ///// </summary>
        //object Result { get; }
    }
}
