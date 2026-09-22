using ExampleForm.EntityFrameworkCore;

namespace ExampleForm.HttpApi.Host.efhandler
{



    /// <summary>
    /// 
    /// </summary>
    public class DbReadonlyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConnectionStringSelector _selector;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="selector"></param>
        public DbReadonlyMiddleware(RequestDelegate next, IConnectionStringSelector selector)
        {
            _next = next;
            _selector = selector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            //string connectionStringName = ExampleFormDbProperties.SqliteConnectionStringName;
            if (endpoint?.Metadata.GetMetadata<UseReadOnlyConnectionAttribute>() != null)
            {
                _selector.SetReadOnly(true);
                //connectionStringName = ExampleFormDbProperties.SqliteReadOnlyConnectionStringName;
            }
            //_selector.SetConnectionStringName(connectionStringName);
            await _next(context);
        }

    }

}
