using PasteApeart.Helper;

namespace PasteApeart.Application.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbsEventModel : IEventModel
    {
        /// <summary>
        /// 延迟 秒
        /// </summary>
        [PasteUnit("s")]
        public long ExpireSecond { get; set; }

        /// <summary>
        /// 截止 时间戳秒
        /// </summary>
        [PasteHidden]
        public long ExpireTime { get; set; }

        /// <summary>
        /// 对象名称 不需要外部赋值，在发送消息的时候会统一处理，其实就是Type.Name
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 排重键 写入格式要包含:id再使用ToAutoStartTaskMutexAsync的扩展的时候会进行替换，基于实际情况设定
        /// </summary>
        public virtual string MutexKey { get; set; }

        /// <summary>
        /// 排重值 主要用途在于任务排重，比如有多个地方对一个订单进行了统计任务，比如你延迟5分钟，中途多次触发，这个val就修改了，执行的时候，判断是否是最新的，就能判定是否要执行了
        /// </summary>
        public virtual string MutexVal { get; set; }

        /// <summary>
        /// 当前次数 每次运行的时候++
        /// </summary>
        [PasteHidden]
        public int RetryTime { get; set; }

        /// <summary>
        /// 最大次数 最大重试次数，0表示一次性任务，一般都是一次性任务
        /// </summary>
        public virtual int MaxRetryTime { get; set; }

        /// <summary>
        /// 重试模型 比如5s 30s 1m 5m 30m 1h还是比如1h 4h 8h 24h等，也就是重试的时候下一次啥时候执行，用哪个模型结算
        /// </summary>
        public virtual string RetryModel { get; set; }

        /// <summary>
        /// 订阅模式 当前是否是订阅模式
        /// </summary>
        public virtual bool IsSubscribe { get; set; } = false;
    }
}
