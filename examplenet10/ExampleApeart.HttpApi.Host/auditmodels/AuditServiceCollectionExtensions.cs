namespace ExampleApeart.HttpApi.Host.auditmodels
{
    /// <summary>
    /// 
    /// </summary>
    public static class AuditServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuditLogging(this IServiceCollection services)
        {
            //        public static IServiceCollection AddAuditLogging(this IServiceCollection services, Action<AuditOptions> configure = null)
            //var options = new AuditOptions();
            //configure?.Invoke(options);

            //services.AddSingleton(options);

            // 注册审计日志服务
            //services.AddScoped<IAuditLogManager, AuditLogManager>();

            //用于入库，比如这里的存入SQLite数据库
            services.AddScoped<IAuditLogProvider, SqliteAuditLogProvider>(); // 可替换

            services.AddScoped<AuditMiddleware>();

            //services.AddSingleton<IActionDescriptorCollectionProvider, Defaultaction>();

            // 注册中间件
            //services.AddTransient<AuditMiddleware>();

            return services;
        }
    }
}
