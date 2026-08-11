using ExampleApeart.HttpApi.Host;
using ExampleApeart.HttpApi.Host.auditmodels;
using ExampleApeart.HttpApi.Host.Services;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ExampleApeart
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IConfiguration _configuration;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            //Console.WriteLine("---ExampleApeart.Startup.Constructor---");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //Console.WriteLine("---ExampleApeart.Startup.ConfigureServices---");



            //读取配置，载入配置等

            services.AddHttpContextAccessor();

            services.AddExampleApeartEntityFrameworkCoreModule(_configuration);
            services.AddExampleApeartHandlerModule(_configuration);
            services.AddExampleApeartApplicationModule(_configuration);
            services.AddExampleApeartHttpApiHostModule(_configuration);

            // 这会自动注册 ILoggerFactory 和 ILogger<T>
            services.AddLogging();

            //services.AddApplication<ExampleApeartHttpApiHostModule>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //Console.WriteLine("---ExampleApeart.Startup.Configure---");
            app.UseHttpsRedirection();
            app.Use(async (context, next) =>
            {
                //if (context.Request.Path.StartsWithSegments("/swagger"))
                //{
                //    var _need_no_auth = true;
                //    if (context.Request.Cookies.ContainsKey(PublicString.TokenHeadName))
                //    {
                //        var token = context.Request.Cookies.TryGetValue(PublicString.TokenHeadName, out var cookie);
                //        if (!String.IsNullOrEmpty(cookie))
                //        {
                //            if (cookie.TokenCheck())
                //            {
                //                _need_no_auth = false;
                //            }
                //        }
                //    }
                //    if (_need_no_auth)
                //    {
                //        context.Response.StatusCode = 401;
                //        await context.Response.WriteAsync("Unauthorized");
                //        return;
                //    }
                //}
                await next();
            });
            app.UseStaticFiles();
            app.UseDefaultFiles();
            //app.UseRateLimiter();
            app.UseRouting();
            app.UseMiddleware<AuthMiddleware>();
            app.UseMiddleware<AuditMiddleware>();
            //读写分离
            //app.UseMiddleware<DbReadonlyMiddleware>();
            app.UseCors(ExampleApeartHttpApiHostModule.DefaultCorsPolicyName);
            app.UseResponseCaching();
            app.UseSwagger(c =>
            {
                //c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "文档V1接口");
                //options.RoutePrefix = "swagger";
                //options.InjectJavascript("/swagger/ui/abp.js");
                //这个有坑，要nginx那边配置/swagger/ui/abp.js/abp.swaager.js 我感觉应该不是这样的
                options.DocExpansion(DocExpansion.None);//是否展开
            });

            app.UseEndpoints(options =>
            {
                options.MapControllers();
                // 打印所有路由
                //Console.WriteLine("=== 所有已注册的路由 ===");
                //var dataSource = options.DataSources.FirstOrDefault();
                //if (dataSource != null)
                //{
                //    foreach (var endpoint in dataSource.Endpoints)
                //    {
                //        if (endpoint is RouteEndpoint routeEndpoint)
                //        {
                //            Console.WriteLine($"{routeEndpoint.DisplayName} -> {routeEndpoint.RoutePattern.RawText}");
                //        }
                //    }
                //}
            });
            
            //app.UseConfiguredEndpoints();
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 系统启动完成!");
        }
    }

}
