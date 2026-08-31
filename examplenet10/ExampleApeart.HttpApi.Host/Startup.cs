using ExampleApeart.HttpApi.Host;
using ExampleApeart.HttpApi.Host.auditmodels;
using ExampleApeart.HttpApi.Host.Services;
using Scalar.AspNetCore;

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

            services.AddOpenApi("v1");
            services.AddOpenApi("v2");
            services.AddOpenApi("v3");
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
                //if (context.Request.Path.StartsWithSegments("/scalar"))
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

            app.UseEndpoints(options =>
            {

                options.MapScalarApiReference(opa =>
                {
                    opa.AddDocument("v1", "API V1", "/openapi/v1.json");
                    opa.AddDocument("v2", "API V2", "/openapi/v2.json");
                    opa.AddDocument("v3", "API V3", "/openapi/v3.json");
                });
                options.MapOpenApi();
                options.MapControllers();
            });

            //app.UseConfiguredEndpoints();
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 系统启动完成!");
        }
    }

}
