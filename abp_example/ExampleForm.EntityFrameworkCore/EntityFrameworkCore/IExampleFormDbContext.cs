using ExampleForm.testmodels;
using ExampleForm.toolmodels;
using ExampleForm.usermodels;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExampleFormDbContext : IEfCoreDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * DbSet<Question> Questions { get; }
         */
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

        /// <summary>
        /// 测试表 用于测试CURD的表
        /// </summary>
        public DbSet<TestTable> TestTable { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DbSet<IncludeTable> IncludeTable { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DbSet<MyTable> MyTable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<GuidTest1> GuidTest1 { get; set; }

        /// <summary>
        /// 测试UINT
        /// </summary>
        public DbSet<UintTest1> UintTest1 { get; set; }

        /// <summary>
        /// 测试表
        /// </summary>
        public DbSet<ParentTable> ParentTable { get; set; }

        /// <summary>
        /// 图片信息
        /// </summary>
        public DbSet<UploadImageInfo> UploadImageInfo { get; set; }

        /// <summary>
        /// 测试链接表
        /// </summary>
        public DbSet<TestLinkInclude> TestLinkInclude { get; set; }

    }

}