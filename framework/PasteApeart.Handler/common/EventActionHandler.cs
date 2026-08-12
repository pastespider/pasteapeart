using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler.eventmodels;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace PasteApeart.Handler
{
    /// <summary>
    /// 队列处理 可以被RabbitMQ或者Channel的消费者调用
    /// </summary>
    public class EventActionHandler : ISingletonDependency, IEventActionHandler
    {

        private readonly ILogger<EventActionHandler> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IChannelHelper _channelHelper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="channelHelper"></param>
        public EventActionHandler(ILogger<EventActionHandler> logger, IServiceProvider serviceProvider, IChannelHelper channelHelper)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _channelHelper = channelHelper;
        }


        private readonly ConcurrentDictionary<string, Type> _eventTypeCache = new();
        private readonly ConcurrentDictionary<Type, (Type HandlerType, MethodInfo HandleMethod)> _handlerCache = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task HandlerEventMessage(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }
            object input = null;
            object handler = null;
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
                //var input = JsonSerializer.Deserialize(msg, modelType, new JsonSerializerOptions
                //{
                //    PropertyNameCaseInsensitive = true
                //});
                //input = JsonConvert.DeserializeObject(msg, modelType);
                input = JsonSerializer.Deserialize(msg, modelType, JsonConfig.Options);
                //var aaa = Newtonsoft.Json.JsonConvert.DeserializeObject(msg,);
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
                handler = scope.ServiceProvider.GetRequiredService(handlerType);
                if (handler == null)
                {
                    _logger.LogError($"没有找到 {eventName} 的 IEventHandler 实现");
                    return;
                }
                try
                {
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
                }
                catch (Exception handlerExl)
                {
                    _logger.LogException(handlerExl);
                }
                finally
                {
                    try
                    {
                        if (handler is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                        if (handler is IAsyncDisposable disposable1)
                        {
                            await disposable1.DisposeAsync();
                        }
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理消息时发生错误: {msg}");
                if (input != null)
                {
                    //if (input is EventRetryModel emodel)
                    //{
                    //    if (emodel.TryMaxTime > 0 && emodel.CurrentTime < emodel.TryMaxTime)
                    //    {
                    //        emodel.CurrentTime++;
                    //        emodel.ExpireSecond = EventHandlerExtensions.NextSecond(emodel.CurrentTime);
                    //        await _channelHelper.WriteAutoMQAsync(emodel);
                    //    }
                    //}
                }
            }
        }

        private readonly ConcurrentDictionary<Type, Type> _eventHandlerCache = new();
        private readonly ConcurrentDictionary<Type, MethodInfo> _handleMethodCache = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        public async Task HanlderEventMessage(IEventModel read)
        {
            _logger.LogWarning($"Event Msg:{read.EventName}");
            //read是一个IEventModel
            var typeName = read.GetType().Name;
            var eventType = read.GetType();
            using var _scope = _serviceProvider.CreateScope();
            var handlerType = _eventHandlerCache.GetOrAdd(eventType, t => typeof(IEventHandler<>).MakeGenericType(t));
            var handler = _scope.ServiceProvider.GetService(handlerType);
            // 通过依赖注入获取处理器实例
            if (handler == null)
            {
                _logger.LogError($"没有找到 {typeName} 的 IEventHandler 实现！");
                return;
            }
            // 获取处理方法（带缓存）
            var handleMethod = _handleMethodCache.GetOrAdd(eventType, t =>
                handlerType.GetMethod("HandleEventAsync"));
            if (handleMethod == null)
            {
                _logger.LogError($"无法找到 {typeName} 的 HandleEventAsync 方法");
                return;
            }
            try
            {
                var task = handleMethod.Invoke(handler, new[] { read }) as Task;
                if (task != null)
                {
                    await task;
                }
                //_logger.LogInformation($"执行了 {typeName} 的 HandleEventAsync 函数");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理事件 {eventType.Name} 时发生错误");
            }
            finally
            {
                try
                {
                    if (handler is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    if (handler is IAsyncDisposable disposable1)
                    {
                        await disposable1.DisposeAsync();
                    }
                }
                catch
                {

                }
            }
        }

    }
}
