namespace PasteApeart.Handler.lazy
{

    /// <summary>
    /// 懒加载 需要使用AUTOFAC
    /// </summary>
    public interface ILazyServiceProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T LazyGetRequiredService<T>() where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object LazyGetRequiredService(Type serviceType);
    }

}
