using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ExampleForm.Application.Contracts;
using PasteApeart.Helper;
using System.Reflection;
using Volo.Abp.Application.Services;
using PasteApeart.Application.Contracts;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ExampleFormAppService : ApplicationService
    {
        /// <summary>
        /// 
        /// </summary>
        protected IExampleFormDbContext _dbContext => LazyServiceProvider.LazyGetRequiredService<IExampleFormDbContext>();

        /// <summary>
        /// 
        /// </summary>
        protected CacheHandler _userHelper => LazyServiceProvider.LazyGetRequiredService<CacheHandler>();

        /// <summary>
        /// 
        /// </summary>
        protected ExampleFormAppService()
        {
            ObjectMapperContext = typeof(ExampleFormApplicationModule);
        }

        private HttpContext _context { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        protected HttpContext _httpContext
        {
            get
            {
                if (_context != null)
                {
                    return _context;
                }
                var _http = LazyServiceProvider.LazyGetRequiredService<IHttpContextAccessor>();
                _context = _http.HttpContext;
                return _context;
            }
        }

        /// <summary>
        /// 获取登录用户的ID
        /// </summary>
        /// <returns></returns>
        protected int ReadCurrentUserId()
        {
            var token = ReadCurrentUserToken();
            var userid = token.CurrentUserId();
            return userid;
        }

        /// <summary>
        /// 获取登录用户的ID
        /// </summary>
        /// <returns></returns>
        protected int ReadCurrentUserIdThrwoException()
        {
            var userid = ReadCurrentUserId();
            if (userid <= 0)
            {
                throw new PasteCodeException("密钥信息有误，请重新登陆", 401);
            }
            return userid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected int ReadCurrentAdminId()
        {
            return ReadCurrentUserId();
        }

        /// <summary>
        /// 获取当前用户的密钥
        /// </summary>
        /// <returns></returns>
        protected string ReadCurrentUserToken()
        {
            if (_httpContext.Request.Headers.TryGetValue(PublicString.TokenHeadName, out var heads))
            {
                var token = heads.FirstOrDefault();
                if (!String.IsNullOrEmpty(token))
                {
                    if (token.TokenCheck())
                    {
                        return token;
                    }
                }
            }
            if (_httpContext.Request.Cookies.TryGetValue(PublicString.TokenHeadName, out var cookie))
            {
                if (!String.IsNullOrEmpty(cookie))
                {
                    if (cookie.TokenCheck())
                    {
                        return cookie.ToString();
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 是否是超级权限
        /// </summary>
        /// <returns></returns>
        protected async Task<bool> IsRootAsync()
        {
            var token = this.ReadCurrentUserToken();
            //注意，这里要进一步解析，下面是省略的写法
            var isRoot = token.Contains("_root_");

            //未完待续
            return await Task.FromResult(isRoot);
        }

        /// <summary>
        /// 特殊的过滤 在UI中隐藏某些字段
        /// </summary>
        /// <param name="action"></param>
        /// <param name="dto"></param>
        /// <param name="dtoType"></param>
        /// <param name="listModel">list模式，特别针对枚举类型</param>
        /// <returns></returns>
        protected async Task<VoloModelInfo> DefaultBuildPropertyInfo(string action, object dto, Type dtoType, bool listModel = false)
        {
            if (!listModel)
            {
                if (dtoType.Name.EndsWith("ListDto"))
                {
                    listModel = true;
                }
            }
            var _model = BuilderHelper.AttributeReadModelProperty(dto, dtoType, listModel);
            //if (action == "add" || action == "update" || action == "detail")
            //{
            //    var isroot =await this.IsRootAsync();
            //    if (!isroot)
            //    {
            //        //if (dtoType.GetInterfaces().Contains(typeof(ISiteEntity)))
            //        //{
            //        var binds = _model.Properties.Where(x => x.Name == "siteId" || x.Name == "extendSite").ToList();
            //        if (binds?.Any() == true)
            //        {
            //            foreach (var pro in binds)
            //            {
            //                if (pro.Attributes == null)
            //                {
            //                    pro.Attributes = new System.Collections.Generic.List<VoloModelAttribute>();
            //                }
            //                if (pro.Attributes.Any(x => x.Name == "hidden") == false)
            //                {
            //                    pro.Attributes.Add(new VoloModelAttribute { Name = "hidden" });
            //                }
            //            }
            //        }
            //        //}
            //    }
            //}
            //if (action == "list")
            //{
            //    //Logger.LogWarning($"{dtoType.FullName} Enter list");

            //    if (_model.Attributes == null || _model.Attributes?.Any(x => x.Name == "disable") == false)
            //    {
            //        var attrs = dtoType.GetCustomAttributes<PasteAuthAttribute>();
            //        if (attrs?.Any() == true)
            //        {
            //            var currentToken = this.ReadCurrentUserToken();
            //            var roles = await _userHelper.ReadRoleListAsync(currentToken);
            //            var volo = ExampleFormHelper.ExtendHelper.ToEnableAttribute(roles, attrs.ToList());
            //            //var volo = UserHandler.ToDisableAttribute(roles, attrs.ToList());
            //            if (volo != null)
            //            {
            //                //Logger.LogWarning($"{dtoType.Name} 获取到组合的Disable信息 {volo.Args1}-{volo.Args2}-{volo.Args3}-{volo.Args4}");
            //                if (_model.Attributes == null)
            //                {
            //                    _model.Attributes = new System.Collections.Generic.List<VoloModelAttribute> { volo };
            //                }
            //                else
            //                {
            //                    _model.Attributes.Add(volo);
            //                }
            //            }
            //            else
            //            {
            //                Logger.LogWarning($"{dtoType.Name} 没有从配置和权限列表中读取到Disable的Attribute信息!");
            //            }
            //        }

            //    }

            //}

            //return _model;
            return await Task.FromResult(_model);
        }


        /// <summary>
        /// 校验是否有执行权限 使用PasteDisableAttribute和PasteAuthAttribute进行标记
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected async Task DefaultAuthDataFilterAsync(Type entityType, object obj, string action)
        {
            var attrs = entityType.GetCustomAttributes<PasteAuthAttribute>();
            if (attrs?.Any() == true)
            {
                var currentToken = this.ReadCurrentUserToken();
                //var roles = await _smartHelper.ReadRoleAuthListAsync(currentToken);
                var roles = await _userHelper.ReadRoleListAsync(currentToken);
                var hasAuth = ExtendHelper.HasRoleFromPasteAuthConfigAttribute(action, roles, attrs.ToList());
                if (!hasAuth.auth)
                {
                    throw new PasteCodeException(hasAuth.msg, 403);
                }
            }
        }

        /// <summary>
        /// 进行默认值的写入，注意要判断当前是否有值
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected async Task DefaultSetDataExtendAsync(Type entityType, object obj, string action)
        {
            foreach (var item in entityType.GetProperties())
            {
                if (!item.CanWrite)
                {
                    continue;
                }
                var value = item.GetValue(obj);
                if (value != null &&                  // 值不为 null
                    ((value is long longValue && longValue != 0L) ||  // 是 long 且不为 0
                     (value is int intValue && intValue != 0)))       // 是 int 且不为 0
                {
                    continue;  // 有值则跳过
                }
                //进行判断写入
                switch (action)
                {
                    case "add":
                        {
                            switch (item.Name)
                            {
                                case "UserId":
                                case "CreateUserId":
                                    {
                                        var userid = this.ReadCurrentUserId();
                                        item.SetValue(obj, userid);
                                    }
                                    break;
                            }
                        }
                        break;
                    case "update":
                        {
                            switch (item.Name)
                            {
                                case "UpdateDate":
                                case "UpdateTime":
                                    {
                                        var now = DateTime.Now;
                                        if (item.PropertyType == typeof(DateTime) || item.PropertyType == typeof(DateTime?))
                                        {
                                            item.SetValue(obj, now);
                                        }
                                        if (item.PropertyType == typeof(long))
                                        {
                                            var nowtime = now.ToUnixTimeSeconds();
                                            item.SetValue(obj, now);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }

            }
            await Task.CompletedTask;
        }

    }
}
