using PasteApeart.Application.Contracts;
using PasteApeart.EntityFrameworkCore;
using PasteApeart.Handler;
using PasteApeart.usermodels;

namespace PasteApeart
{
    /// <summary>
    /// 数据对象的缓存帮助类 注意当前是IScopedDependency周期
    /// </summary>
    public class CacheHandler : IScopedDependency
    {
        private readonly IAppCache _appCache;
        //private readonly IObjectMapper _objectMapper;
        private readonly IPasteApeartDbContext _dbContext;

        /// <summary>
        /// 数据对象的缓存帮助类 注意当前是IScopedDependency周期
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="cache"></param>
        /// <param name="map"></param>
        public CacheHandler(IPasteApeartDbContext serviceProvider, IAppCache cache)
        {
            _dbContext = serviceProvider;
            _appCache = cache;
            //_objectMapper = map;
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
        public async Task<TDto> DtoAsync<TEntity, TDto>(object id, int expireSeconds = 3600) where TEntity : Entity
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
        public async Task<TDto> DtoAsyncThrowException<TEntity, TDto>(object id, int expireSeconds = 3600) where TEntity : Entity
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
        private static string GetCachePrefix<TEntity>()
        {
            return $"cache:item:{typeof(TEntity).Name.ToLower()}:";
        }

        #endregion


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[Obsolete("Use DtoAsync")]
        //public async Task<UserInfoDto> ItemUserInfo(int id)
        //{
        //    var item = await _appCache.GetObjectAsync<UserInfoDto>($"{PublicString._cacheheaduser}{id}");
        //    if (item == null || item == default)
        //    {
        //        var find = _dbContext.UserInfo.Where(x => x.Id == id).AsNoTracking().FirstOrDefault();
        //        if (find != null && find != default)
        //        {
        //            item = _objectMapper.Map<UserInfo, UserInfoDto>(find);
        //            await _appCache.SetObjectAsync<UserInfoDto>($"{PublicString._cacheheaduser}{id}", item, 3600);
        //        }
        //    }
        //    return item;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[Obsolete("Use RemoveEntityAsync")]
        //public async Task<bool> RemoveUserItemCacheAsync(int id)
        //{
        //    return await _appCache.DeleteAsync($"{PublicString._cacheheaduser}{id}");
        //}


        ///// <summary>
        ///// 是否有权限
        ///// </summary>
        ///// <param name="_model"></param>
        ///// <param name="_role"></param>
        ///// <param name="_token"></param>
        ///// <returns></returns>
        //public (int code, bool role) HasRole(string _model, string _role, string _token = null)
        //{
        //    if (!string.IsNullOrEmpty(_token))
        //    {
        //        var roles = _appCache.GetObject<List<RoleAuthDto>>(_token);
        //        if (roles != null && roles.Any())
        //        {
        //            var root = roles.Where(x => (x.Model == "root" && x.Role == "root") || (x.Model == _model && x.Role == _role)).Any();
        //            return (403, root);
        //        }
        //        else
        //        {
        //            return (401, false);
        //        }
        //    }
        //    return (401, false);
        //}

        ///// <summary>
        ///// 是否有权限 包含角色的判断，如果一个权限配置了角色的，那么满足角色或者权限的可以访问
        ///// </summary>
        ///// <param name="_model"></param>
        ///// <param name="_role"></param>
        ///// <param name="_token"></param>
        ///// <returns></returns>
        //public (int code, bool role) NewHasRole(string _model, string _role, string _grade, string _token = null)
        //{
        //    if (!string.IsNullOrEmpty(_token))
        //    {
        //        if (!String.IsNullOrEmpty(_grade))
        //        {
        //            if (_token.Contains($"{_grade}"))
        //            {
        //                return (200, true);
        //            }
        //        }
        //        var roles = _appCache.GetObject<List<RoleAuthDto>>(_token);
        //        if (roles != null && roles.Any())
        //        {
        //            var root = roles.Where(x => (x.Model == "root" && x.Role == "root") || (x.Model == _model && x.Role == "root") || (x.Model == _model && x.Role == _role)).Any();
        //            return (403, root);
        //        }
        //        else
        //        {
        //            return (401, false);
        //        }
        //    }
        //    return (401, false);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_model"></param>
        /// <param name="_role"></param>
        /// <param name="_grade"></param>
        /// <param name="_token"></param>
        /// <returns></returns>
        public async Task<(int code, bool role)> HasRoleAsync(string _model, string _role, string _grade, string _token = null)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                if (!String.IsNullOrEmpty(_grade))
                {
                    if (_token.Contains($"{_grade}"))
                    {
                        return (200, true);
                    }
                }
                var roles = await ReadRoleListAsync(_token);
                if (roles != null && roles.Any())
                {
                    var has_auth = roles.Where(x => (x.Model == "root" && x.Role == "root") || (x.Model == _model && x.Role == "root") || (x.Model == _model && x.Role == _role)).Any();
                    return (403, has_auth);
                }
                else
                {
                    return (401, false);
                }
            }
            return (401, false);
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<RoleAuthDto>> ReadRoleListAsync(string token)
        {
            var roles = await _appCache.GetObjectAsync<List<RoleAuthDto>>(token);
            if (roles != null && roles.Any())
            {
                return roles;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_model"></param>
        /// <param name="_role"></param>
        /// <param name="_grade"></param>
        /// <param name="_token"></param>
        /// <returns></returns>
        public async Task<bool> HasAuthAsync(string _model, string _role, string _grade, string _token = null)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                if (!String.IsNullOrEmpty(_grade))
                {
                    if (_token.Contains($"{_grade}"))
                    {
                        return true;
                    }
                }
                var roles = await ReadRoleListAsync(_token.CurrentTokenCacheKey());
                if (roles != null && roles.Any())
                {
                    var has_auth = roles.Where(x => (x.Model == "root" && x.Role == "root") || (x.Model == _model && x.Role == "root") || (x.Model == _model && x.Role == _role)).Any();
                    return has_auth;
                }
            }
            return false;
        }

    }
}
