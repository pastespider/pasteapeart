using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace PasteApeart.EntityFrameworkCore.auditing
{

    /// <summary>
    /// 
    /// </summary>
    public class AuditMetadataCache : IDisposable
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _expiration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expiration"></param>
        public AuditMetadataCache(TimeSpan? expiration = null)
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _expiration = expiration ?? TimeSpan.FromHours(1); // 默认1小时过期
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        // 检查类是否禁用审计
        public bool IsAuditDisabledOnType(Type type)
        {
            var key = $"Class_{type.FullName}";

            return _cache.GetOrCreate(key, entry =>
            {
                entry.SlidingExpiration = _expiration;
                return type.GetCustomAttribute<DisableAuditingAttribute>() != null;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool IsAuditDisabledOnProperty(PropertyInfo property)
        {
            var key = $"Property_{property.DeclaringType?.FullName}_{property.Name}";

            return _cache.GetOrCreate(key, entry =>
            {
                entry.SlidingExpiration = _expiration;
                return property.GetCustomAttribute<DisableAuditingAttribute>() != null;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void ClearType(Type type)
        {
            _cache.Remove($"Class_{type.FullName}");

            // 清除该类型的所有属性缓存
            foreach (var property in type.GetProperties())
            {
                _cache.Remove($"Property_{type.FullName}_{property.Name}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearAll()
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0); // 清除所有
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _cache?.Dispose();
        }
    }
}
