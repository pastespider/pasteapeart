//using Microsoft.EntityFrameworkCore;
//using PasteApeart.EntityFrameworkCore;
//using ExampleApeart.usermodels;

//namespace ExampleApeart.EntityFrameworkCore
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public interface ISqlDbContext : ICollectAuditDb, IDisposable
//    {

//        /// <summary>
//        /// 权限列表
//        /// </summary>
//        public DbSet<RoleInfo> RoleInfo { get; set; }

//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class AbcDbContext : DbContext, ISqlDbContext
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="options"></param>
//        public AbcDbContext(DbContextOptions<AbcDbContext> options) : base(options)
//        {

//        }


//        /// <summary>
//        /// 权限列表
//        /// </summary>
//        public DbSet<RoleInfo> RoleInfo { get; set; }
//    }
//}
