using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasteApeart;
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

            services.AddAutoDependencyInjection(typeof(PasteApeartHandlerModule).Assembly);

            services.AddAutoDependencyInjection(typeof(ExampleApeartHandlerModule).Assembly);


            return services;
        }

    }

}
