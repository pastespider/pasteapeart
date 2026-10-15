using ExampleApeart.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using PasteApeart;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler.lazy;
using PasteApeart.Helper;
using System.Reflection;

namespace ExampleApeart
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ExampleApeartAppService : IAppService
    {

        /// <summary>
        /// 关键要public 不然autofac不会自动注入
        /// </summary>
        public ILazyServiceProvider LazyServiceProvider { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected IExampleApeartDbContext _dbContext => LazyServiceProvider.LazyGetRequiredService<IExampleApeartDbContext>();

        /// <summary>
        /// 
        /// </summary>
        protected CacheHandler _userHelper => LazyServiceProvider.LazyGetRequiredService<CacheHandler>();

        /// <summary>
        /// 当前登录用户
        /// </summary>
        protected CurrentUser _currentUser => LazyServiceProvider.LazyGetRequiredService<CurrentUser>();

        /// <summary>
        /// 
        /// </summary>
        protected ExampleApeartAppService()
        {
            //ObjectMapperContext = typeof(ExampleApeartApplicationModule);
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
            return _currentUser.Token;
            //if (_httpContext.Request.Headers.TryGetValue(PublicString.TokenHeadName, out var heads))
            //{
            //    var token = heads.FirstOrDefault();
            //    if (!String.IsNullOrEmpty(token))
            //    {
            //        if (token.TokenCheck())
            //        {
            //            return token;
            //        }
            //    }
            //}
            //if (_httpContext.Request.Cookies.TryGetValue(PublicString.TokenHeadName, out var cookie))
            //{
            //    if (!String.IsNullOrEmpty(cookie))
            //    {
            //        if (cookie.TokenCheck())
            //        {
            //            return cookie.ToString();
            //        }
            //    }
            //}
            //return "";
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
        /// 名称映射设置
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        protected string DefaultMapperEntityName(string entityName)
        {
            switch (entityName)
            {
                case "Share": return "ShareInfo";//示例
            }
            return entityName;
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
