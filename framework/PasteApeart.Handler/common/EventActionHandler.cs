using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler.Extensions;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace PasteApeart.Handler
{
    /// <summary>
    /// 队列处理 可以被RabbitMQ或者Channel的消费者调用
    /// </summary>
    public class EventActionHandler : IEventActionHandler
    {

        private readonly ILogger<EventActionHandler> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IChannelHelper _channelHelper;
        private readonly Type[] _dtoTypeArray = null;

        /// <summary>
        /// 队列处理 可以被RabbitMQ或者Channel的消费者调用
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="channelHelper"></param>
        /// <param name="dtoTypes"></param>
        public EventActionHandler(ILogger<EventActionHandler> logger, IServiceProvider serviceProvider, IChannelHelper channelHelper, Type[] dtoTypes = null)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _channelHelper = channelHelper;
            if (dtoTypes != null)
            {
                _dtoTypeArray = dtoTypes;
            }
        }


        private readonly ConcurrentDictionary<string, Type> _eventTypeCache = new();

        private readonly ConcurrentDictionary<Type, (Type HandlerType, MethodInfo HandleMethod)> _handlerCache = new();

        /// <summary>
        /// 主要适用于分布式队列 比如RabitMQ等
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task ActionMessageAsync(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            if (_dtoTypeArray?.Any() != true)
            {
                _logger.LogError("ActionMessageAsync 的string模式下，构造函数的dtoTypes是必传的，表示EventModel所在程序集的类型集合");
                return;
            }

            await ActionReturnResultAsync(msg, _dtoTypeArray);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public async Task ActionMessageAsync(string msg, Type[] types)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }
            await ActionReturnResultAsync(msg, types);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public async Task<object> ActionResultAsync(string msg, string methodName = "HandleEventAsync")
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return null;
            }

            if (_dtoTypeArray?.Any() != true)
            {
                _logger.LogError("ActionMessageAsync 的string模式下，构造函数的dtoTypes是必传的，表示EventModel所在程序集的类型集合");
                return null;
            }

            return await ActionReturnResultAsync(msg, _dtoTypeArray, methodName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="types"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public async Task<object> ActionResultAsync(string msg, Type[] types, string methodName = "HandleEventAsync")
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return null;
            }
            return await ActionReturnResultAsync(msg, types, methodName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        private async Task ActionAsync(string msg, Type[] types)
        {
            object input = null;
            object handler = null;
            try
            {
                // 1. 只反序列化一次，使用 System.Text.Json 性能更好
                using var jsonDoc = JsonDocument.Parse(msg);
                var root = jsonDoc.RootElement;
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
                    return types.FirstOrDefault(x => x.Name == name);
                });
                if (modelType == null)
                {
                    _logger.LogError($"未找到事件类型: {eventName}");
                    return;
                }
                // 3. 反序列化为具体类型
                input = JsonSerializer.Deserialize(msg, modelType, JsonConfig.Options);
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
                        await _channelHelper.WriteAutoQueueAsync(tryModel);
                        return;
                    }
                }
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
                    catch (Exception exlDisposable)
                    {
                        _logger.LogException(exlDisposable);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="types"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private async Task<object> ActionReturnResultAsync(string msg, Type[] types, string methodName = "HandleEventAsync")
        {
            object input = null;
            object handler = null;
            try
            {
                // 1. 只反序列化一次，使用 System.Text.Json 性能更好
                using var jsonDoc = JsonDocument.Parse(msg);
                var root = jsonDoc.RootElement;
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
                    return null;
                }
                // 2. 获取事件类型（带缓存）
                var modelType = _eventTypeCache.GetOrAdd(eventName, name =>
                {
                    return types.FirstOrDefault(x => x.Name == name);
                });
                if (modelType == null)
                {
                    _logger.LogError($"未找到事件类型: {eventName}");
                    return null;
                }
                // 3. 反序列化为具体类型
                input = JsonSerializer.Deserialize(msg, modelType, JsonConfig.Options);
                if (input == null)
                {
                    _logger.LogError($"无法将消息反序列化为类型 {eventName}: {msg}");
                    return null;
                }
                using var scope = _serviceProvider.CreateScope();
                if (input is IEventModel tryModel)
                {
                    if (tryModel.ExpireSecond > 0)
                    {
                        await _channelHelper.WriteAutoQueueAsync(tryModel);
                        return null;
                    }
                }
                // 4. 获取处理器信息（带缓存）
                var (handlerType, handleMethod) = _handlerCache.GetOrAdd(modelType, type =>
                {
                    var hType = typeof(IEventHandler<>).MakeGenericType(type);
                    var method = hType.GetMethod(methodName);
                    return (hType, method);
                });
                if (handleMethod == null)
                {
                    _logger.LogError($"处理器 {handlerType.Name} 没有 {methodName} 方法");
                    return null;
                }
                handler = scope.ServiceProvider.GetRequiredService(handlerType);
                if (handler == null)
                {
                    _logger.LogError($"没有找到 {eventName} 的 IEventHandler 实现");
                    return null;
                }
                try
                {
                    // 5. 执行处理
                    var task = handleMethod.Invoke(handler, new[] { input }) as Task;
                    if (task != null)
                    {
                        await task;
                        //var type = task.GetType();
                        //if (type.IsGenericType)
                        //{
                        //    return type.GetProperty("Result")?.GetValue(task);
                        //}
                        //return null;
                        // 从 handler 实例上读 Result 属性（而不是从 Task<T> 里读）
                        var resultProp = handlerType.GetProperty("Result");
                        return resultProp?.GetValue(handler);
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
                    catch (Exception exlDisposable)
                    {
                        _logger.LogException(exlDisposable);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            return null;
        }


        private readonly ConcurrentDictionary<Type, Type> _eventHandlerCache = new();
        private readonly ConcurrentDictionary<Type, MethodInfo> _handleMethodCache = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        [Obsolete("Use ActionMessageAsync")]
        public async Task HanlderEventMessage(IEventModel read)
        {
            await ActionMessageAsync(read);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        [Obsolete("Use ActionMessageAsync")]
        public async Task HanlderEventMessage(string read)
        {
            await ActionMessageAsync(read);
        }

        /// <summary>
        /// 主要适用于本地Chennel模式
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        public async Task ActionMessageAsync(IEventModel read)
        {
            _logger.LogInformation($"Event Msg:{read.EventName}");
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
            //var handleMethod = _handleMethodCache.GetOrAdd(eventType, t =>
            //    handlerType.GetMethod("HandleEventAsync"));
            var handleMethod = _handleMethodCache.GetOrAdd(handlerType, t => t.GetMethod("HandleEventAsync"));
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
                catch (Exception exl)
                {
                    _logger.LogException(exl);
                }
            }
        }

    }
}
