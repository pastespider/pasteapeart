using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PasteApeart;
using PasteApeart.Handler;
using ExampleApeart.Application.Contracts;
using ExampleApeart.Handler;
using ExampleApeart.HttpApi.Host.auditmodels;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;
using System.Text.Json.Serialization;
using PasteApeart.Handler.redis;
using PasteApeart.Dependency;
using PasteApeart.Handler.Extensions;

namespace ExampleApeart.HttpApi.Host
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExampleApeartHttpApiHostModule
    {
        /// <summary>
        /// 
        /// </summary>
        public const string DefaultCorsPolicyName = "Default";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Services"></param>
        /// <param name="Configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddExampleApeartHttpApiHostModule(this IServiceCollection Services, IConfiguration Configuration = null)
        {

            Services.AddAutoDependencyInjection(typeof(ExampleApeartHttpApiHostModule).Assembly);

            //这个是对返回值序列号的格式转换
            //Services.Configure<JsonOptions>(options =>
            //{
            //    options.InputDateTimeFormats = new List<string> { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd" };//.Add("yyyy-MM-dd HH:mm:ss");
            //    options.OutputDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            //    //options.DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            //    //options.UseHybridSerializer = false;//使用newtonsoft代替system.text.json
            //});  //对类型为DateTimeOffset生效

            Services.AddMemoryCache();
            Services.AddHttpClient();

            //swagger的输入格式默认
            Services.Configure<SwaggerGenOptions>(options =>
            {
                options.MapType<DateTime>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "yyyy-MM-dd HH:mm:ss", // 自定义格式
                    Example = new OpenApiString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                });

                options.MapType<DateTimeOffset>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "yyyy-MM-dd HH:mm:ss", // 自定义格式
                    Example = new OpenApiString(DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                });
            });

            //Services.Configure<A>(Configuration.GetSection("ConnectionStrings"));

            Services.Configure<RedisConfig>(Configuration.GetSection("Redis"));//abc:abcitem

            Services.Configure<AppConfig>(Configuration.GetSection("AppConfig"));//abc:abcitem

            var config = new AppConfig();
            Configuration.GetSection("AppConfig").Bind(config);
            //var _type = Services.GetConfiguration().GetValue<string>("ConnectionStrings:CacheType");
            //if (_type == "redis")
            if (config.CacheType == "redis")
            {
                //载入第一个
                Services.AddSingleton<IAppCache, RedisClient>();
            }
            else
            {
                Services.AddSingleton<IAppCache, MemoryClient>();
            }

            //Services.Configure<Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcOptions>(options =>
            //{
            //    options.ConventionalControllers.Create(typeof(ExampleApeartApplicationModule).Assembly, option =>
            //    {
            //        option.UseV3UrlStyle = true;
            //        option.RootPath = "app";
            //    });
            //});

            Services.AddSwaggerGen(
                options =>
                {
                    //Bearer
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Name = PublicString.TokenHeadName,
                        Scheme = "",
                        Description = "在此输入token信息",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    });
                    //添加文档
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Support APP API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) =>
                    {
                        return (description.RelativePath.Contains("/app/") || description.RelativePath.Contains("/plan/"));
                    });
                    options.CustomSchemaIds(type => type.FullName);
                    //添加小锁
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                                   {new OpenApiSecurityScheme{ Reference=new OpenApiReference{ Type=ReferenceType.SecurityScheme,Id="Bearer" }},new string[]{ }}
                    });

                    options.ResolveConflictingActions(apidesc => apidesc.First());

                    //这里可以用isdevmodel来判断
                    if (System.IO.File.Exists(@"ExampleApeart.Application.xml")) { options.IncludeXmlComments(@"ExampleApeart.Application.xml"); }
                    if (System.IO.File.Exists(@"ExampleApeart.Application.Contracts.xml")) { options.IncludeXmlComments(@"ExampleApeart.Application.Contracts.xml"); }
                    if (System.IO.File.Exists(@"ExampleApeart.Domain.xml")) { options.IncludeXmlComments(@"ExampleApeart.Domain.xml"); }
                    if (System.IO.File.Exists(@"ExampleApeart.HttpApi.Host.xml")) { options.IncludeXmlComments(@"ExampleApeart.HttpApi.Host.xml"); }
                    //添加模块介绍
                    options.DocumentFilter<AuthTagDescriptions>();
                }
            );

            //全局处理异常信息
            Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            Services.AddControllers(options => { options.Filters.Add(typeof(PasteThrowExceptionAttribute)); });
            //AppService中有添加
                //.AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                //    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;//允许数字从字符串中获取
                //    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;  // 驼峰 小写开头
                //    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                //    //options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());  // 枚举转字符串
                //    //options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                //    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;  // 忽略循环引用
                //});

            Services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(
                            Configuration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            //context.UsePasteRateLimiter();

            //添加审计日志
            Services.AddAuditLogging();

            Services.AddHostedService<StartHostedService>();

            return Services;
        }
    }
}
