//using PasteApeart.Application.Contracts.eventmodels;
//using Microsoft.Extensions.Logging;

//namespace PasteApeart.Handler.eventmodels
//{
//    /// <summary>
//    /// 案例 EventDemoModel对应的业务执行案例，管好自己的一亩三分地的哲学就在这里体现了
//    /// </summary>
//    public class EventDemoModelHandler : IEventHandler<EventDemoModel>, ITransientDependency
//    {
//        private readonly IAppCache _appCache;

//        private readonly ILogger<EventDemoModelHandler> _logger;

//        private readonly IChannelHelper _channelHelper;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="appCache"></param>
//        /// <param name="logger"></param>
//        /// <param name="channelHelper"></param>
//        public EventDemoModelHandler(IAppCache appCache, ILogger<EventDemoModelHandler> logger, IChannelHelper channelHelper)
//        {
//            _appCache = appCache;
//            _logger = logger;
//            _channelHelper = channelHelper;
//        }



//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task HandleEventAsync(EventDemoModel input)
//        {
//            //按需调用 是否排重，比如统计某一个信息，有时候只要统计一次，最后一次得执行即可，前面得都可以抛弃
//            if (!await input.ToMatchTaskMutexAsync(_appCache))
//            {
//                _logger.LogWarning($"{nameof(EventDemoModelHandler)} 任务被重置了，当前任务不需要处理 ");
//                return;
//            }

//            //按需使用并发锁
//            var lockKey = $"lock:{nameof(EventDemoModel)}:{input.UserId}";
//            if(!await _appCache.LockAsync(lockKey, 10))
//            {
//                //设置重试 
//                await input.ToAutoExpireAndSendAsync(_channelHelper);
//                return;
//            }

//            try
//            {

//                //业务代码

//            }catch(Exception exl)
//            {
//                _logger.LogException(exl);
//            }
//            finally
//            {
//                await _appCache.UnLockAsync(lockKey);
//            }

//            ////启动一个其他的任务，并带上延迟，下面例子标识过10分钟后启动EventDemoModel对应的业务任务
//            //var newTaskModel = new EventDemoModel { };
//            ////选填 任务是否延迟触发，否则立马触发(当然这个立马中间还是需要消耗一些时效的，比如MQ的中间通讯，比如Channel的排队等)
//            //newTaskModel.ExpireSecond = 600;
//            ////业务代码的其他赋值
//            ////选填 表示启动任务标记，一般用于任务排重，比如git提交后1个小时后执行gc,你这样操作后，那么只有最后一个任务会被执行
//            //await newTaskModel.ToStartTaskMutexAsync(_appCache,"3");
//            ////消息压入到队列，基于配置走Channel或者比如RabbitMQ
//            //await _channelHelper.WriteAutoQueueAsync(newTaskModel);

//            ////基于对AbsEventModel的扩展，你还可以实现重试机制，比如异常后5s 10s 30s 5m等非等时重试

//            //是否包裹异常处理，看业务需求，反正队列消费者那边会有异常处理

//        }
//    }
//}
