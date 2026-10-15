using PasteApeart.Dependency;
using PasteApeart.EntityFrameworkCore;
using System.Security.Claims;

namespace ExampleApeart.HttpApi.Host.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthMiddleware : IMiddleware, IScopedDependency
    {
        private readonly CurrentUser _currentUser;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public AuthMiddleware(CurrentUser current)
        {
            _currentUser = current;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = String.Empty;
            if (context.Request.Headers.TryGetValue(PublicString.TokenHeadName, out var vals))
            {
                foreach (var htoken in vals)
                {
                    if (htoken.TokenCheck())
                    {
                        token = htoken;
                        break;
                    }
                }
            }
            if (String.IsNullOrEmpty(token))
            {
                if (context.Request.Cookies.TryGetValue(PublicString.TokenHeadName, out var tok))
                {
                    if (tok.TokenCheck())
                    {
                        token = tok;
                    }
                }
            }
            if (!String.IsNullOrEmpty(token))
            {
                //Console.WriteLine($"解析到密钥：{token}");

                var userId = token.CurrentUserId();
                //context.Items["CurrentUser"] = new CurrentUser { Id = userId, Token = token };
                var claims = new[] {
                    new Claim("FilterSiteId", "true"),
                    new Claim(ClaimTypes.NameIdentifier,userId.ToString()),
                    new Claim("Token", token),
                    new Claim("UserName", token),
                    new Claim("UserId", userId.ToString())
                };
                var identity = new ClaimsIdentity(claims, "Custom");//Simple
                context.User = new ClaimsPrincipal(identity);
                //context.User.FindFirstValue
                _currentUser.Id = userId;
                _currentUser.Token = token;
                //Console.WriteLine($"配置了用户信息{token} {userId}");
                context.Items["CurrentUser"] = _currentUser;
            }

            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<UseReadOnlyConnectionAttribute>() != null)
            {
                context.Items["IsReadOnly"] = true;
            }
            await next(context);
        }
    }
}
