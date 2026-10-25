using ExampleForm.usermodels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PasteApeart;
using PasteApeart.Handler;
using PasteApeart.Handler.common;
using PasteApeart.Handler.Level;
using PasteApeart.Handler.rabbit;
using Volo.Abp.Modularity;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    [DependsOn(
        typeof(ExampleFormDomainModule),
        typeof(ExampleFormApplicationContractsModule)
        //typeof(AbpAutoMapperModule),
        //typeof(AbpObjectMappingModule)
        )]
    public class ExampleFormHandlerModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            base.ConfigureServices(context);

            context.Services.AddScoped<ILevelHandler>(sp => new LevelHandler(sp.GetRequiredService<IExampleFormDbContext>().As<DbContext>()));
            //context.Services.AddScoped<LevelHandler>(sp => new LevelHandler(sp.GetRequiredService<IExampleFormDbContext>().As<DbContext>()));

            context.Services.AddSingleton<IChannelHelper, ChannelHelper>();

            context.Services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();

            context.Services.AddSingleton<IRabbitMultiConsumerHandler, RabbitMultiConsumerHandler>();

            context.Services.AddSingleton<IEventActionHandler>(sp =>
                new EventActionHandler(
                    sp.GetRequiredService<ILogger<EventActionHandler>>(),
                    sp,
                    sp.GetRequiredService<IChannelHelper>(),
                    typeof(UserInfoDto).Assembly.GetTypes())
            );

            context.Services.AddScoped<IEventQuickActionHandler>(sp =>
                new EventQuickActionHandler(
                    sp.GetRequiredService<ILogger<EventQuickActionHandler>>(),
                    sp.GetRequiredService<IEventActionHandler>(),
                    typeof(UserInfoDto).Assembly.GetTypes())
            );

        }
    }
}
