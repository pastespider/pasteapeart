using PasteApeart.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders.Testing;
using System.Text.Json;
using System.Text.Json.Serialization;
//using PasteForm.Helper;
//using Volo.Abp.AutoMapper;

namespace PasteApeart
{

    /// <summary>
    /// 
    /// </summary>
    public static class PasteApeartApplicationModule
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasteApeartApplicationModule(this IServiceCollection services, IConfiguration configuration = null)
        {
            var assembly = typeof(PasteApeartApplicationModule).Assembly;

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

            // 注册 AutoApiConvention
            services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new ApeartAppServiceAutoConvention());
            });

            // 自动注入
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
}
