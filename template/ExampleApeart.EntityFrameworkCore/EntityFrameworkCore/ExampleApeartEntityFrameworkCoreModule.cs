using $ext_safeprojectname$.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasteApeart.EntityFrameworkCore;
using PasteApeart.EntityFrameworkCore.auditing;

namespace $ext_safeprojectname$
{

    /// <summary>
    /// 
    /// </summary>
    public static class $ext_safeprojectname$EntityFrameworkCoreModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        public static void RegisterDbContext<T>(this IServiceCollection services)
            where T : DbContext, ICollectAuditDb, I$ext_safeprojectname$DbContext
        {
            services.AddScoped<T>();
            services.AddScoped<ICollectAuditDb>(sp => sp.GetRequiredService<T>());
            services.AddScoped<I$ext_safeprojectname$DbContext>(sp => sp.GetRequiredService<T>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection Add$ext_safeprojectname$EntityFrameworkCoreModule(this IServiceCollection services, IConfiguration configuration = null)
        {
            //services.AddScoped<IConnectionStringSelector, ConnectionStringSelector>();


            //IServiceCollection
            var config = new AppConfig();
            if (configuration != null)
            {
                configuration.GetSection("AppConfig").Bind(config);
            }
            else
            {
                var buildConfig = BuildConfiguration();
                buildConfig.GetSection("AppConfig").Bind(config);
            }


            if (config.UseReadWrite)
            {
                services.AddSingleton<ReadWriteSplitInterceptor>();
            }
            if (config.DataType == "sqlite")
            {
                services.AddScoped<ICollectAuditDb>(sp => sp.GetRequiredService<SqliteDb>());
                services.AddScoped<I$ext_safeprojectname$DbContext>(sp => sp.GetRequiredService<SqliteDb>());
                // 如果中间件或其他地方注入了抽象类，也需要注册
                services.AddScoped<CollectAuditDb<SqliteDb>>(sp => sp.GetRequiredService<SqliteDb>());
                services.AddScoped<AbsFilterDbContext<SqliteDb>>(sp => sp.GetRequiredService<SqliteDb>());

                //用了过滤就不能用DbContextPool
                services.AddDbContext<SqliteDb>((sp, options) =>
                {
                    options.UseSqlite(configuration.GetConnectionString($ext_safeprojectname$DbProperties.SqliteConnectionStringName));
                    if (config.UseReadWrite)
                    {
                        //读写分离 如果不需要那么用上面的，或者下面2行注释掉即可
                        var rwp = sp.GetRequiredService<ReadWriteSplitInterceptor>();
                        options.AddInterceptors(rwp);
                    }
                });
            }
            //是否判断是否开启了审计日志
            //审计日志
            services.AddSingleton<AuditMetadataCache>();
            services.AddScoped<MyAuditingDb>();
            services.AddDbContextPool<MyAuditingDb>((sp, options) =>
            {
                options.UseSqlite(configuration.GetConnectionString($ext_safeprojectname$DbProperties.AuditingConnectionStringName));
            });

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        private static IConfigurationRoot _config { get; set; } = null;

        /// <summary>
        /// 构建配置信息 因为add-migration需要用，所以迁移到这里，大家一起用
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IConfigurationRoot BuildConfiguration(string[] args = null)
        {
            if (_config != null)
            {
                return _config;
            }
            var _env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var build = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{(!String.IsNullOrEmpty(_env) ? _env : "Production")}.json", optional: true)
                    .AddEnvironmentVariables();
            if (args != null)
            {
                build = build.AddCommandLine(args);
            }
            _config = build.Build();
            return _config;
        }
    }

}