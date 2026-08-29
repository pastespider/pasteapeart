using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasteApeart;
using PasteApeart.Handler;
using PasteApeart.Handler.eventmodels;
using PasteApeart.Handler.rabbitmodel;
using System.Collections.Concurrent;

namespace ExampleApeart
{


    public static class ExampleApeartHandlerModule
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddExampleApeartHandlerModule(this IServiceCollection services, IConfiguration configuration = null)
        {
            services.AddTransient<ILazyServiceProvider, LazyServiceProvider>();
            services.AddScoped<CurrentUser>();

            services.AddSingleton<IChannelHelper,ChannelHelper>();
            services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();
            services.AddSingleton<IEventActionHandler, EventActionHandler>();
            services.AddSingleton<RabbitMultiConsumerHandler>();
            services.AddSingleton<RabbitMQProducer>();

            services.AddSingleton<ChannelHelper>();


            //services.AddAutoDependencyInjection(typeof(PasteApeartHandlerModule).Assembly);

            services.AddAutoDependencyInjection(typeof(ExampleApeartHandlerModule).Assembly);

            ////还是使用下面的扫描注入
            //var handlers = typeof(ExampleApeartHandlerModule).Assembly
            //    .GetTypes()
            //    .Where(t => !t.IsAbstract &&
            //               !t.IsGenericType &&
            //               t.GetInterfaces()
            //                .Any(i => i.IsGenericType &&
            //                         i.GetGenericTypeDefinition() == typeof(IEventHandler<>)));
            //foreach (var handler in handlers)
            //{
            //    var interfaceType = handler.GetInterfaces()
            //        .First(i => i.IsGenericType &&
            //                    i.GetGenericTypeDefinition() == typeof(IEventHandler<>));
            //    //Console.WriteLine($"Auto Register {interfaceType.FullName}-{handler.FullName}");
            //    //context.Services.AddTransient(interfaceType, handler);
            //    if (typeof(ISingletonDependency).IsAssignableFrom(handler))
            //    {
            //        services.AddSingleton(interfaceType, handler);
            //    }
            //    else if (typeof(IScopedDependency).IsAssignableFrom(handler))
            //    {
            //        services.AddScoped(interfaceType, handler);
            //    }
            //    else // 默认使用 ITransientDependency
            //    {
            //        services.AddTransient(interfaceType, handler);
            //    }
            //}

            return services;
        }

    }

}
