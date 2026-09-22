using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using PasteApeart;
using ExampleApeart.EntityFrameworkCore;
using PasteApeart.Handler.lazy;

namespace ExampleApeart.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class IBaseController : ControllerBase
    {
        /// <summary>
        /// 用了AUTOFACE所以这里要public
        /// </summary>
        public LazyServiceProvider LazyServiceProvider { get; set; }

        //private IAppCache _appCache => LazyServiceProvider.LazyGetRequiredService<IAppCache>();


        // 方式3：懒加载版本
        private ILogger _logger;
        /// <summary>
        /// 
        /// </summary>
        protected ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    var loggerFactory = LazyServiceProvider.LazyGetService<ILoggerFactory>();
                    _logger = loggerFactory?.CreateLogger(GetType().FullName!) ?? NullLogger.Instance;
                }
                return _logger;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected IExampleApeartDbContext _dbContext => LazyServiceProvider.LazyGetRequiredService<IExampleApeartDbContext>();


        /// <summary>
        /// 获取登录者UID
        /// </summary>
        /// <returns></returns>
        protected int CurrentUserId()
        {
            var token = this.CurrentUserToken();
            if (!String.IsNullOrEmpty(token))
            {
                return token.CurrentUserId();
            }
            return 0;
        }

        /// <summary>
        /// 获取登录者UID
        /// </summary>
        /// <returns></returns>
        protected string CurrentUserToken()
        {
            if (base.HttpContext.Request.Headers.TryGetValue(PublicString.TokenHeadName, out var hhs))
            {
                foreach (var head in hhs)
                {
                    if (head.TokenCheck())
                    {
                        return head;
                    }
                }
            }
            if (base.HttpContext.Request.Cookies.TryGetValue(PublicString.TokenHeadName, out var ccc))
            {
                if (!String.IsNullOrEmpty(ccc))
                {
                    if (ccc.TokenCheck())
                    {
                        return ccc;
                    }
                }
            }
            return "";
        }


    }
}
