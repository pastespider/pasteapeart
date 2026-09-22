using Microsoft.Extensions.Configuration;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace ExampleForm.EntityFrameworkCore
{
    /// <summary>
    /// 标注使用只读链接串
    /// </summary>
    public class UseReadOnlyConnectionAttribute : Attribute
    {

    }

    public interface IConnectionStringSelector
    {
        [Obsolete("use readonly")]
        string GetConnectionStringName();

        [Obsolete("use readonly")]
        void SetConnectionStringName(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsReadOnly();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isread"></param>
        void SetReadOnly(bool isread);
    }

    /// <summary>
    /// 返回当前上下文的链接串名称，注意是名称，不是链接字符串
    /// </summary>
    public class ConnectionStringSelector : IConnectionStringSelector
    {
        /// <summary>
        /// 
        /// </summary>
        private string _connectionStringName = ExampleFormDbProperties.SqliteConnectionStringName;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Obsolete("use readonly")]
        public string GetConnectionStringName() => _connectionStringName;

        /// <summary>
        /// 只读
        /// </summary>
        private bool _readOnly = false;

        /// <summary>
        /// 是否只读
        /// </summary>
        /// <returns></returns>
        public bool IsReadOnly() => _readOnly;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        [Obsolete("use readonly")]
        public void SetConnectionStringName(string name) => _connectionStringName = name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isread"></param>
        public void SetReadOnly(bool isread) => _readOnly = isread;
    }

    /// <summary>
    /// 基于链接名称，返回对应的链接串
    /// </summary>
    public class DynamicConnectionStringResolver : IConnectionStringResolver, IScopedDependency
    {
        private readonly DefaultConnectionStringResolver _defaultResolver;
        private readonly IConnectionStringSelector _selector;
        private readonly IConfiguration _configurationRoot;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultResolver"></param>
        /// <param name="selector"></param>
        /// <param name="configurationRoot"></param>
        public DynamicConnectionStringResolver(
            DefaultConnectionStringResolver defaultResolver,
            IConnectionStringSelector selector,
            IConfiguration configurationRoot)
        {
            _defaultResolver = defaultResolver;
            _selector = selector;
            _configurationRoot = configurationRoot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        public async Task<string> ResolveAsync(string connectionStringName = null)
        {
            var dynamicConnectionStringName = ExampleFormDbProperties.ConnectionStringName;
            var dataType = _configurationRoot.GetValue<string>("ConnectionStrings:DataType", String.Empty);
            if (dataType == "sqlite")
            {
                if (_selector.IsReadOnly())
                {
                    dynamicConnectionStringName = ExampleFormDbProperties.SqliteReadOnlyConnectionStringName;
                }
                else
                {
                    dynamicConnectionStringName = ExampleFormDbProperties.SqliteConnectionStringName;
                }
            }
            else
            {
                if (_selector.IsReadOnly())
                {
                    dynamicConnectionStringName = ExampleFormDbProperties.ReadonlyConnectionStringName;
                }
            }
            //var dynamicConnectionStringName = _selector.GetConnectionStringName();
            return await _defaultResolver.ResolveAsync(dynamicConnectionStringName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        [Obsolete]
        public string Resolve(string connectionStringName = null)
        {
            var dynamicConnectionStringName = ExampleFormDbProperties.ConnectionStringName;
            var dataType = _configurationRoot.GetValue<string>("ConnectionStrings:DataType", String.Empty);
            if (dataType == "sqlite")
            {
                if (_selector.IsReadOnly())
                {
                    dynamicConnectionStringName = ExampleFormDbProperties.SqliteReadOnlyConnectionStringName;
                }
                else
                {
                    dynamicConnectionStringName = ExampleFormDbProperties.SqliteConnectionStringName;
                }
            }
            else
            {
                if (_selector.IsReadOnly())
                {
                    dynamicConnectionStringName = ExampleFormDbProperties.ReadonlyConnectionStringName;
                }
            }
            //var dynamicConnectionStringName = _selector.GetConnectionStringName();
            return _defaultResolver.Resolve(dynamicConnectionStringName);
        }
    }

}
