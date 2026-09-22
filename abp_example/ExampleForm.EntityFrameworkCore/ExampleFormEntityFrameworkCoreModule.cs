using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ExampleForm.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    [DependsOn(
        typeof(ExampleFormDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
    )]
    public class ExampleFormEntityFrameworkCoreModule : AbpModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            //Console.WriteLine($"{DateTime.Now} ExampleFormEntityFrameworkCoreModule.ConfigureServices");
            context.Services.AddAbpDbContext<ExampleFormDbContext>(options =>
            {
                options.AddDefaultRepositories<ExampleFormDbContext>(includeAllEntities: true);
            });
            context.Services.AddAbpDbContext<SqliteDbContext>(options =>
            {
                options.AddDefaultRepositories<SqliteDbContext>(includeAllEntities: true);
            });
            context.Services.Configure<AbpDbContextOptions>(options =>
            {
                options.Configure<ExampleFormDbContext>(option =>
                {
                    option.UseNpgsql();
                });
                options.Configure<SqliteDbContext>(option =>
                {
                    option.UseSqlite();
                });
            });


            var _type = BuildConfiguration(null).GetValue<string>("ConnectionStrings:DataType");
            if (_type == "pgsql")
            {
                System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                //判断什么数据库类型
                context.Services.AddScoped<IExampleFormDbContext, ExampleFormDbContext>();
                //Configure<AbpDbContextOptions>(options => { options.UseNpgsql(); });
                //context.Services.AddAbpDbContext<ExampleFormDbContext>(options =>
                //{
                //    options.AddDefaultRepositories<ExampleFormDbContext>(includeAllEntities: true);
                //});
                //context.Services.Configure<AbpDbContextOptions>(options =>
                //{
                //    options.Configure<ExampleFormDbContext>(option =>
                //    {
                //        option.UseNpgsql();
                //    });
                //});
            }
            else
            {
                Console.WriteLine("SqlType is sqlite");

                //读写分离支持 如果不需要，需要把下面三行给注释掉
                context.Services.AddScoped<IConnectionStringSelector, ConnectionStringSelector>();
                context.Services.Replace(ServiceDescriptor.Singleton<IConnectionStringResolver, DynamicConnectionStringResolver>());
                //  app.UseMiddleware<ConnectionStringMiddleware>(); 在UseRouting之后

                context.Services.AddScoped<IExampleFormDbContext, SqliteDbContext>();

                //context.Services.AddAbpDbContext<SqliteDbContext>(options =>
                //{
                //    options.AddDefaultRepositories<SqliteDbContext>(includeAllEntities: true);
                //});
                //context.Services.Configure<AbpDbContextOptions>(options =>
                //{
                //    options.Configure<SqliteDbContext>(option =>
                //    {
                //        option.UseSqlite();
                //    });
                //});
            }

            SequentialGuidValueGenerator._guidGenerator = context.Services.BuildServiceProvider().GetRequiredService<IGuidGenerator>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            base.PreConfigureServices(context);
        }

        /// <summary>
        /// 
        /// </summary>
        private static IConfigurationRoot _config { get; set; } = null;

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