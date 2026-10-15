using PasteApeart.Application.Contracts;
using PasteApeart.Handler.redis;
using Serilog;

namespace PasteApeart.Handler
{
    /// <summary>
    /// 事件模型的扩展
    /// </summary>
    public static class AbsEventModelExtensions
    {

        /// <summary>
        /// 启动 任务排重标记，修改MutexVal
        /// </summary>
        /// <param name="info"></param>
        /// <param name="appCache"></param>
        /// <param name="id"></param>
        /// <param name="expireSecond"></param>
        /// <returns></returns>
        public static async Task ToStartTaskMutexAsync(this AbsEventModel info, IAppCache appCache, object id = null, int expireSecond = 300)
        {
            if (string.IsNullOrEmpty(info.MutexKey))
            {
                info.MutexKey = $"mutex:{info.GetType().Name}:{(id?.ToString() ?? "0")}";
            }
            else
            {
                info.MutexKey = info.MutexKey.Replace(":id", $":{(id?.ToString() ?? "0")}");
            }
            info.MutexVal = Guid.NewGuid().ToString("N");
            await appCache.SetAsync(info.MutexKey, info.MutexVal, (int)info.ExpireSecond + expireSecond);
        }

        /// <summary>
        /// 判断是否有同样的任务，如果有则返回false表示不能启动
        /// </summary>
        /// <param name="info"></param>
        /// <param name="appCache"></param>
        /// <param name="id"></param>
        /// <param name="expireSecond"></param>
        /// <returns>是否可以启动 false表示正在执行中</returns>
        public static async Task<bool> ToAutoStartTaskMutexAsync(this AbsEventModel info, IAppCache appCache, object id = null, int expireSecond = 300)
        {
            if (string.IsNullOrEmpty(info.MutexKey))
            {
                info.MutexKey = $"mutex:{info.GetType().Name}:{(id?.ToString() ?? "0")}";
            }
            else
            {
                info.MutexKey = info.MutexKey.Replace(":id", $":{(id?.ToString() ?? "0")}");
            }
            if (!await appCache.ExistsAsync(info.MutexKey))
            {
                info.MutexVal = Guid.NewGuid().ToString("N");
                await appCache.SetAsync(info.MutexKey, info.MutexVal, (int)info.ExpireSecond + expireSecond);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查任务是否被重置 返回true表示可以执行 返回false表示不能执行
        /// </summary>
        /// <param name="info"></param>
        /// <param name="appCache"></param>
        /// <returns>true:可以执行，匹配 false:任务被重置，不能执行</returns>
        public static async Task<bool> ToMatchTaskMutexAsync(this AbsEventModel info, IAppCache appCache)
        {
            if (string.IsNullOrEmpty(info.MutexVal))
            {
                //当前不参与互斥判定
                return true;
            }
            if (!string.IsNullOrEmpty(info.MutexKey))
            {
                var readVal = await appCache.GetAsync(info.MutexKey);
                if (string.IsNullOrEmpty(readVal))
                {
                    return true;
                }
                if (readVal != info.MutexVal)
                {
                    Log.Logger.Warning($"任务互斥 Model:{info.GetType().Name} Key:{info.MutexKey} Val:{info.MutexVal} != Read:{readVal}");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="info"></param>
        /// <param name="appCache"></param>
        /// <returns></returns>
        public static async Task ToCleanTaskMutexAsync(this AbsEventModel info, IAppCache appCache)
        {
            if (!string.IsNullOrEmpty(info.MutexKey))
            {
                await appCache.DeleteAsync(info.MutexKey);
            }
        }

        /// <summary>
        /// 任务是否标记 注意要使用这个的时候，任务结束的时候需要调用 ToCleanTaskMutexAsync
        /// 需要之前有对MutexKey进行赋值 也就是ToAutoStartTaskMutexAsync/ToStartTaskMutexAsync
        /// </summary>
        /// <param name="info"></param>
        /// <param name="appCache"></param>
        /// <returns>true 任务在进行 false:没有任务 或者任务已经结束</returns>
        public static async Task<bool> ToExistsTaskMutexAsync(this AbsEventModel info, IAppCache appCache)
        {
            if (!string.IsNullOrEmpty(info.MutexKey))
            {
                return await appCache.ExistsAsync(info.MutexKey);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="_channelHelper"></param>
        /// <returns></returns>
        public static async Task<bool> ToAutoExpireAndSendAsync(this AbsEventModel input, IChannelHelper _channelHelper)
        {
            input.RetryTime++;
            if (input.MaxRetryTime <= 0)
            {
                return false;
            }
            if (input.RetryTime > input.MaxRetryTime)
            {
                return false;
            }
            //计算下一次时间 默认基于5秒的倍率延迟
            var baseRate = 5;
            if (!string.IsNullOrEmpty(input.RetryModel))
            {
                //基于业务自定义模型
            }
            input.ExpireSecond = baseRate * input.RetryTime;
            //可以设定最大时间等
            await _channelHelper.WriteAutoQueueAsync(input);
            return true;
        }



    }
}
