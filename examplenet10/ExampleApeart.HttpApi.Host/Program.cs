using Autofac;
using Autofac.Extensions.DependencyInjection;
using ExampleApeart.Handler;
using PasteApeart;
using PasteApeart.Handler.lazy;
using Serilog;

//using Serilog.Sinks.RabbitMQ;

namespace ExampleApeart
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {

        /// <summary>
        /// 
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            try
            {

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 系统开始启动");

                Directory.SetCurrentDirectory(AppContext.BaseDirectory);

                Configuration = ExampleApeartEntityFrameworkCoreModule.BuildConfiguration(args);

                // 修改这里：使用 AutofacServiceProviderFactory
                var host = Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
                    {
                        // Autofac 配置
                        ConfigureAutofac(containerBuilder);
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseConfiguration(Configuration);
                        webBuilder.UseStartup<Startup>();
                    })
                    .UseSerilog()
                    .Build();

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration)
                    .WriteTo.Sink(new SerilogCustomHandler(host.Services), Serilog.Events.LogEventLevel.Error)
                    .Enrich.FromLogContext()
                    .CreateLogger();

                if (!System.IO.Directory.Exists("data"))
                {
                    System.IO.Directory.CreateDirectory("data");
                }

                Log.Information("Starting web host.");
                host.Run();

                return 0;

                //Directory.SetCurrentDirectory(AppContext.BaseDirectory);

                //Configuration = ExampleApeartEntityFrameworkCoreModule.BuildConfiguration(args);
                //var builder = CreateHostBuilder(args).Build();
                //Log.Logger = new LoggerConfiguration()
                //    .ReadFrom.Configuration(Configuration)
                //    .WriteTo.Sink(new SerilogCustomHandler(builder.Services), Serilog.Events.LogEventLevel.Error)
                //    .Enrich.FromLogContext()
                //    .CreateLogger();

                //if (!System.IO.Directory.Exists("data")) { System.IO.Directory.CreateDirectory("data"); }

                //var _model = Environment.GetEnvironmentVariable("RunConfig:Model");
                //Log.Information("Starting web host.");
                //builder.Run();
                //return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureAutofac(ContainerBuilder containerBuilder)
        {
            // 获取程序集
            var applicationAssembly = typeof(ExampleApeartApplicationModule).Assembly;

            // 1. 注册所有 AppService（启用属性注入）
            containerBuilder.RegisterAssemblyTypes(applicationAssembly)
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IAppService).IsAssignableFrom(t))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .PropertiesAutowired();  // 关键：启用属性注入

            // 2. 注册 LazyServiceProvider
            containerBuilder.RegisterType<LazyServiceProvider>()
                .InstancePerLifetimeScope()
                .PropertiesAutowired();

            //// 关键：将所有接口都指向 SqliteDbContext 可以在这里注入，也可以在EF那边那样注入
            //builder.RegisterType<SqliteDbContext>()
            //    .As<IApBaseDb>()
            //    .As<IApeartDbContext>()
            //    .As<ApBaseDb<SqliteDbContext>>()
            //    .As<ApeartDbContext<SqliteDbContext>>()
            //    .InstancePerLifetimeScope();

            // 3. 注册 Startup 所在程序集的所有 Controller
            var hostAssembly = typeof(Startup).Assembly;
            containerBuilder.RegisterAssemblyTypes(hostAssembly)
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller"))
                .InstancePerLifetimeScope()
                .PropertiesAutowired();

            // 4. 如果有其他程序集需要注册，继续添加
            // var domainAssembly = typeof(ExampleApeartDomainModule).Assembly;
            // containerBuilder.RegisterAssemblyTypes(domainAssembly)
            //     .Where(t => t.IsClass && !t.IsAbstract)
            //     .AsImplementedInterfaces()
            //     .InstancePerLifetimeScope()
            //     .PropertiesAutowired();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(Configuration);
                    webBuilder.UseStartup<Startup>();
                })
            .UseSerilog();
        //.UseAutofac();

        //	  <PackageReference Include="Microsoft.Extensions.Hosting.WindowsServices" Version="6.0.3" />
        //internal static Microsoft.Extensions.Hosting.IHostBuilder CreateHostServiceBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //    .UseWindowsService(s => { s.ServiceName = "ExampleApeart"; })
        //    .ConfigureWebHostDefaults(webBuilder =>
        //    {
        //        //webBuilder.UseUrls("http://*:51666");//由外部指定 --urls=http://*:51666
        //        webBuilder.UseConfiguration(Configuration);
        //        webBuilder.UseStartup<Startup>();
        //    })
        //    .UseContentRoot(AppContext.BaseDirectory)
        //    .UseSerilog()
        //    .UseAutofac();
    }
}
