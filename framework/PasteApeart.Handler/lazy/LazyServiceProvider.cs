using Autofac;
using System.Collections.Concurrent;

namespace PasteApeart.Handler.lazy
{

    /// <summary>
    /// 
    /// </summary>
    public class LazyServiceProvider : ILazyServiceProvider
    {
        private readonly IComponentContext _componentContext;
        private readonly ConcurrentDictionary<string, Lazy<object>> _cache = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentContext"></param>
        public LazyServiceProvider(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LazyGetRequiredService<T>() where T : class
        {
            var key = typeof(T).FullName;
            var lazy = _cache.GetOrAdd(key, _ =>
                new Lazy<object>(() => _componentContext.Resolve<T>()));
            return (T)lazy.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LazyGetService<T>() where T : class
        {
            var key = typeof(T).FullName;
            var lazy = _cache.GetOrAdd(key, _ =>
                new Lazy<object>(() => _componentContext.ResolveOptional<T>()));
            return (T)lazy.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public T LazyGetService<T>(Func<IServiceProvider, object> factory) where T : class
        {
            var key = $"{typeof(T).FullName}_{factory.Method.Name}";
            var lazy = _cache.GetOrAdd(key, _ =>
                new Lazy<object>(() => factory(_componentContext.Resolve<IServiceProvider>())));
            return (T)lazy.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public T LazyGetRequiredService<T>(Func<IServiceProvider, object> factory) where T : class
        {
            var key = $"{typeof(T).FullName}_{factory.Method.Name}";
            var lazy = _cache.GetOrAdd(key, _ =>
                new Lazy<object>(() => factory(_componentContext.Resolve<IServiceProvider>())));
            return (T)lazy.Value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object LazyGetRequiredService(Type serviceType)
        {
            var key = serviceType.FullName;
            return _cache.GetOrAdd(key, type =>
                new Lazy<object>(() => _componentContext.Resolve(serviceType)))
                .Value;
        }
    }


}
