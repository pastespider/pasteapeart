//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ApplicationParts;
//using Microsoft.AspNetCore.Mvc.Controllers;
//using Microsoft.Extensions.DependencyInjection;
//using PasteApeart.Handler;
//using System.Reflection;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace PasteApeart
//{

//    /// <summary>
//    /// 
//    /// </summary>
//    public abstract class ApplicationService : IAppService
//    {
//        /// <summary>
//        /// 关键要public 不然autofac不会自动注入
//        /// </summary>
//        public ILazyServiceProvider LazyServiceProvider { get; set; }

//    }

//    ///// <summary>
//    ///// 
//    ///// </summary>
//    //public static class ApplyService
//    //{
//    //    /// <summary>
//    //    /// 
//    //    /// </summary>
//    //    /// <param name="services"></param>
//    //    /// <param name="assembly"></param>
//    //    /// <returns></returns>
//    //    public static IServiceCollection ApplyAppService(this IServiceCollection services, Assembly assembly)
//    //    {

//    //        //var assembly = typeof(PasteApeartApplicationModule).Assembly;

//    //        // 先注册 Controllers，拿到 PartManager
//    //        var mvcBuilder = services.AddControllers()
//    //            .AddControllersAsServices()
//    //            .AddJsonOptions(options =>
//    //            {
//    //                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
//    //                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;//允许数字从字符串中获取
//    //                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;  // 驼峰 小写开头
//    //                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//    //                //options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());  // 枚举转字符串
//    //                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;  // 忽略循环引用
//    //                options.JsonSerializerOptions.Converters.Add(new SafeStringConverter());
//    //                options.JsonSerializerOptions.Converters.Add(new SafeDateTimeConverter());
//    //            });
//    //        var partManager = mvcBuilder.PartManager;

//    //        // 添加 Feature Provider
//    //        partManager.FeatureProviders.Add(new AppServiceControllerFeatureProvider());

//    //        // 注册 AutoApiConvention
//    //        services.Configure<MvcOptions>(options =>
//    //        {
//    //            options.Conventions.Add(new ApeartAppServiceAutoConvention());
//    //        });

//    //        // 自动注入
//    //        var appServiceTypes = assembly.GetTypes()
//    //            .Where(t => t.IsClass && !t.IsAbstract && typeof(IAppService).IsAssignableFrom(t))
//    //            .ToList();
//    //        if (appServiceTypes?.Count > 0)
//    //        {
//    //            foreach (var serviceType in appServiceTypes)
//    //            {
//    //                services.AddScoped(serviceType);
//    //            }
//    //        }
//    //        return services;
//    //    }
//    //}
//}



//// 2. 注册




////namespace PasteApeart
////{
////    /// <summary>
////    /// 让 MVC 能发现 AppService
////    /// </summary>
////    public class AppServiceControllerFeatureProvider : ControllerFeatureProvider
////    {
////        ///// <summary>
////        ///// 
////        ///// </summary>
////        //public AppServiceControllerFeatureProvider()
////        //{
////        //    Console.WriteLine("AppServiceControllerFeatureProvider 初始化了");
////        //}

////        /// <summary>
////        /// 
////        /// </summary>
////        /// <param name="typeInfo"></param>
////        /// <returns></returns>
////        protected override bool IsController(TypeInfo typeInfo)
////        {
////            // 如果是抽象类或接口，跳过
////            if (!typeInfo.IsClass || typeInfo.IsAbstract || typeInfo.IsInterface)
////                return false;

////            // 实现了 IAppService 接口的类，都当作 Controller
////            if (typeof(IAppService).IsAssignableFrom(typeInfo))
////                return true;

////            // 默认规则：以 Controller 结尾
////            return typeInfo.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase);
////        }
////        //private void Test()
////        //{
////        //    //            // 方式1：让 MVC 发现所有实现 IAppService 的类
////        //    //            services.AddControllers()
////        //    //                .ConfigureApplicationPartManager(manager =>
////        //    //                {
////        //    //                    // 添加特性提供者，让 MVC 识别 AppService
////        //    //                    manager.FeatureProviders.Add(new AppServiceControllerFeatureProvider());
////        //    //                });
////        //    //// 方式2：改类名
////        //    //public class BookAppServiceController : IAppService { } // 加 Controller 后缀
////        //}
////    }

////}

//// 3. 你的 AppService 不需要加 Controller 后缀了
////public class BookAppService : IAppService
////    {
////        public async Task<List<BookDto>> GetListAsync() { }
////        public async Task<BookDto> GetAsync(Guid id) { }
////        public async Task CreateAsync(CreateBookDto input) { }
////    }


