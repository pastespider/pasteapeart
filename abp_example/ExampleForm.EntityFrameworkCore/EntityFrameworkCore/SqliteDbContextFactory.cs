using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    public class SqliteDbContextFactory : IDesignTimeDbContextFactory<SqliteDbContext>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public SqliteDbContext CreateDbContext(string[] args)
        {
            //这个仅仅在EF的时候调用
            //System.Console.WriteLine($"{System.DateTime.Now} ExampleFormDbContextFactory.CreateDbContext");
            System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var configuration = ExampleFormEntityFrameworkCoreModule.BuildConfiguration(args);// BuildConfiguration();

            var builder = new DbContextOptionsBuilder<SqliteDbContext>()
                .UseSqlite(configuration.GetConnectionString(ExampleFormDbProperties.SqliteConnectionStringName));
            //    .ReplaceService<IMigrationsSqlGenerator, MyMigrationsSqlGenerator>();

            return new SqliteDbContext(builder.Options);
        }


    }
}
