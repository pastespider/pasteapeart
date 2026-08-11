using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PasteApeart;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler;
using $ext_safeprojectname$.Application.Contracts;
using $ext_safeprojectname$.Handler;
using System.Text.Json;

namespace $ext_safeprojectname$
{

    /// <summary>
    /// 自定义全局错误处理，使用错误代码httpstatuscode
    /// </summary>
    public class PasteThrowExceptionAttribute : ExceptionFilterAttribute
    {

        /// <summary>
        /// 
        /// </summary>
        private readonly ILogger<PasteThrowExceptionAttribute> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public PasteThrowExceptionAttribute(ILogger<PasteThrowExceptionAttribute> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            // 如果异常没有被处理则进行处理
            if (context.ExceptionHandled == false)
            {
                var info = new ExceptionModel();
                info.code = "500";
                info.success = false;
                info.message = context.Exception.Message;

                if (context.Exception.InnerException != null)
                {
                    switch (context.Exception.InnerException)
                    {
                        case PasteCodeException pc:
                            {
                                info.code = $"{pc.Code}";
                            }
                            break;
                    }
                }
                else
                {
                    switch (context.Exception)
                    {
                        case PasteCodeException pc:
                            {
                                info.code = $"{pc.Code}";
                            }
                            break;
                    }
                }
                if (context.ModelState != null)
                {
                    if (context.ModelState.Count > 0)
                    {
                        foreach (var key in context.ModelState.Keys)
                        {
                            context.ModelState.TryGetValue(key, out var val);
                            if (val.Errors.Count > 0)
                            {
                                info.message = $"{key} {val.Errors[0].ErrorMessage}";
                                info.code = "400";
                                break;
                            }
                        }
                    }
                }
                if (info.code == "500")
                {
                    _logger.LogError(context.Exception.InnerException != null ? context.Exception.InnerException : context.Exception, context.HttpContext.Request.Path, null);
                }

                if (info.code == "204")
                {
                    context.Result = new ContentResult
                    {
                        StatusCode = 204
                    };
                }
                else
                {
                    context.Result = new ContentResult
                    {
                        StatusCode = int.Parse(info.code),
                        ContentType = "application/json;charset=utf-8",
                        Content = JsonSerializer.Serialize(info, JsonConfig.Options)
                    };
                }
            }
            // 设置为true，表示异常已经被处理了
            context.ExceptionHandled = true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnExceptionAsync(ExceptionContext context)
        {
            return base.OnExceptionAsync(context);
        }
    }


}
