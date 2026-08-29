using Microsoft.EntityFrameworkCore;

namespace ExampleApeart.EntityFrameworkCore
{
    /// <summary>
    /// 表示使用Sqlite数据库
    /// </summary>
    public class SqliteDb : AbsFilterDbContext<SqliteDb>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="isDesign"></param>
        public SqliteDb(DbContextOptions<SqliteDb> options, bool isDesign = false) : base(options, isDesign) { }
    }


    ///// <summary>
    ///// 
    ///// </summary>
    //public interface IMyDbContext : ICollectAuditDb
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public DbSet<UserInfo> UserInfo { get; set; }
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //public class ByDbContext : DbContext, IMyDbContext
    //{

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="db"></param>
    //    public ByDbContext(DbContextOptions<ByDbContext> db) : base(db)
    //    {

    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public DbSet<UserInfo> UserInfo { get; set; }
    //}


}
