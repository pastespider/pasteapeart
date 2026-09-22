
## 使用案例代码


```csharp

        /// <summary>
        /// 配置autofac
        /// </summary>
        /// <param name="containerBuilder"></param>
        private static void ConfigureAutofac(ContainerBuilder containerBuilder)
        {

            /*
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
             */

            // 获取程序集
            // 1. 注册所有 AppService（启用属性注入）
            containerBuilder.RegisterAssemblyTypes(typeof(ExampleApeartApplicationModule).Assembly)
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

```