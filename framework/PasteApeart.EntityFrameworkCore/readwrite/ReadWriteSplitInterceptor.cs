using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Data.Common;

namespace PasteApeart.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadWriteSplitInterceptor : DbConnectionInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ReadOnlyOption _config;

        /*       
            services.AddSingleton<ReadWriteSplitInterceptor>();
            services.AddDbContext<SqliteDb>((sp, options) =>
            {
                options.UseSqlite(configuration.GetConnectionString(PasteApeartDbProperties.SqliteConnectionStringName));
                //读写分离 如果不需要那么用上面的，或者下面2行注释掉即可
                var rwp = sp.GetRequiredService<ReadWriteSplitInterceptor>();
                options.AddInterceptors(rwp);
            });
         */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="config"></param>
        /// <param name="readConfig"></param>
        public ReadWriteSplitInterceptor(IHttpContextAccessor serviceProvider, IConfiguration config,IOptions<ReadOnlyOption> readConfig)
        {

            _httpContextAccessor = serviceProvider;
            _configuration = config;
            _config = readConfig.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
        {
            var isReadOnly = _httpContextAccessor.HttpContext?.Items["IsReadOnly"] as bool? ?? false;
            if (isReadOnly)
            {
                connection.ConnectionString = _configuration.GetConnectionString(_config.CurrentReadStringName);
            }
            else
            {
                connection.ConnectionString = _configuration.GetConnectionString(_config.CurrentMainStringName);
            }
            //var connectName = _httpContextAccessor.HttpContext?.Items["ConnectName"] as string ?? string.Empty;
            //if (!string.IsNullOrEmpty(connectName))
            //{
            //    connection.ConnectionString = _configuration.GetConnectionString(connectName);
            //}
            return base.ConnectionOpening(connection, eventData, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
        {
            var isReadOnly = _httpContextAccessor.HttpContext?.Items["IsReadOnly"] as bool? ?? false;
            if (isReadOnly)
            {
                connection.ConnectionString = _configuration.GetConnectionString(_config.CurrentReadStringName);
            }
            else
            {
                connection.ConnectionString = _configuration.GetConnectionString(_config.CurrentMainStringName);
            }
            //var connectName = _httpContextAccessor.HttpContext?.Items["ConnectName"] as string ?? string.Empty;
            //if (!string.IsNullOrEmpty(connectName))
            //{
            //    connection.ConnectionString = _configuration.GetConnectionString(connectName);
            //}
            return base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
        }

        //使用案例 先注入ReadWriteSplitInterceptor 应该是Icope
    }

}
