using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using PasteApeart;
using PasteApeart.auditmodels;
using PasteApeart.EntityFrameworkCore.auditing;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ExampleApeart.HttpApi.Host.auditmodels
{

    /// <summary>
    /// 审计日志中间件
    /// </summary>
    public class AuditMiddleware : IMiddleware
    {

        private readonly ILogger<AuditMiddleware> _logger;
        private readonly IAuditLogProvider _auditLogManager;
        private readonly AuditingConfigOptions _config;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="auditLogManager"></param>
        /// <param name="config"></param>
        /// <param name="actionDescriptorCollectionProvider"></param>
        public AuditMiddleware(ILogger<AuditMiddleware> logger, IAuditLogProvider auditLogManager, IOptions<AuditingConfigOptions> config, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {

            _logger = logger;
            _auditLogManager = auditLogManager;
            _config = config.Value;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="_next"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate _next)
        {
            if (!_config.Enable)
            {
                await _next(context);
                return;
            }

            if (IsAuditingDisabled(context))
            {
                await _next(context);
                return;
            }

            if (ShouldSkipAudit(context))
            {
                await _next(context);
                return;
            }
            var stopwatch = Stopwatch.StartNew();
            // 初始化审计日志
            var auditLog = new ApAuditLog
            {
                ExecutionTime = DateTime.UtcNow,
                Url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}",
                HttpMethod = context.Request.Method,
                QueryString = context.Request.QueryString.ToString(),
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                Headers = SerializeHeaders(context.Request.Headers)
            };

            // 获取用户信息
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                auditLog.UserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            if (!_config.EnableHttpGet)
            {
                if (context.Request.Method.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
                {
                    await _next(context);
                    return;
                }
            }

            // 读取请求体
            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();
                auditLog.RequestBody = await ReadRequestBody(context.Request);
                context.Request.Body.Position = 0;
            }

            try
            {
                if (!_config.LogResponseBody)
                {
                    await _next(context);
                }
                else
                {
                    var originalBodyStream = context.Response.Body;
                    // 需要记录，使用缓冲流
                    using var responseBodyStream = new MemoryStream();
                    context.Response.Body = responseBodyStream;

                    try
                    {
                        await _next(context);

                        // 只记录小型响应体，避免大文件问题
                        if (responseBodyStream.Length <= 10 * 1024 * 1024) // 10MB 限制
                        {
                            responseBodyStream.Seek(0, SeekOrigin.Begin);

                            var contentType = context.Response.ContentType ?? "";
                            if (IsTextContent(contentType))
                            {
                                var responseBody = await new StreamReader(responseBodyStream, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
                                auditLog.ResponseBody = Truncate(responseBody, 5000);
                            }
                            else
                            {
                                auditLog.ResponseBody = $"[Binary] Type: {contentType}, Size: {responseBodyStream.Length} bytes";
                            }
                        }
                        else
                        {
                            auditLog.ResponseBody = $"[Large File] Size: {responseBodyStream.Length} bytes, exceeded logging limit";
                        }
                        // 复制回原始流
                        responseBodyStream.Seek(0, SeekOrigin.Begin);
                        await responseBodyStream.CopyToAsync(originalBodyStream);
                    }
                    finally
                    {
                        context.Response.Body = originalBodyStream;
                    }
                }
                stopwatch.Stop();
                auditLog.ExecutionDuration = stopwatch.ElapsedMilliseconds;
                auditLog.HttpStatusCode = context.Response.StatusCode;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                auditLog.ExecutionDuration = stopwatch.ElapsedMilliseconds;
                auditLog.HasException = true;
                auditLog.Exception = ex.ToString();
                auditLog.HttpStatusCode = StatusCodes.Status500InternalServerError;
                throw;
            }
            finally
            {
                try
                {
                    if (context.Items[_config.AuditItemKey] is AuditCollection collectedData)
                    {
                        // 将实体变更序列化存入审计日志
                        if (collectedData.EntityChanges.Any())
                        {
                            auditLog.EntityChanges = collectedData.EntityChanges;
                        }
                        context.Items.Remove(_config.AuditItemKey);
                    }
                    // 保存审计日志
                    await _auditLogManager.InsertAsync(auditLog);
                }
                catch (Exception exl)
                {
                    //Console.WriteLine(exl.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        private string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }

        /// <summary>
        /// 记录header
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        private string SerializeHeaders(IHeaderDictionary headers)
        {
            //这里可以把cookie也去掉
            var safeHeaders = headers
                .Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) && !h.Key.Equals("token", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => string.Join(",", h.Value));
            return JsonSerializer.Serialize(safeHeaders);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            var contentType = request.ContentType ?? "";

            // 检查是否需要排除
            if (_config.ExcludedContentTypes.Any(ct => contentType.StartsWith(ct, StringComparison.OrdinalIgnoreCase)))
            {
                return $"[{contentType}] 内容未记录";
            }

            // 检查内容长度
            if (request.ContentLength > _config.MaxRequestBodySize)
            {
                return $"[请求体过大] ContentLength: {request.ContentLength} bytes，超过限制 {_config.MaxRequestBodySize} bytes";
            }

            // 对于非文本内容，不记录
            if (!IsTextContent(contentType))
            {
                return $"[非文本内容] ContentType: {contentType}";
            }

            try
            {
                request.EnableBuffering();
                request.Body.Position = 0;

                using var reader = new StreamReader(
                    request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true);

                var result = await reader.ReadToEndAsync();

                // 限制存储长度
                if (result.Length > _config.MaxRequestBodySize)
                {
                    result = result.Substring(0, _config.MaxRequestBodySize) +
                             $"... [截断，总长度: {result.Length} characters]";
                }
                if (request.Body.CanSeek)
                {
                    request.Body.Position = 0;
                }
                return result;
            }
            catch (Exception ex)
            {
                return $"[读取请求体失败: {ex.Message}]";
            }
        }

        /// <summary>
        /// 检查是否有禁用特性
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsAuditingDisabled(HttpContext context)
        {
            // 获取路由数据
            var routeData = context.GetRouteData();
            var controllerName = routeData.Values["controller"]?.ToString();
            var actionName = routeData.Values["action"]?.ToString();

            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
                return false;

            // 从所有 Action 描述中查找匹配的
            foreach (var descriptor in _actionDescriptorCollectionProvider.ActionDescriptors.Items)
            {
                if (descriptor is ControllerActionDescriptor controllerDescriptor)
                {
                    if (controllerDescriptor.ControllerName.Equals(controllerName, StringComparison.OrdinalIgnoreCase) &&
                        controllerDescriptor.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase))
                    {
                        // 检查 Action 特性
                        var actionDisabled = controllerDescriptor.MethodInfo
                            .GetCustomAttributes(typeof(DisableAuditingAttribute), true)
                            .Any();

                        // 检查 Controller 特性
                        var controllerDisabled = controllerDescriptor.ControllerTypeInfo
                            .GetCustomAttributes(typeof(DisableAuditingAttribute), true)
                            .Any();
                        //这里有问题 如果Controller是禁用的 Action是启用的那么应该启用
                        return actionDisabled || controllerDisabled;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private bool IsTextContent(string contentType)
        {
            var textTypes = new[]
            {
                "application/json",
                "application/xml",
                "text/",
                "application/x-www-form-urlencoded"
            };
            return textTypes.Any(tt => contentType.StartsWith(tt, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool ShouldSkipAudit(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            if (!String.IsNullOrEmpty(path))
            {
                if (_config.IgnoredPaths?.Count > 0)
                {
                    if (_config.IgnoredPaths.Contains(path))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }




}
