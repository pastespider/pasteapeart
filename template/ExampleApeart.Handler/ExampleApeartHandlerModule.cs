using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasteApeart;
using System.Collections.Concurrent;

namespace $ext_safeprojectname$
{


    public static class $ext_safeprojectname$HandlerModule
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection Add$ext_safeprojectname$HandlerModule(this IServiceCollection services, IConfiguration configuration = null)
        {
            services.AddTransient<ILazyServiceProvider, LazyServiceProvider>();
            services.AddScoped<CurrentUser>();

            services.AddSingleton<IChannelHelper,ChannelHelper>();
            services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();
            services.AddSingleton<IEventActionHandler, EventActionHandler>();
            services.AddSingleton<RabbitMultiConsumerHandler>();
            services.AddSingleton<RabbitMQProducer>();
            services.AddSingleton<ChannelHelper>();

            services.AddAutoDependencyInjection(typeof(PasteApeartHandlerModule).Assembly);

            services.AddAutoDependencyInjection(typeof($ext_safeprojectname$HandlerModule).Assembly);


            return services;
        }

    }

}
