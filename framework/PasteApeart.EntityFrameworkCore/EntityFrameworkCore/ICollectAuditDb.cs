using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace PasteApeart.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICollectAuditDb
    {
        // ========== DbSet 相关 ==========

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        // ========== 添加操作（保持 object 类型）==========

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        EntityEntry Add(object entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void AddRange(params object[] entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void AddRange(IEnumerable<object> entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task AddRangeAsync(params object[] entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default);

        // ========== 更新操作（保持 object 类型）==========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        EntityEntry Update(object entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void UpdateRange(params object[] entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void UpdateRange(IEnumerable<object> entities);

        // ========== 删除操作（保持 object 类型）==========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        EntityEntry Remove(object entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void RemoveRange(params object[] entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void RemoveRange(IEnumerable<object> entities);

        // ========== Attach 操作（保持 object 类型）==========

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        EntityEntry Attach(object entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void AttachRange(params object[] entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void AttachRange(IEnumerable<object> entities);

        // ========== 查找操作 ==========

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        ValueTask<object?> FindAsync(Type entityType, params object?[]? keyValues);
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
#pragma warning disable CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。
        ValueTask<TEntity?> FindAsync<TEntity>(params object?[]? keyValues) where TEntity : class;
#pragma warning restore CS8632 // 只能在 "#nullable" 注释上下文内的代码中使用可为 null 的引用类型的注释。

        // ========== 条目操作 ==========

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        EntityEntry Entry(object entity);

        // ========== 保存操作 ==========

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        //Task<int> SaveChangesWithAuditAsync(CancellationToken cancellationToken = default);

        // ========== 其他属性 ==========

        /// <summary>
        /// 
        /// </summary>
        ChangeTracker ChangeTracker { get; }

        /// <summary>
        /// 
        /// </summary>
        DatabaseFacade Database { get; }
    }

}
