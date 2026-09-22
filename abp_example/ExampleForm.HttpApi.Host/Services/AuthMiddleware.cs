using System.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace ExampleForm.HttpApi.Host.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthMiddleware : IMiddleware, IScopedDependency
    {

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

                var claims = new[] { new Claim("FilterSiteId", "true"), new Claim(AbpClaimTypes.UserName, token), new Claim(AbpClaimTypes.SurName, token) };
                var identity = new ClaimsIdentity(claims, "Custom");//Simple
                context.User = new ClaimsPrincipal(identity);

            }
            await next(context);
        }
    }
}
