using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PasteApeart.Dependency
{
    /// <summary>
    /// 自动执行注入
    /// </summary>
    public static class AutoDependencyExtensions
    {

        /// <summary>
        /// 扫描注入 瞬时 作用域 单例等 ITransient ISingleton IScoped
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoDependency(this IServiceCollection services, Assembly assembly)
        {
            // 1. 扫描所有类型
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface);

            foreach (var type in types)
            {
                // 检查实现的接口
                var interfaces = type.GetInterfaces();

                // ITransientDependency → 瞬时
                if (interfaces.Any(i => i == typeof(ITransient)))
                {
                    // 注册自身
                    services.AddTransient(type);

                    // 注册所有接口（除了标记接口本身）
                    var serviceInterfaces = interfaces.Where(i => i != typeof(ITransient));
                    foreach (var serviceInterface in serviceInterfaces)
                    {
                        services.AddTransient(serviceInterface, type);
                    }
                }
                // ISingletonDependency → 单例
                else if (interfaces.Any(i => i == typeof(ISingleton)))
                {
                    services.AddSingleton(type);

                    var serviceInterfaces = interfaces.Where(i => i != typeof(ISingleton));
                    foreach (var serviceInterface in serviceInterfaces)
                    {
                        services.AddSingleton(serviceInterface, type);
                    }
                }
                // IScopedDependency → 作用域
                else if (interfaces.Any(i => i == typeof(IScoped)))
                {
                    services.AddScoped(type);

                    var serviceInterfaces = interfaces.Where(i => i != typeof(IScoped));
                    foreach (var serviceInterface in serviceInterfaces)
                    {
                        services.AddScoped(serviceInterface, type);
                    }
                }
            }

            return services;
        }
    }
}
