using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PasteApeart.EntityFrameworkCore;

namespace PasteApeart
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
            //System.Console.WriteLine($"{System.DateTime.Now} PasteApeartDbContextFactory.CreateDbContext");
            System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var configuration = PasteApeartEntityFrameworkCoreModule.BuildConfiguration(args);// BuildConfiguration();

            //var builder = new DbContextOptionsBuilder<ApeartDbContext>()
            //    .UseNpgsql(configuration.GetConnectionString(PasteApeartDbProperties.ConnectionStringName));
            var builder = new DbContextOptionsBuilder<SqliteDb>()
                    .UseSqlite(configuration.GetConnectionString(PasteApeartDbProperties.SqliteConnectionStringName));
            //    .ReplaceService<IMigrationsSqlGenerator, MyMigrationsSqlGenerator>();

            return new SqliteDb(builder.Options,true);
        }


    }
}
