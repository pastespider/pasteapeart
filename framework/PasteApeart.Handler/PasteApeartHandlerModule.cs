using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasteApeart.Handler.rabbit;
using System.Collections.Concurrent;

namespace PasteApeart.Handler
{


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
            
            services.AddScoped<ICacheHandler, CacheHandler>();
            
            services.AddSingleton<IChannelHelper, ChannelHelper>();
            services.AddSingleton<IEventActionHandler, EventActionHandler>();

            services.AddScoped<CurrentUser>();
            services.AddAutoDependencyInjection(typeof(PasteApeartHandlerModule).Assembly);
            return services;
        }

    }

}
