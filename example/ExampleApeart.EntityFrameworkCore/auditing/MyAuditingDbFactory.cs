using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExampleApeart
{
    /// <summary>
    /// 
    /// </summary>
    public class MyAuditingDbFactory : IDesignTimeDbContextFactory<MyAuditingDb>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public MyAuditingDb CreateDbContext(string[] args)
        {
            var configuration = ExampleApeartEntityFrameworkCoreModule.BuildConfiguration(args);// 
            var builder = new DbContextOptionsBuilder<MyAuditingDb>()
                    .UseSqlite(configuration.GetConnectionString(ExampleApeartDbProperties.AuditingConnectionStringName));
            return new MyAuditingDb(builder.Options);
        }
    }
}
