using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasteApeart.Handler.lazy;
using PasteApeart.Handler.rabbit;

namespace PasteApeart.Handler
{

    /// <summary>
    /// 
    /// </summary>
    public static class PasteApeartHandlerModule
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddPasteApeartHandlerModule(this IServiceCollection services, IConfiguration configuration = null)
        {
            services.AddTransient<ILazyServiceProvider, LazyServiceProvider>();

            services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();
            services.AddSingleton<IRabbitMultiConsumerHandler, RabbitMultiConsumerHandler>();

            //由于和IEntity绑定 所以一般是按照项目自定义 也便于扩展
            //services.AddScoped<ICacheHandler, CacheHandler>();

            services.AddSingleton<IChannelHelper, ChannelHelper>();
            services.AddSingleton<IEventActionHandler, EventActionHandler>();

            //services.AddScoped<CurrentUser>();

            //services.AddAutoDependencyInjection(typeof(PasteApeartHandlerModule).Assembly);

            return services;
        }

    }

}
