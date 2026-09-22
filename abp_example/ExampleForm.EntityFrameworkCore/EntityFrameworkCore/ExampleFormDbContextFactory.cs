using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    public class ExampleFormDbContextFactory : IDesignTimeDbContextFactory<ExampleFormDbContext>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ExampleFormDbContext CreateDbContext(string[] args)
        {
            //这个仅仅在EF的时候调用
            //System.Console.WriteLine($"{System.DateTime.Now} ExampleFormDbContextFactory.CreateDbContext");
            System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var configuration = ExampleFormEntityFrameworkCoreModule.BuildConfiguration(args);// BuildConfiguration();

            var builder = new DbContextOptionsBuilder<ExampleFormDbContext>()
                .UseNpgsql(configuration.GetConnectionString(ExampleFormDbProperties.ConnectionStringName));
            //    .ReplaceService<IMigrationsSqlGenerator, MyMigrationsSqlGenerator>();

            return new ExampleFormDbContext(builder.Options);
        }


    }
}
