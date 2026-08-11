using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace $ext_safeprojectname$
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
            var configuration = $ext_safeprojectname$EntityFrameworkCoreModule.BuildConfiguration(args);// 
            var builder = new DbContextOptionsBuilder<MyAuditingDb>()
                    .UseSqlite(configuration.GetConnectionString($ext_safeprojectname$DbProperties.AuditingConnectionStringName));
            return new MyAuditingDb(builder.Options);
        }
    }
}
