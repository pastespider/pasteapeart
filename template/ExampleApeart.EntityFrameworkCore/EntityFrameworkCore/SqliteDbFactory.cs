using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using $ext_safeprojectname$.EntityFrameworkCore;

namespace $ext_safeprojectname$
{
    /// <summary>
    /// 
    /// </summary>
    public class SqliteDbFactory : IDesignTimeDbContextFactory<SqliteDb>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public SqliteDb CreateDbContext(string[] args)
        {
            //这个仅仅在EF的时候调用
            //System.Console.WriteLine($"{System.DateTime.Now} $ext_safeprojectname$DbContextFactory.CreateDbContext");
            System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var configuration = $ext_safeprojectname$EntityFrameworkCoreModule.BuildConfiguration(args);// BuildConfiguration();

            //var builder = new DbContextOptionsBuilder<ApeartDbContext>()
            //    .UseNpgsql(configuration.GetConnectionString($ext_safeprojectname$DbProperties.ConnectionStringName));
            var builder = new DbContextOptionsBuilder<SqliteDb>()
                    .UseSqlite(configuration.GetConnectionString($ext_safeprojectname$DbProperties.SqliteConnectionStringName));
            //    .ReplaceService<IMigrationsSqlGenerator, MyMigrationsSqlGenerator>();

            return new SqliteDb(builder.Options,true);
        }


    }
}
