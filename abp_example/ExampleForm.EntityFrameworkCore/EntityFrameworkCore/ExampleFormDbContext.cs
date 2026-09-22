using ExampleForm.testmodels;
using ExampleForm.toolmodels;
using ExampleForm.usermodels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PasteApeart.Domain.Entity;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Users;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    [ConnectionStringName(ExampleFormDbProperties.ConnectionStringName)]
    public class ExampleFormDbContext : AbpDbContext<ExampleFormDbContext>, IExampleFormDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * public DbSet<Question> Questions { get; set; }
         */

        private int CurrentUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="currentUser"></param>
        public ExampleFormDbContext(DbContextOptions<ExampleFormDbContext> options, ICurrentUser currentUser = null)
            : base(options)
        {
            if (currentUser != null)
            {
                var userStr = currentUser.FindClaimValue("UserId");
                if (userStr != null)
                {
                    if (int.TryParse(userStr, out var uid))
                    {
                        CurrentUserId = uid;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureExampleForm();


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.AddInterceptors(new AutoWriteInterceptor(_currentUser));//注入，自动填写日期
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        protected override void HandleExtraPropertiesOnSave(EntityEntry entry)
        {
            base.HandleExtraPropertiesOnSave(entry);
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is IUserIdEntiry userEntity)
                {
                    if (userEntity.UserId == 0)
                    {
                        //未完待续，上面要获取值 要先赋值
                        userEntity.UserId = CurrentUserId;
                    }
                }
            }
        }

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