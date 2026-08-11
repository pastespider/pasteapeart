using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PasteApeart.auditmodels;
using PasteApeart.EntityFrameworkCore.auditing;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PasteApeart.EntityFrameworkCore
{



    /// <summary>
    /// 
    /// </summary>
    public abstract class CollectAuditDb<T> : DbContext, ICollectAuditDb where T : DbContext
    {
        private IHttpContextAccessor shttpContextAccessor = null;
        private AuditingConfigOptions sconfig = null;
        private AuditMetadataCache sauditMetadataCache = null;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public CollectAuditDb(DbContextOptions<T> options) : base(options)
        {
            //_config = this.GetService<IOptions<AuditingConfigOptions>>().Value;
            //_httpContextAccessor = this.GetService<IHttpContextAccessor>();
            //_auditMetadataCache = this.GetService<AuditMetadataCache>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected AuditingConfigOptions _config
        {
            get
            {
                if (sconfig == null)
                {
                    sconfig = this.GetService<IOptions<AuditingConfigOptions>>()?.Value;
                }
                return sconfig;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected IHttpContextAccessor _httpContextAccessor
        {
            get
            {
                if (shttpContextAccessor == null)
                {
                    shttpContextAccessor = this.GetService<IHttpContextAccessor>();
                }
                return shttpContextAccessor;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected AuditMetadataCache _auditMetadataCache
        {
            get
            {
                if (sauditMetadataCache == null)
                {
                    sauditMetadataCache = this.GetService<AuditMetadataCache>();
                }
                return sauditMetadataCache;
            }
        }

        private static readonly ConcurrentDictionary<Type, bool> _classAuditCache = new();
        private static readonly ConcurrentDictionary<PropertyInfo, bool> _propertyAuditCache = new();

        /// <summary>
        /// 这里后面高单例 延迟清理... .. .
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsAuditDisabledOnType(Type type)
        {
            if (_auditMetadataCache != null)
            {
                return _auditMetadataCache.IsAuditDisabledOnType(type);
            }
            return _classAuditCache.GetOrAdd(type, t =>
                t.GetCustomAttribute<DisableAuditingAttribute>() != null);
        }

        /// <summary>
        /// 这里后面高单例 延迟清理... .. .
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private bool IsAuditDisabledOnProperty(PropertyInfo property)
        {
            if (_auditMetadataCache != null)
            {
                return _auditMetadataCache.IsAuditDisabledOnProperty(property);
            }
            return _propertyAuditCache.GetOrAdd(property, p =>
                p.GetCustomAttribute<DisableAuditingAttribute>() != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<ApEntityChangeInfo> GetChanges()
        {

            //Console.WriteLine("Begin GetChanges");
            var changes = new List<ApEntityChangeInfo>();
            // 获取所有被跟踪的实体
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                var entityType = entry.Entity.GetType();
                if (IsAuditDisabledOnType(entityType))
                {
                    continue;
                }
                //var disableAuditOnClass = entityType.GetCustomAttribute<DisableAuditingAttribute>();
                //if (disableAuditOnClass != null)
                //{
                //continue; // 跳过整个实体的审计
                //}

                var entityChange = new ApEntityChangeInfo
                {
                    EntityType = entry.Entity.GetType().Name,
                    EntityId = GetEntityId(entry),
                    ChangeType = (int)entry.State,
                    ChangeTime = DateTime.UtcNow,
                    PropertyChanges = new List<ApPropertyChangeInfo>()
                };

                switch (entry.State)
                {
                    case EntityState.Added:
                        // 新增：收集所有当前值
                        foreach (var property in entry.Properties)
                        {
                            if (property.Metadata.IsPrimaryKey()) continue;

                            var propertyInfo = entityType.GetProperty(property.Metadata.Name);
                            if (propertyInfo != null && IsAuditDisabledOnProperty(propertyInfo))
                                continue;

                            entityChange.PropertyChanges.Add(new ApPropertyChangeInfo
                            {
                                PropertyName = property.Metadata.Name,
                                OriginalValue = null,
                                NewValue = FormatValue(property.CurrentValue),
                                ValueType = property.Metadata.ClrType.Name
                            });
                        }
                        break;

                    case EntityState.Modified:
                        // 修改：只收集实际变化的属性
                        foreach (var property in entry.Properties)
                        {
                            if (property.IsModified)
                            {
                                var propertyInfo = entityType.GetProperty(property.Metadata.Name);
                                if (propertyInfo != null && IsAuditDisabledOnProperty(propertyInfo))
                                    continue;

                                entityChange.PropertyChanges.Add(new ApPropertyChangeInfo
                                {
                                    PropertyName = property.Metadata.Name,
                                    OriginalValue = FormatValue(property.OriginalValue),
                                    NewValue = FormatValue(property.CurrentValue),
                                    ValueType = property.Metadata.ClrType.Name
                                });
                            }
                        }
                        break;

                    case EntityState.Deleted:
                        // 删除：记录被删除的实体信息
                        foreach (var property in entry.Properties)
                        {
                            if (property.Metadata.IsPrimaryKey()) continue;

                            var propertyInfo = entityType.GetProperty(property.Metadata.Name);
                            if (propertyInfo != null && IsAuditDisabledOnProperty(propertyInfo))
                                continue;

                            entityChange.PropertyChanges.Add(new ApPropertyChangeInfo
                            {
                                PropertyName = property.Metadata.Name,
                                OriginalValue = FormatValue(property.OriginalValue),
                                NewValue = null,
                                ValueType = property.Metadata.ClrType.Name
                            });
                        }
                        break;
                }

                // 只记录有变化的实体
                if (entityChange.PropertyChanges.Any())
                {
                    changes.Add(entityChange);
                }
            }
            return changes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private string GetEntityId(EntityEntry entry)
        {
            var keyProperties = entry.Metadata.FindPrimaryKey()?.Properties;
            if (keyProperties == null || !keyProperties.Any())
                return null;

            var keyValues = keyProperties.Select(p => entry.Property(p.Name).CurrentValue);
            return string.Join(",", keyValues);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string FormatValue(object value)
        {
            if (value == null) return null;

            // 处理复杂类型
            if (value is string || value is ValueType)
                return value.ToString();

            // 对于复杂对象，序列化为JSON
            return JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                WriteIndented = false,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private void AutoWrite()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                // 处理 UserId（你原来的逻辑）
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is IUserIdEntiry userEntity)
                    {
                        if (userEntity.UserId == 0)
                        {
                            //userEntity.UserId = _currentUser?.Id ?? 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AutoWrite();
            if (_config.Enable)
            {
                // 1. 收集变更
                var changes = GetChanges();
                // 2. 如果有变更且有HttpContext，存入审计收集器
                if (changes.Any() && _httpContextAccessor?.HttpContext != null)
                {
                    AutoSaveRangeAuditingEntityChanges(changes);
                }
            }
            // 3. 执行真正的保存
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            AutoWrite();
            if (_config.Enable)
            {
                var changes = GetChanges();
                if (changes.Any())
                {
                    AutoSaveRangeAuditingEntityChanges(changes);
                }
            }
            return base.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changes"></param>
        private void AutoSaveRangeAuditingEntityChanges(List<ApEntityChangeInfo> changes)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            if (httpContext.Items[_config.AuditItemKey] is AuditCollection collection)
            {
                collection.EntityChanges.AddRange(changes);
            }
            else
            {
                var coll = new AuditCollection();
                coll.EntityChanges = changes;// new List<EntityChangeInfo>() { change };
                httpContext.Items.TryAdd(_config.AuditItemKey, coll);
            }
        }

    }
}
