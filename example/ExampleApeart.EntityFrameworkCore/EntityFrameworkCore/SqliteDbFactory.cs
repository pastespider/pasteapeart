using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ExampleApeart.EntityFrameworkCore;

namespace ExampleApeart
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
            //System.Console.WriteLine($"{System.DateTime.Now} ExampleApeartDbContextFactory.CreateDbContext");
            System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var configuration = ExampleApeartEntityFrameworkCoreModule.BuildConfiguration(args);// BuildConfiguration();

            //var builder = new DbContextOptionsBuilder<ApeartDbContext>()
            //    .UseNpgsql(configuration.GetConnectionString(ExampleApeartDbProperties.ConnectionStringName));
            var builder = new DbContextOptionsBuilder<SqliteDb>()
                    .UseSqlite(configuration.GetConnectionString(ExampleApeartDbProperties.SqliteConnectionStringName));
            //    .ReplaceService<IMigrationsSqlGenerator, MyMigrationsSqlGenerator>();

            return new SqliteDb(builder.Options,true);
        }


    }
}
