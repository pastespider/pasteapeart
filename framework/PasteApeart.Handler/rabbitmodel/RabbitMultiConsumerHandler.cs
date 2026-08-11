using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler.eventmodels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PasteApeart.Handler.rabbitmodel
{
    /// <summary>
    /// 多Channel的订阅者模式 - 优化版
    /// </summary>
    public class RabbitMultiConsumerHandler : ISingletonDependency, IAsyncDisposable
    {
        private readonly RabbitConfig _rabbitConfig;
        private readonly ILogger<RabbitMultiConsumerHandler> _logger;
        private IConnection _conn;
        private readonly List<IChannel> _channels = new();
        private readonly int _channelCount = 2; // 根据CPU核心数调整
        //private readonly SemaphoreSlim _channelSemaphore;
        private IChannel _defaultChannel; // 用于声明的默认channel
        private readonly IServiceProvider _serviceProvider;
        private readonly ChannelHelper _channelHelper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="rabbitConfig"></param>
        public RabbitMultiConsumerHandler(
            IServiceProvider serviceProvider,
            ILogger<RabbitMultiConsumerHandler> logger,
            ChannelHelper channelHelper,
            IOptions<RabbitConfig> rabbitConfig)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _channelHelper = channelHelper;
            _rabbitConfig = rabbitConfig.Value;
            _channelCount = _rabbitConfig.ReadChannelCount;
            //_channelSemaphore = new SemaphoreSlim(_channelCount, _channelCount);

            _channelCompletionSources = new List<TaskCompletionSource<bool>>();
        }

        /// <summary>
        /// 多Channel批量初始化
        /// </summary>
        public async Task InitAsync()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                // 创建连接工厂
                var connFactory = new ConnectionFactory
                {
                    HostName = _rabbitConfig.HostName,
                    Port = _rabbitConfig.Port,
                    UserName = _rabbitConfig.UserName,
                    Password = _rabbitConfig.Password,
                    VirtualHost = _rabbitConfig.VirtualHost,
                    ClientProvidedName = _rabbitConfig.ClientName,
                    //DispatchConsumersAsync = true // 启用异步消费者
                };

                // 创建单个连接
                _conn = await connFactory.CreateConnectionAsync();

                // 创建用于声明的默认channel
                _defaultChannel = await _conn.CreateChannelAsync();

                // 声明交换机和队列
                await DeclareExchangesAndQueues();

                // 创建多个消费channel
                await CreateConsumerChannels();

                stopwatch.Stop();
                _logger.LogWarning($"Multi-Channel Consumer Initialized. Duration: {stopwatch.Elapsed.TotalMilliseconds}ms");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ multi-channel initialization failed!");
            }
        }

        /// <summary>
        /// 声明交换机和队列（使用默认channel）
        /// </summary>
        private async Task DeclareExchangesAndQueues()
        {
            var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", _rabbitConfig.ExpiredExchange },
            { "x-dead-letter-routing-key", _rabbitConfig.ExpireRouteKey }
        };

            await BindExpireListAsync(arguments);

            if (arguments.ContainsKey("x-message-ttl"))
            {
                arguments.Remove("x-message-ttl");
            }

            // 声明交换机
            await _defaultChannel.ExchangeDeclareAsync(
                exchange: _rabbitConfig.ExpiredExchange,
                type: _rabbitConfig.ExpireExchangeType,
                durable: _rabbitConfig.Durable);

            // 声明死信队列
            await _defaultChannel.QueueDeclareAsync(
                queue: _rabbitConfig.ExpiredWatchQueue,
                durable: _rabbitConfig.Durable,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // 绑定队列
            await _defaultChannel.QueueBindAsync(
                queue: _rabbitConfig.ExpiredWatchQueue,
                exchange: _rabbitConfig.ExpiredExchange,
                routingKey: _rabbitConfig.ExpireRouteKey,
                arguments: null);

            #region 以下是绑定到队列去的 处理订阅广播模式
            if (_rabbitConfig.SubscribeEnable)
            {
                if (string.IsNullOrEmpty(_rabbitConfig.SubscribeQueue))
                {
                    _logger.LogError("广播队列配置错误，没有消费队列名称，不需要消费广播信息");
                    return;
                }

                // 声明交换机
                await _defaultChannel.ExchangeDeclareAsync(
                    exchange: _rabbitConfig.SubscribeExchange,
                    type: "fanout",
                    durable: true);

                // 声明当前实例的专属队列
                await _defaultChannel.QueueDeclareAsync(
                    queue: _rabbitConfig.SubscribeQueue, // 使用专属队列名
                    durable: false, // 广播队列通常不需要持久化，实例挂了队列就可以销毁
                    exclusive: true, // 排他性队列，连接断开后自动删除
                    autoDelete: true,
                    arguments: null);

                // 绑定到广播交换机（Fanout 不需要 routingKey）
                await _defaultChannel.QueueBindAsync(
                    queue: _rabbitConfig.SubscribeQueue,
                    exchange: _rabbitConfig.SubscribeExchange,
                    routingKey: "");

                _logger.LogWarning("广播模式交换机配置完成");
            }
            #endregion

        }


        //对应的任务列表
        private readonly List<TaskCompletionSource<bool>> _channelCompletionSources;

        /// <summary>
        /// 创建多个消费channel
        /// </summary>
        private async Task CreateConsumerChannels()
        {

            for (int i = 0; i < _channelCount; i++)
            {
                var channelNumber = i;
                try
                {
                    var channel = await _conn.CreateChannelAsync();

                    // 每个channel单独设置QoS
                    await channel.BasicQosAsync(0, 1, false);

                    // ✅ 初始状态：已完成（空闲状态）
                    var tcs = new TaskCompletionSource<bool>();
                    tcs.TrySetResult(true);
                    _channelCompletionSources.Add(tcs);// = tcs;
                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.ReceivedAsync += async (sender, e) =>
                    {
                        await Consumer_Received(sender, e, channelNumber);
                    };

                    // 每个channel都消费同一个队列，实现负载均衡
                    await channel.BasicConsumeAsync(
                        queue: _rabbitConfig.ExpiredWatchQueue,
                        autoAck: false,
                        consumer: consumer);


                    _channels.Add(channel);

                    _logger.LogInformation($"Consumer Channel {channelNumber} started.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to create consumer channel {channelNumber}");
                }
            }
        }

        /// <summary>
        /// 消费者接收处理（带channel编号）
        /// </summary>
        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e, int channelNumber)
        {
            // ✅ 收到消息时重置为未完成
            _channelCompletionSources[channelNumber] = new TaskCompletionSource<bool>();
            var channel = ((AsyncEventingBasicConsumer)sender).Channel;
            var msg = string.Empty;
            var boolHandler = false;
            try
            {
                if (!cancellation.IsCancellationRequested)
                {
                    boolHandler = true;
                    // 获取信号量，控制并发
                    msg = Encoding.UTF8.GetString(e.Body.ToArray());
                    if (!string.IsNullOrEmpty(msg))
                    {
                        await HandlerEventMessage(msg);
                    }
                    // ✅ 处理完成
                    _channelCompletionSources[channelNumber].TrySetResult(true);
                }
                else
                {
                    return;//由于没有ACK，推送端会进入等待期
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Channel {channelNumber} processing failed. Message: {msg}");
                // 处理失败时 直接忽略 所以下面注释了
                // ✅ 异常也完成
                _channelCompletionSources[channelNumber].TrySetResult(false);
            }
            finally
            {
                if (boolHandler)
                {
                    // 确认消息 异常当成完成，所以下面代码生效
                    await channel.BasicAckAsync(e.DeliveryTag, false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private CancellationTokenSource cancellation { get; set; } = new();

        /// <summary>
        /// 优雅关闭
        /// </summary>
        public async Task ShutdownAsync()
        {
            try
            {
                _logger.LogInformation("Shutting down multi-channel consumer...");
                //cancellation.Cancel();
                if (cancellation != null)
                {
                    await cancellation.CancelAsync();
                    //cancellation.Dispose();
                }


                var tasks = _channelCompletionSources
                    .Where(tcs => !tcs.Task.IsCompleted)
                    .Select(tcs => tcs.Task);
                if (tasks.Any())
                {
                    try
                    {
                        using var cts = new CancellationTokenSource(6000);
                        await Task.WhenAll(tasks).WaitAsync(cts.Token);
                        _logger.LogInformation("All messages processed");
                    }
                    catch (TimeoutException)
                    {
                        _logger.LogWarning($"Shutdown timeout after 6 s");
                    }
                }
                else
                {
                    _logger.LogWarning($"没有在执行的任务，直接结束消费者程序！");
                }

                // 关闭所有消费channel
                foreach (var channel in _channels)
                {
                    if (channel.IsOpen)
                    {
                        await channel.CloseAsync();
                    }
                    channel.Dispose();
                }

                // 关闭默认channel
                if (_defaultChannel?.IsOpen == true)
                {
                    await _defaultChannel.CloseAsync();
                    _defaultChannel.Dispose();
                }

                // 关闭连接
                if (_conn?.IsOpen == true)
                {
                    await _conn.CloseAsync();
                    _conn.Dispose();
                }

                if (cancellation != null)
                {
                    cancellation.Dispose();
                }

                _channels.Clear();
            }catch(Exception exl)
            {
                _logger.LogException(exl);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicArgs"></param>
        /// <returns></returns>
        private async Task BindExpireListAsync(Dictionary<string, object> dicArgs)
        {
            #region 新版本，基于数据库模式
            //using var scope = _serviceProvider.CreateScope();
            //using var _dbContext = scope.ServiceProvider.GetRequiredService<IPasteApeartDbContext>();

            //var queueList = await _dbContext.QueueInfo.Where(x => x.IsEnable).AsNoTracking().ToListAsync();
            //if (queueList?.Any() == true)
            //{
            //    _rabbitConfig.ExpireTimeList = queueList.Select(x => new RabbitExcepireItem { ExpireExchange = x.Exchange, ExpireQueue = x.Queue, ExpireTime = x.ExpireSecond * 1000, ExpireSecond = x.ExpireSecond, Key = x.Name, StayTime = x.CurrentValue, Zindex = x.Sort }).ToList();
            //}
            //else
            //{
            //    if (_rabbitConfig.ExpireTimeList != null)
            //    {
            //        foreach (var item in _rabbitConfig.ExpireTimeList)
            //        {
            //            item.ExpireSecond = item.ExpireTime / 1000;
            //        }
            //    }
            //}
            if (_rabbitConfig.ExpireTimeList?.Count > 0)
            {
                foreach (var item in _rabbitConfig.ExpireTimeList)
                {
                    var tasks = new List<Task>();
                    var millSecond = item.ExpireSecond * 1000;
                    if (dicArgs.ContainsKey("x-message-ttl")) { dicArgs["x-message-ttl"] = millSecond; } else { dicArgs.Add("x-message-ttl", millSecond); }
                    tasks.Add(_defaultChannel.ExchangeDeclareAsync(exchange: item.ExpireExchange, type: _rabbitConfig.ExpireExchangeType, durable: _rabbitConfig.Durable));
                    tasks.Add(_defaultChannel.QueueDeclareAsync(queue: item.ExpireQueue, durable: _rabbitConfig.Durable, exclusive: false, autoDelete: false, arguments: dicArgs));
                    tasks.Add(_defaultChannel.QueueBindAsync(queue: item.ExpireQueue, exchange: item.ExpireExchange, routingKey: _rabbitConfig.ExpireRouteKey, arguments: null));
                    await Task.WhenAll(tasks);
                }
            }
            #endregion
        }


        private readonly ConcurrentDictionary<string, Type> _eventTypeCache = new();
        private readonly ConcurrentDictionary<Type, (Type HandlerType, MethodInfo HandleMethod)> _handlerCache = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task HandlerEventMessage(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }
            object input = null;
            try
            {
                // 1. 只反序列化一次，使用 System.Text.Json 性能更好
                using var jsonDoc = JsonDocument.Parse(msg);
                var root = jsonDoc.RootElement;
                //if (!root.TryGetProperty("EventName", out var eventNameProperty) ||
                //    eventNameProperty.GetString() is not string eventName)
                //{
                //    _logger.LogError($"消息缺少 EventName 字段: {msg}");
                //    return;
                //}
                JsonElement eventNameProperty;
                // 使用不区分大小写的匹配，更健壮
                if (!root.TryGetProperty("eventName", out eventNameProperty))
                {
                    root.TryGetProperty("EventName", out eventNameProperty);
                }
                // 获取字符串值并验证
                string eventName = eventNameProperty.GetString();
                if (string.IsNullOrEmpty(eventName))
                {
                    _logger.LogError($"EventName 字段值为空: {msg}");
                    return;
                }

                // 2. 获取事件类型（带缓存）
                var modelType = _eventTypeCache.GetOrAdd(eventName, name =>
                {
                    return typeof(AbsEventModel).Assembly.GetTypes()
                        .FirstOrDefault(t => t.Name == name);
                });
                //&& typeof(EventBaseModel).IsAssignableFrom(t)
                if (modelType == null)
                {
                    _logger.LogError($"未找到事件类型: {eventName}");
                    return;
                }
                // 3. 反序列化为具体类型
                input = JsonSerializer.Deserialize(msg,modelType,JsonConfig.Options);
                if (input == null)
                {
                    _logger.LogError($"无法将消息反序列化为类型 {eventName}: {msg}");
                    return;
                }
                using var scope = _serviceProvider.CreateScope();
                if (input is IEventModel tryModel)
                {
                    if (tryModel.ExpireSecond > 0)
                    {
                        //var _channelHandler = scope.ServiceProvider.GetRequiredService<ChannelHelper>();
                        await _channelHelper.WriteAutoQueueAsync(tryModel);
                        return;
                    }
                }

                _logger.LogWarning($"begin handler {eventName}");

                //EventTryHandler
                // 4. 获取处理器信息（带缓存）
                var (handlerType, handleMethod) = _handlerCache.GetOrAdd(modelType, type =>
                {
                    var hType = typeof(IEventHandler<>).MakeGenericType(type);
                    var method = hType.GetMethod("HandleEventAsync");
                    return (hType, method);
                });
                if (handleMethod == null)
                {
                    _logger.LogError($"处理器 {handlerType.Name} 没有 HandleEventAsync 方法");
                    return;
                }
                var handler = scope.ServiceProvider.GetRequiredService(handlerType);
                if (handler == null)
                {
                    _logger.LogError($"没有找到 {eventName} 的 IEventHandler 实现");
                    return;
                }
                // 5. 执行处理
                var task = handleMethod.Invoke(handler, new[] { input }) as Task;
                if (task != null)
                {
                    await task;
                }
                else
                {
                    _logger.LogError($"无法调用 {eventName} 的处理方法");
                }
                if (handler is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                if (handler is IAsyncDisposable disposable1)
                {
                    await disposable1.DisposeAsync();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理消息时发生错误: {msg}");
                //if (input != null)
                //{
                //    if (input is AbsEventModel emodel)
                //    {
                //        if (emodel.LimitTime > 0 && emodel.CurrentTime < emodel.LimitTime)
                //        {
                //            emodel.CurrentTime++;
                //            emodel.ExpireSecond = EventHandlerExtensions.NextSecond(emodel.CurrentTime);
                //            await _channelHelper.WriteAutoQueueAsync(emodel);
                //        }
                //    }
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            await ShutdownAsync();
        }
    }
}
