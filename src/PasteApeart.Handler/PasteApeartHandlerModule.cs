using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace PasteApeart
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
            services.AddScoped<CurrentUser>();

            services.AddAutoDependencyInjection(typeof(PasteApeartHandlerModule).Assembly);

            return services;
        }

    }

}
