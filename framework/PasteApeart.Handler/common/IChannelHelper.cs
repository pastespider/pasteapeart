using PasteApeart.Application.Contracts;
using System.Threading.Channels;

namespace PasteApeart.Handler
{
    /// <summary>
    /// 
    /// </summary>
    public interface IChannelHelper
    {
        /// <summary>
        /// 
        /// </summary>
        Channel<IEventModel> ChannelEventModel { get; }

        /// <summary>
        /// 压入到队列 如果MutexKey有值而MutexVal没有，则会进行赋值 如果ExpireSecond大于0则压入延迟队列，否则是及时队列
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
        [Obsolete("use WriteAutoQueueAsync set IsSubscribe=true")]
        Task WriteBroadcastMessageAsync(IEventModel input, bool isremove);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        [Obsolete("use WriteAutoQueueAsync set MutexKey !=stringEmpty and MutexVal is null")]
        Task WriteMutexMQAsync<T>(T input) where T : IEventModel;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Obsolete("use WriteAutoQueueAsync set MutexKey !=stringEmpty and MutexVal is null")]
        Task WriteMutexMQAsync<T>(T input, object id) where T : IEventModel;
    }
}