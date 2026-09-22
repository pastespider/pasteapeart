using ExampleForm.Handler;
using ExampleForm.HttpApi.Host.efhandler;
using ExampleForm.HttpApi.Host.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.WebEncoders.Testing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PasteApeart.Handler.redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text.Json;
using System.Text.Json.Serialization;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Json;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    [DependsOn(
        typeof(ExampleFormApplicationModule),
        typeof(ExampleFormEntityFrameworkCoreModule),
        typeof(AbpAutofacModule),
        typeof(AbpSwashbuckleModule)
        )]
    public class ExampleFormHttpApiHostModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var Configuration = context.Services.GetConfiguration();

            #region 格式化日期的输出 --------------------------------------------------------


            //context.Services.AddControllers().AddNewtonsoftJson(options =>
            //{
            //    //日期格式化
            //    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //    //首字母小写
            //    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            //    //格式化显示
            //    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //    //忽略循环引用
            //    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            //    var dateConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter
            //    {
            //        DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            //    };
            //    options.SerializerSettings.Converters.Add(dateConverter);
            //    options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            //});
            //Configure<MvcNewtonsoftJsonOptions>(options =>
            //{
            //    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";//对类型为DateTime的生效
            //    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            //    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

            //});

            //全局的默认配置
            Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings();
            Newtonsoft.Json.JsonConvert.DefaultSettings = new Func<Newtonsoft.Json.JsonSerializerSettings>(() =>
            {
                //日期格式化
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //是否格式化换行便于阅读
                setting.Formatting = Newtonsoft.Json.Formatting.Indented;
                //首字母小写
                setting.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                return setting;
            });

            //这个是对返回值序列号的格式转换
            Configure<AbpJsonOptions>(options =>
            {
                options.InputDateTimeFormats = new List<string> { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd" };//.Add("yyyy-MM-dd HH:mm:ss");
                options.OutputDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                //options.DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //options.UseHybridSerializer = false;//使用newtonsoft代替system.text.json
            });  //对类型为DateTimeOffset生效

            context.Services.Configure<AbpSystemTextJsonSerializerOptions>(options =>
            {
                // 这里配置 System.Text.Json
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;  // 驼峰
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Encoder = JavaScriptTestEncoder.UnsafeRelaxedJsonEscaping;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;  // 忽略循环引用
            });

            context.Services.Configure<JsonOptions>(options =>
            {
                // 这里配置 System.Text.Json
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;  // 驼峰
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Encoder = JavaScriptTestEncoder.UnsafeRelaxedJsonEscaping;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;  // 忽略循环引用
            });

            #endregion ============================================================================

            //swagger的输入格式默认
            context.Services.Configure<SwaggerGenOptions>(options =>
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


            context.Services.Configure<RedisConfig>(Configuration.GetSection("Redis"));//abc:abcitem

            var _type = context.Services.GetConfiguration().GetValue<string>("ConnectionStrings:CacheType");
            if (_type == "redis")
            {
                //载入第一个
                context.Services.AddSingleton<IAppCache, RedisClient>();
            }
            else
            {
                context.Services.AddSingleton<IAppCache, MemoryClient>();
            }


            context.Services.Configure<Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(ExampleFormApplicationModule).Assembly, option =>
                {
                    option.UseV3UrlStyle = true;
                    option.RootPath = "app";
                });
            });

            context.Services.AddSwaggerGen(
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
                    if (System.IO.File.Exists(@"ExampleForm.Application.xml")) { options.IncludeXmlComments(@"ExampleForm.Application.xml"); }
                    if (System.IO.File.Exists(@"ExampleForm.Application.Contracts.xml")) { options.IncludeXmlComments(@"ExampleForm.Application.Contracts.xml"); }
                    if (System.IO.File.Exists(@"ExampleForm.Domain.xml")) { options.IncludeXmlComments(@"ExampleForm.Domain.xml"); }
                    if (System.IO.File.Exists(@"ExampleForm.HttpApi.Host.xml")) { options.IncludeXmlComments(@"ExampleForm.HttpApi.Host.xml"); }
                    //添加模块介绍
                    options.DocumentFilter<AuthTagDescriptions>();
                }
            );

            //全局处理异常信息
            //context.Services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    options.SuppressModelStateInvalidFilter = true;
            //});
            context.Services.AddControllers(options => { options.Filters.Add(typeof(PasteThrowExceptionAttribute)); });


            context.Services.AddCors(options =>
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
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            //context.UsePasteRateLimiter();

            context.Services.AddHostedService<StartHostedService>();

            //禁用了还是生效? 调试生效还是咋生效的
            //context.Services.AddAntiforgery(options =>
            //{
            //    //options.HeaderName = "Your-Custom-Header-Name"; // 自定义请求头名称
            //    options.Cookie.Name = "XSRF-TOKEN"; // 自定义 Cookie 名称
            //    options.Cookie.HttpOnly = true; // 确保 Cookie 是 HttpOnly
            //    //options.Cookie.SecurePolicy = true;   // 确保 Cookie 仅通过 HTTPS 传输
            //});

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            //运行后台服务
            //context.AddBackgroundWorker<servicemodel.BackWorker>();
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            app.UseHttpsRedirection();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/swagger"))
                {
                    var _need_no_auth = true;
                    if (context.Request.Cookies.ContainsKey(PublicString.TokenHeadName))
                    {
                        var token = context.Request.Cookies.TryGetValue(PublicString.TokenHeadName, out var cookie);
                        if (!String.IsNullOrEmpty(cookie))
                        {
                            if (cookie.TokenCheck())
                            {
                                _need_no_auth = false;
                            }
                        }
                    }
                    if (_need_no_auth)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                }
                await next();
            });
            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseRouting();
            //app.UseRateLimiter();

            app.UseMiddleware<AuthMiddleware>();
            app.UseMiddleware<DbReadonlyMiddleware>();
            app.UseCors(DefaultCorsPolicyName);
            app.UseResponseCaching();
            app.UseSwagger(c =>
            {
                //c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "文档V1接口");
                //options.RoutePrefix = "swagger";
                //options.InjectJavascript("/swagger/ui/abp.js");
                //这个有坑，要nginx那边配置/swagger/ui/abp.js/abp.swaager.js 我感觉应该不是这样的
                options.DocExpansion(DocExpansion.None);//是否展开
            });
            //app.UseSwagger();
            //app.UseAbpSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support APP API");
            //});



            app.UseConfiguredEndpoints();
        }
    }
}
