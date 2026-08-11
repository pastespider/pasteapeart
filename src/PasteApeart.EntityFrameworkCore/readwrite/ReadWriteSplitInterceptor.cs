using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="config"></param>
        public ReadWriteSplitInterceptor(IHttpContextAccessor serviceProvider, IConfiguration config)
        {

            _httpContextAccessor = serviceProvider;
            _configuration = config;
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
                Console.WriteLine("配置只读链接串");
                //connection.ConnectionString = _config.GetConnectionString("ReadOnly");
                connection.ConnectionString = _configuration.GetConnectionString(PasteApeartDbProperties.SqliteReadOnlyConnectionStringName);
            }
            else
            {
                connection.ConnectionString = _configuration.GetConnectionString(PasteApeartDbProperties.SqliteConnectionStringName);
            }
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
                Console.WriteLine("配置只读链接串");
                connection.ConnectionString = _configuration.GetConnectionString(PasteApeartDbProperties.SqliteReadOnlyConnectionStringName);
                //connection.ConnectionString = _config.GetConnectionString("ReadOnly");
            }
            else
            {
                connection.ConnectionString = _configuration.GetConnectionString(PasteApeartDbProperties.SqliteConnectionStringName);
            }
            return base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
        }

        //使用案例 先注入ReadWriteSplitInterceptor 应该是Icope
    }

}
