using ExampleForm.Application;
using ExampleForm.Handler;
using ExampleForm.usermodels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using PasteApeart.Application;
using PasteApeart.Helper;
using Volo.Abp.Application;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    [DependsOn(
        typeof(ExampleFormHandlerModule),
        typeof(AbpDddApplicationModule)
        //typeof(AbpAutoMapperModule)
        //typeof(AbpObjectMappingModule)
        )]
    public class ExampleFormApplicationModule : AbpModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            context.Services.AddAutoMapperObjectMapper<ExampleFormApplicationModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                //是否一一映射
                options.AddMaps<ExampleFormApplicationModule>(validate: false);
            });

            //var config = TypeAdapterConfig.GlobalSettings;
            //config.Default.IgnoreNullValues(true);

            context.Services.AddSingleton<IPasteDefaultHelper>(sp =>
                new PasteDefaultHelper(
                    sp.GetRequiredService<IMemoryCache>(),
                    typeof(UserInfo).Assembly.GetTypes(),
                    typeof(UserInfoDto).Assembly.GetTypes(),
                    typeof(LinqExtend)
                )
            );

            //ExampleFormDefaultService
            context.Services.AddScoped<IPasteDefaultAppService, ExampleFormDefaultService>();
            //context.Services.AddScoped<IPasteDefaultAppService, PasteDefaultAppService>(sp =>
            //             new PasteDefaultAppService(
            //                 sp.GetRequiredService<IPasteDefaultHelper>(),
            //                 sp.GetRequiredService<IExampleFormDbContext>().As<DbContext>()
            //                 )
            //         );

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PasteResultDto<T> : ListResultDto<T>, IPagedResult<T>
    {
        /// <inheritdoc />
        public long TotalCount { get; set; } //TODO: Can be a long value..?

        /// <summary>
        /// Creates a new <see cref="PagedResultDto{T}"/> object.
        /// </summary>
        public PasteResultDto()
        {

        }

        /// <summary>
        /// 数据模型
        /// </summary>
        public VoloModelInfo PasteModel { get; set; }

        /// <summary>
        /// 设定类型
        /// </summary>
        /// <param name="listObj">new XXXListDto()</param>
        /// <param name="queryObj">new InputQueryXXX()</param>
        /// <returns></returns>
        public VoloModelInfo BuildModel(object listObj, object queryObj)
        {
            var _model = BuilderHelper.AttributeReadModelProperty(listObj, listObj.GetType(), true);
            var _query_model = BuilderHelper.AttributeReadModelProperty(queryObj, queryObj.GetType());
            if (_query_model != null)
            {
                _model.QueryProperties = _query_model.Properties;
            }
            PasteModel = _model;
            return _model;
        }

        /// <summary>
        /// Creates a new <see cref="PagedResultDto{T}"/> object.
        /// </summary>
        /// <param name="totalCount">Total count of Items</param>
        /// <param name="items">List of items in current page</param>
        public PasteResultDto(long totalCount, IReadOnlyList<T> items)
            : base(items)
        {
            TotalCount = totalCount;
        }
    }

}
