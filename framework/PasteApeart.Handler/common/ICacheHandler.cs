
namespace PasteApeart
{
    public interface ICacheHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="id"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        Task<TDto> DtoAsync<TEntity, TDto>(object id, int expireSeconds = 3600) where TEntity : class, IEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="id"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        Task<TDto> DtoAsyncThrowException<TEntity, TDto>(object id, int expireSeconds = 3600) where TEntity : class, IEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> RemoveEntityAsync<TEntity>(object id);
    }
}