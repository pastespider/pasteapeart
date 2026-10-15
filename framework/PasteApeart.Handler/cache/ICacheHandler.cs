
namespace PasteApeart.Handler.cache
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheHandler
    {
        /// <summary>
        /// 读取缓存，不抛出异常
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="id"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        Task<TDto> DtoAsync<TEntity, TDto>(object id, int expireSeconds = 3600) where TEntity : class;

        /// <summary>
        /// 读取缓存 不存在则会抛出异常
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="id"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        Task<TDto> DtoAsyncThrowException<TEntity, TDto>(object id, int expireSeconds = 3600) where TEntity : class;

        /// <summary>
        /// 移除某一个对象的缓存
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> RemoveEntityAsync<TEntity>(object id);

        /// <summary>
        /// 获取缓存的Key头
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        string GetCachePrefix<TEntity>();
    }
}