using ExampleApeart.Domain;
using ExampleApeart.usermodels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PasteApeart;
using PasteApeart.EntityFrameworkCore;

namespace ExampleApeart.EntityFrameworkCore
{



    /// <summary>
    /// 
    /// </summary>
    public abstract class AbsFilterDbContext<T> : CollectAuditDb<T>, IExampleApeartDbContext where T : AbsFilterDbContext<T>
    {
        //public class ExampleApeartDbContext : AbpDbContext<ExampleApeartDbContext>, IExampleApeartDbContext

        /* Add DbSet for each Aggregate Root here. Example:
         * public DbSet<Question> Questions { get; set; }
         */

        /// <summary>
        /// 
        /// </summary>
        public int CurrentUserId { get; set; } = 0;

        //private bool FilterUserId { get; set; } = true;

        /// <summary>
        /// 当前权限用户集
        /// </summary>
        public IQueryable<int> QueryUserId { get; set; }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 是否要过滤
        /// </summary>
        public bool EnableFilter { get; set; } = true;

        ///// <summary>
        ///// 基于当前用户查询得有权限对象
        ///// </summary>
        //public IQueryable<string> QueryAdvertiserId { get; set; }

        /// <summary>
        /// 设计模式 add-migration
        /// </summary>
        private bool IsDesignModel { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="isDesign"></param>
        public AbsFilterDbContext(DbContextOptions<T> options, bool isDesign = false) : base(options)
        {
            IsDesignModel = isDesign;
            if (IsDesignModel)
            {
                return;
            }

            var httpContextAccessor = this.GetService<IHttpContextAccessor>();
            if (httpContextAccessor != null)
            {
                var http = httpContextAccessor?.HttpContext;
                if (http != null)
                {
                    var token = http.User.FindFirst("Token")?.Value;
                    //这里可以基于token等判定是否过滤
                    if (http.Items["CurrentUser"] is CurrentUser ucr)
                    {
                        CurrentUserId = ucr.Id;
                        Console.WriteLine("获取到用户" + CurrentUserId);
                    }
                }
            }

            if (CurrentUserId > 1)
            {
                EnableFilter = true;
                //这里模拟查询 实际情况可以用类似this.UserGrade.Where(x => x.GradeId == CurrentUserId).Select(x => x.UserId);
                var uid = new int[] { 1, 2 };
                QueryUserId = uid.AsQueryable();
            }
            else
            {
                EnableFilter = false;
                var uid = new int[] { 10, 20, 30 };
                QueryUserId = uid.AsQueryable();
            }
            //Console.WriteLine($"Current UserId:{CurrentUserId} Filter:{FilterQuery}");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigureExampleApeart();
            if (!IsDesignModel)
            {
                //方式三
                FilterIQueryable(builder);
            }
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

        #region 方式三，实现IQueryable<int>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void FilterIQueryable(ModelBuilder modelBuilder)
        {
            if (QueryUserId == null || !EnableFilter)
            {
                return;
            }
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                #region  实现类似多个个多租户的过滤 !EnableFilter || QueryGroupId.Any(x.UserId)
                //// !EnableFilter || QueryGroupId.Any(x.UserId)
                //if (typeof(IUserIdEntiry).IsAssignableFrom(entityType.ClrType))
                //{
                //    var entityParam = Expression.Parameter(entityType.ClrType, "e");
                //    var userIdProperty = Expression.Property(entityParam, nameof(IUserIdEntiry.UserId));
                //    var userIdType = userIdProperty.Type;
                //    // 关键：获取 UserIds 的查询表达式
                //    var userIdsQuery = Expression.Property(Expression.Constant(this), nameof(QueryUserId));

                //    // 使用 Any 方法：UserIds.Any(id => id == e.UserId)
                //    var anyMethod = typeof(Queryable)
                //        .GetMethods()
                //        .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                //        .MakeGenericMethod(userIdType);

                //    var idParam = Expression.Parameter(userIdType, "id");
                //    var equality = Expression.Equal(idParam, userIdProperty);
                //    var anyLambda = Expression.Lambda(equality, idParam);

                //    // 构建 UserIds.Any(id => id == e.UserId)
                //    var subquery = Expression.Call(
                //        anyMethod,
                //        userIdsQuery,           // IQueryable<int>
                //        Expression.Quote(anyLambda) // Lambda 表达式
                //    );

                //    // 加上开关
                //    var filterSwitch = Expression.Property(Expression.Constant(this), nameof(EnableFilter));
                //    var notFilter = Expression.Not(filterSwitch);
                //    var body = Expression.OrElse(notFilter, subquery);

                //    var lambda = Expression.Lambda(body, entityParam);
                //    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                //}
                #endregion

                #region 实现类似单个多租户的过滤 !EnableFilter || x.UserId == UserId
                //// !EnableFilter || x.UserId == UserId
                //if (typeof(IUserIdEntiry).IsAssignableFrom(entityType.ClrType))
                //{
                //    var idProperty = entityType.ClrType.GetProperty(nameof(IUserIdEntiry.UserId));
                //    var keyType = idProperty.PropertyType; // 这里应为 int

                //    var entityParam = Expression.Parameter(entityType.ClrType, "e");
                //    var entityUserIdProperty = Expression.Property(entityParam, nameof(IUserIdEntiry.UserId));

                //    // 当前对象上的 UserId 属性，例如 this.UserId
                //    var currentUserIdProperty = Expression.Property(Expression.Constant(this), nameof(UserId));

                //    // 开关: !EnableFilter
                //    var filterSwitch = Expression.Property(Expression.Constant(this), nameof(EnableFilter));
                //    var notFilter = Expression.Not(filterSwitch);

                //    // 条件: x.UserId == UserId
                //    var userEqualsCurrent = Expression.Equal(entityUserIdProperty, currentUserIdProperty);

                //    // 组合条件: !EnableFilter || x.UserId == UserId
                //    var body = Expression.OrElse(notFilter, userEqualsCurrent);

                //    var lambda = Expression.Lambda(body, entityParam);
                //    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);

                //    continue;
                //}
                #endregion

            }
        }

        #endregion

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
