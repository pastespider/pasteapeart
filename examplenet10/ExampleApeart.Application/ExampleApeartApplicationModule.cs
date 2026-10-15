using ExampleApeart.Application;
using ExampleApeart.EntityFrameworkCore;
using ExampleApeart.Handler;
using ExampleApeart.usermodels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders.Testing;
using PasteApeart;
using PasteApeart.Application;
using PasteApeart.Dependency;
using PasteApeart.Handler;
using System.Text.Json;
using System.Text.Json.Serialization;
//using PasteApeart.Helper;
//using Volo.Abp.AutoMapper;

namespace ExampleApeart
{

    /// <summary>
    /// 
    /// </summary>
    public static class ExampleApeartApplicationModule
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddExampleApeartApplicationModule(this IServiceCollection services, IConfiguration configuration = null)
        {
            var assembly = typeof(ExampleApeartApplicationModule).Assembly;

            // 先注册 Controllers，拿到 PartManager
            var mvcBuilder = services.AddControllers()
                .AddControllersAsServices()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.Encoder = JavaScriptTestEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;//允许数字从字符串中获取
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;  // 驼峰 小写开头
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    //options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());  // 枚举转字符串
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;  // 忽略循环引用
                    options.JsonSerializerOptions.Converters.Add(new SafeStringConverter());
                    options.JsonSerializerOptions.Converters.Add(new SafeDateTimeConverter());
                });
            var partManager = mvcBuilder.PartManager;

            // 添加 Feature Provider
            partManager.FeatureProviders.Add(new AppServiceControllerFeatureProvider());




            //// 注册 AutoApiConvention 方式一
            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Conventions.Add(new ApeartAppServiceAutoConvention());
            //});

            //方式二 把XXXAppService注入到路由中，这样外部可以访问
            services.AddTransient<AppServiceAutoConvention>();
            services.Configure<MvcOptions>(options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var convention = serviceProvider.GetRequiredService<AppServiceAutoConvention>();
                options.Conventions.Add(convention);
            });
            // 这里需要通过服务提供者获取


            //注入默认接口 承载add update detail list state 等
            services.AddSingleton<IPasteDefaultHelper>(sp =>new PasteDefaultHelper(sp.GetRequiredService<IMemoryCache>(),typeof(RoleInfo).Assembly.GetTypes(),typeof(RoleInfoDto).Assembly.GetTypes(),typeof(LinqExtend)));
            
            services.AddScoped<IPasteDefaultAppService>(sp =>
            {
                //方式一  扩展函数Func的模式
                //var defaultExtend = sp.GetRequiredService<DefaultApiExtendHandler>();
                //var defaultService = new PasteDefaultAppService(
                //    sp.GetRequiredService<PasteDefaultHelper>(),
                //    (sp.GetRequiredService<IExampleApeartDbContext>() as DbContext)
                //)
                //{
                //    //自定义扩展
                //    FuncFilterData = async (tuple) => { await defaultExtend.AuthCheckAsync(tuple.actionName, tuple.entityType, tuple.entityObject); },
                //};
                //方式二 你可以自己写一个，继承PasteApeartDefaultAppService
                var defaultService = new ExampleApeartDefaultService(
                    sp.GetRequiredService<IPasteDefaultHelper>(),
                    sp.GetRequiredService<IChannelHelper>(),
                    (sp.GetRequiredService<IExampleApeartDbContext>())
                )
                { };
                return defaultService;
            });



            // 以Scoped的方式注入AppService
            var appServiceTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IAppService).IsAssignableFrom(t))
                .ToList();
            if (appServiceTypes?.Count > 0)
            {
                foreach (var serviceType in appServiceTypes)
                {
                    services.AddScoped(serviceType);
                }
            }

            // 5. 自动注入其他依赖（ITransientDependency 等）
            services.AddAutoDependencyInjection(assembly);

            return services;
        }
    }

    /// <summary>
    /// 默认接口的扩展
    /// </summary>
    public class DefaultApiExtendHandler : IScopedDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="dtoType"></param>
        /// <returns></returns>
        public async Task AuthCheckAsync(string name, Type type, object dtoType)
        {
            //业务代码实现
            await Task.CompletedTask;
        }

        //其他的扩展实现
    }
}
