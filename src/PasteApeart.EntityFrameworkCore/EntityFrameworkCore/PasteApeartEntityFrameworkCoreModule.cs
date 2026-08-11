using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasteApeart.EntityFrameworkCore;
using PasteApeart.EntityFrameworkCore.auditing;

namespace PasteApeart
{

    /// <summary>
    /// 
    /// </summary>
    public static class PasteApeartEntityFrameworkCoreModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        public static void RegisterDbContext<T>(this IServiceCollection services)
            where T : DbContext, ICollectAuditDb, IPasteApeartDbContext
        {
            services.AddScoped<T>();
            services.AddScoped<ICollectAuditDb>(sp => sp.GetRequiredService<T>());
            services.AddScoped<IPasteApeartDbContext>(sp => sp.GetRequiredService<T>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasteApeartEntityFrameworkCoreModule(this IServiceCollection services, IConfiguration configuration = null)
        {
            services.AddScoped<IConnectionStringSelector, ConnectionStringSelector>();

            services.AddSingleton<ReadWriteSplitInterceptor>();
            services.AddScoped<ICollectAuditDb>(sp => sp.GetRequiredService<SqliteDb>());
            services.AddScoped<IPasteApeartDbContext>(sp => sp.GetRequiredService<SqliteDb>());
            // 如果中间件或其他地方注入了抽象类，也需要注册
            services.AddScoped<CollectAuditDb<SqliteDb>>(sp => sp.GetRequiredService<SqliteDb>());
            services.AddScoped<AbsFilterDbContext<SqliteDb>>(sp => sp.GetRequiredService<SqliteDb>());
            //services.RegisterDbContext<SqliteDbContext>();
            //用上面这一行
            //services.AddScoped<IServiceDbContext>(sp => sp.GetRequiredService<SqliteDbContext>());
            //用了过滤就不能用DbContextPool
            services.AddDbContext<SqliteDb>((sp, options) =>
            {
                options.UseSqlite(configuration.GetConnectionString(PasteApeartDbProperties.SqliteConnectionStringName));
                //读写分离 如果不需要那么用上面的，或者下面2行注释掉即可
                var rwp = sp.GetRequiredService<ReadWriteSplitInterceptor>();
                options.AddInterceptors(rwp);
            });

            //是否判断是否开启了审计日志
            //审计日志
            services.AddSingleton<AuditMetadataCache>();
            services.AddScoped<MyAuditingDb>();
            services.AddDbContextPool<MyAuditingDb>((sp, options) =>
            {
                options.UseSqlite(configuration.GetConnectionString(PasteApeartDbProperties.AuditingConnectionStringName));
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
        public static IConfigurationRoot BuildConfiguration(string[] args)
        {
            if (_config != null)
            {
                return _config;
            }

            _config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();
            return _config;
        }
    }

}