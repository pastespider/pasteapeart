using ExampleApeart.usermodels;
using Microsoft.EntityFrameworkCore;
using PasteApeart.EntityFrameworkCore;

namespace ExampleApeart.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExampleApeartDbContext : ICollectAuditDb, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        int CurrentUserId { get; set; }

        /// <summary>
        /// 假设需要基于查询获取
        /// </summary>
        IQueryable<int> CurrentQueryableUserId { get; set; }

        /// <summary>
        /// 是否要过滤
        /// </summary>
        bool FilterQuery { get; set; }

        ///// <summary>
        ///// 基于当前用户查询得有权限对象
        ///// </summary>
        //IQueryable<string> QueryAdvertiserId { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public DbSet<UserInfo> UserInfo { get; set; }
        /// <summary>
        /// 权限信息
        /// </summary>
        public DbSet<RoleInfo> RoleInfo { get; set; }
        /// <summary>
        /// 角色信息
        /// </summary>
        public DbSet<GradeInfo> GradeInfo { get; set; }
        /// <summary>
        /// 角色权限
        /// </summary>
        public DbSet<GradeRole> GradeRole { get; set; }

        /// <summary>
        /// 用户绑定角色 解绑的直接删除数据
        /// </summary>
        public DbSet<UserGrade> UserGrade { get; set; }

    }


}
