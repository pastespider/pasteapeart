namespace PasteApeart.Application.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventModel
    {
        /// <summary>
        /// 延迟 秒
        /// </summary>
        long ExpireSecond { get; set; }

        /// <summary>
        /// 截止 时间戳秒,用于后续的，比如超过多久的消息丢弃，感觉和MutexKey MutexVal互斥了
        /// </summary>
        long ExpireTime { get; set; }

        /// <summary>
        /// 对象名称 也就是Model的名称，大小写一致
        /// </summary>
        string EventName { get; set; }

        /// <summary>
        /// 广播模式 是否是广播模式的消息，默认false，false为队列消息
        /// </summary>
        bool IsSubscribe { get; set; }

        //重试次数
        //最大重试次数
        //重试模型 内置或者默认值 new Random().Next(5,10)*time
    }


}
