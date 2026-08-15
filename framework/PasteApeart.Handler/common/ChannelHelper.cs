using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;

namespace PasteApeart
{
    /// <summary>
    /// 
    /// </summary>
    public class ChannelHelper : ISingletonDependency, IChannelHelper
    {

        //private readonly Channel<ChannelNoticeModel> _channelnotice;
        private readonly Channel<IEventModel> _channelLocationQueue;
        private readonly ILogger<ChannelHelper> _logger;
        //private readonly AppConfig _config;
        private readonly ConcurrentDictionary<long, List<IEventModel>> queueList;
        private readonly IRabbitMQProducer _producer;
        private readonly RabbitConfig _rabbitConfig;

        private readonly IAppCache _appCache;

        /// <summary>
        /// 
        /// </summary>
        public ChannelHelper(
            ILogger<ChannelHelper> logger,
            IRabbitMQProducer producer,
            IAppCache appCache,
            IOptions<RabbitConfig> rabbitConfig
            )
        {
            //_channelnotice = Channel.CreateBounded<ChannelNoticeModel>(1000);
            _channelLocationQueue = Channel.CreateBounded<IEventModel>(10000);
            queueList = new ConcurrentDictionary<long, List<IEventModel>>();
            _logger = logger;
            _rabbitConfig = rabbitConfig.Value;
            _appCache = appCache;
            //_config = config.Value;
            if (!_rabbitConfig.RabbitQueue)
            {
                _ = PeriodicTimerHandlerAsync(default);
            }
            else
            {
                _producer = producer;
            }

        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="input"></param>
        //public async Task Write(ChannelNoticeModel input)
        //{
        //    await _channelnotice.Writer.WriteAsync(input);
        //}

        /// <summary>
        /// 自动写入最近的延迟队列 本地或者远端
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task WriteAutoQueueAsync(IEventModel input)
        {
            if (String.IsNullOrEmpty(input.EventName))
            {
                input.EventName = input.GetType().Name;
            }
            if (input.ExpireSecond <= 0)
            {
                await SendQueueMessageAsync(input);
            }
            else
            {
                if (_rabbitConfig.RabbitQueue)
                {
                    var send = false;
                    if (_rabbitConfig.ExpireTimeList != null)
                    {
                        var find = _rabbitConfig.ExpireTimeList.Where(x => x.ExpireSecond <= input.ExpireSecond).OrderByDescending(x => x.ExpireSecond).FirstOrDefault();
                        if (find != null)
                        {
                            input.ExpireSecond = input.ExpireSecond - find.ExpireSecond;
                            await this.SendRemoteMessageAsync(input, find.ExpireExchange);
                            send = true;
                        }
                    }
                    if (!send)
                    {
                        input.ExpireSecond = 0;
                        await this.SendQueueMessageAsync(input);
                    }
                }
                else
                {
                    await this.WriteLocationMessageAsync(input, input.ExpireSecond);
                }
            }
        }


        /// <summary>
        /// 需要设定MutexKey的时候调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task WriteMutexMQAsync<T>(T input) where T : IEventModel
        {
            if (input is AbsEventModel abs)
            {
                await abs.ToStartTaskMutexAsync(_appCache, null, 5);
            }
            await WriteAutoQueueAsync(input);
        }

        /// <summary>
        /// 需要设定MutexKey的时候调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task WriteMutexMQAsync<T>(T input, object id) where T : IEventModel
        {
            if (input is AbsEventModel abs)
            {
                await abs.ToStartTaskMutexAsync(_appCache, id, 5);
            }
            await WriteAutoQueueAsync(input);
        }

        /// <summary>
        /// 处理本地延迟队列
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task PeriodicTimerHandlerAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                TimeSpan inter = TimeSpan.FromSeconds(1);
                using var timer = new PeriodicTimer(inter);
                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    //每隔1秒钟这里会一直重复执行么？ 不考虑执行任务耗时
                    #region
                    var now = DateTime.Now.ToUnixTimeSeconds();
                    var finds = queueList.Keys.Where(x => x < now).ToArray();
                    if (finds?.Length > 0)
                    {
                        var findValues = queueList.Where(x => finds.Contains(x.Key)).ToArray();
                        if (findValues?.Length > 0)
                        {
                            foreach (var item in findValues)
                            {
                                if (item.Value != null)
                                {
                                    foreach (var ival in item.Value)
                                    {
                                        await WriteLocationMessageAsync(ival);
                                    }
                                }
                            }
                        }
                        foreach (var item in findValues)
                        {
                            queueList.TryRemove(item.Key, out _);
                        }
                    }
                    //查找是否有过期的任务，进行执行!
                    #endregion
                }
            }
            catch (Exception exlAll)
            {
                _logger.LogException(exlAll);
            }
        }

        /// <summary>
        /// 写入本地 延迟队列
        /// </summary>
        /// <param name="input"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private async Task WriteLocationMessageAsync(IEventModel input, long second)
        {
            if (input.ExpireSecond > 0)
            {
                input.ExpireSecond = 0;
            }
            if (second > 0)
            {
                var distionTime = DateTime.Now.ToUnixTimeSeconds() + second;
                queueList.AddOrUpdate(distionTime, (a) => new List<IEventModel> { input }
                , (c, d) =>
                {
                    d.Add(input);
                    return d;
                });
            }
            else
            {
                await WriteLocationMessageAsync(input);
            }
        }

        /// <summary>
        /// 直接发送本地
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task WriteLocationMessageAsync(IEventModel input)
        {
            //_logger.LogInformation($"本地MQ写入:{JsonSerializer.Serialize(input, input.GetType(), JsonConfig.Options)}");
            await _channelLocationQueue.Writer.WriteAsync(input);
        }

        /// <summary>
        /// 写入队列 本地或者远程
        /// </summary>
        /// <param name="_info"></param>
        /// <returns></returns>
        private async Task SendQueueMessageAsync(IEventModel _info)
        {
            if (_rabbitConfig.RabbitQueue)
            {
                await _producer.SendWithRetryAsync(_rabbitConfig.ExpiredExchange, JsonSerializer.Serialize(_info, _info.GetType(), JsonConfig.Options), _rabbitConfig.ExpireRouteKey);
            }
            else
            {
                //await _channelLocationQueue.Writer.WriteAsync(_info);
                await WriteLocationMessageAsync(_info);
            }
        }

        /// <summary>
        /// 直接发送远端
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        private async Task SendRemoteMessageAsync(IEventModel info, string exchange)
        {
            await _producer.SendWithRetryAsync(exchange, JsonSerializer.Serialize(info, info.GetType(), JsonConfig.Options), _rabbitConfig.ExpireRouteKey);
        }



        /// <summary>
        /// 写入广播事件
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isremove"></param>
        /// <returns></returns>
        public async Task WriteBroadcastMessageAsync(IEventModel input, bool isremove)
        {
            if (input == null) { return; }
            input.EventName = input.GetType().Name;
            if (_rabbitConfig.SubscribeEnable)
            {
                await _producer.WriteSubscribeAsync(System.Text.Json.JsonSerializer.Serialize(input, input.GetType(), JsonConfig.Options));
            }
            else
            {
                _logger.LogError("没有配置广播模式，当前消息是发给广播模式的");
            }
        }


        ///// <summary>
        ///// 
        ///// </summary>
        //public Channel<ChannelNoticeModel> Notice { get { return _channelnotice; } }

        /// <summary>
        /// 
        /// </summary>
        public Channel<IEventModel> ChannelEventModel { get { return _channelLocationQueue; } }

    }

    ///// <summary>
    ///// 
    ///// </summary>
    //public class ChannelNoticeModel
    //{
    //    /// <summary>
    //    /// 内容
    //    /// </summary>
    //    public string body { get; set; }

    //    /// <summary>
    //    /// 动作名称 waitrevice:任务待领取 finish:任务完成 cannel:任务取消 waithand:待处理 waitreply:待反馈
    //    /// </summary>
    //    public string action { get; set; }

    //    /// <summary>
    //    /// 用户id
    //    /// </summary>
    //    public int userid { get; set; }

    //    /// <summary>
    //    /// 部门ID
    //    /// </summary>
    //    public int partid { get; set; }

    //    /// <summary>
    //    /// 整个项目组
    //    /// </summary>
    //    public int projectid { get; set; }

    //    /// <summary>
    //    /// 排除谁
    //    /// </summary>
    //    public int exceptuid { get; set; }
    //}
}
