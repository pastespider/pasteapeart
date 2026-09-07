using PasteApeart.Application.Contracts;
using System.Threading.Channels;

namespace PasteApeart
{
    public interface IChannelHelper
    {
        /// <summary>
        /// 
        /// </summary>
        Channel<IEventModel> ChannelEventModel { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task WriteAutoQueueAsync(IEventModel input);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isremove"></param>
        /// <returns></returns>
        [Obsolete]
        Task WriteBroadcastMessageAsync(IEventModel input, bool isremove);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        [Obsolete]
        Task WriteMutexMQAsync<T>(T input) where T : IEventModel;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Obsolete]
        Task WriteMutexMQAsync<T>(T input, object id) where T : IEventModel;
    }
}