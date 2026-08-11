using Microsoft.EntityFrameworkCore;
using PasteApeart.auditmodels;

namespace ExampleApeart
{
    /// <summary>
    /// 
    /// </summary>
    //[ConnectionStringName(PasteSpiderDbProperties.AuditMysql)]
    public class MyAuditingDb : DbContext, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public MyAuditingDb(DbContextOptions<MyAuditingDb> options) : base(options)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApAuditLog>(b =>
            {
                b.ToTable("ApAuditLog");
                //b.ConfigureByConvention();
            });

            builder.Entity<ApEntityChangeInfo>(b =>
            {
                b.ToTable("ApEntityChangeInfo");
                //b.ConfigureByConvention();
            });

            builder.Entity<ApPropertyChangeInfo>(b =>
            {
                b.ToTable("ApPropertyChangeInfo");
                //b.ConfigureByConvention();
            });

        }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<ApAuditLog> ApAuditLog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<ApEntityChangeInfo> ApEntityChangeInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<ApPropertyChangeInfo> ApPropertyChangeInfo { get; set; }

    }
}
