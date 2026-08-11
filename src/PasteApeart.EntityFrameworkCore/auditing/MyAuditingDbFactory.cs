using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PasteApeart
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
            var configuration = PasteApeartEntityFrameworkCoreModule.BuildConfiguration(args);// 
            var builder = new DbContextOptionsBuilder<MyAuditingDb>()
                    .UseSqlite(configuration.GetConnectionString(PasteApeartDbProperties.AuditingConnectionStringName));
            return new MyAuditingDb(builder.Options);
        }
    }
}
