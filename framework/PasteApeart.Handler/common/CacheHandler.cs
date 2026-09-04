using Microsoft.EntityFrameworkCore;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler.redis;

namespace PasteApeart.Handler
{
    /// <summary>
    /// 数据对象的缓存帮助类 注意当前是IScopedDependency周期
    /// </summary>
    public class CacheHandler : ICacheHandler
    {
        private readonly IAppCache _appCache;
        private readonly DbContext _dbContext;

        /// <summary>
        /// 数据对象的缓存帮助类 注意当前是IScopedDependency周期
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="cache"></param>
        /// <param name="map"></param>
        public CacheHandler(DbContext serviceProvider, IAppCache cache)
        {
            _dbContext = serviceProvider;
            _appCache = cache;
        }

        #region 通用缓存帮助

        /// <summary>
        /// 通用缓存获取
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="id"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        public async Task<TDto> DtoAsync<TEntity, TDto>(object id, int expireSeconds = 3600) where TEntity : class, IEntity
        {
            var cachePrefix = GetCachePrefix<TEntity>();
            var cacheKey = $"{cachePrefix}{id}";
            var read = await _appCache.GetObjectAsync<TDto>(cacheKey);
            if (read != null) { return read; }

            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                //var dto = _objectMapper.Map<TEntity, TDto>(entity);
                var dto = Mapster.TypeAdapter.Adapt<TEntity, TDto>(entity);
                await _appCache.SetObjectAsync<TDto>(cacheKey, dto, expireSeconds);
                return dto;
            }
            return default;
        }

        /// <summary>
        /// 通用缓存获取 没有则会抛出异常
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="id"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public async Task<TDto> DtoAsyncThrowException<TEntity, TDto>(object id, int expireSeconds = 3600) where TEntity : class, IEntity
        {
            var read = await DtoAsync<TEntity, TDto>(id, expireSeconds);
            if (read != null)
            {
                return read;
            }
            else
            {
                throw new PasteCodeException($"没有获取到对象:{id.ToString()}");
            }
        }

        /// <summary>
        /// 通用缓存删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> RemoveEntityAsync<TEntity>(object id)
        {
            var cachePrefix = GetCachePrefix<TEntity>();
            var cacheKey = $"{cachePrefix}{id}";
            return await _appCache.DeleteAsync(cacheKey);
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public string GetCachePrefix<TEntity>()
        {
            return $"cache:item:{typeof(TEntity).Name.ToLower()}:";
        }

        #endregion

    }
}
