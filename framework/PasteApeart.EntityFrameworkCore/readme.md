## 项目说明

## 文件列表


```

│  PasteApeart.EntityFrameworkCore.csproj
│  PasteApeart.EntityFrameworkCore.xml
│  readme.md
│
├─auditing
│      AuditCollection.cs
│      AuditMetadataCache.cs
│      MyAuditingDb.cs
│      MyAuditingDbFactory.cs
│
├─bin
│  ├─Debug
│  │  └─net8.0
│  └─Release
│      ├─net10.0
│      │      PasteApeart.Domain.dll
│      │      PasteApeart.Domain.pdb
│      │      PasteApeart.Domain.xml
│      │      PasteApeart.EntityFrameworkCore.deps.json
│      │      PasteApeart.EntityFrameworkCore.dll
│      │      PasteApeart.EntityFrameworkCore.pdb
│      │      PasteApeart.EntityFrameworkCore.xml
│      │
│      └─net8.0
│              PasteApeart.Domain.dll
│              PasteApeart.Domain.pdb
│              PasteApeart.Domain.xml
│              PasteApeart.EntityFrameworkCore.deps.json
│              PasteApeart.EntityFrameworkCore.dll
│              PasteApeart.EntityFrameworkCore.pdb
│              PasteApeart.EntityFrameworkCore.xml
│
├─EntityFrameworkCore
│      CollectAuditDb.cs
│      ICollectAuditDb.cs
│
├

```

# 审计日志和过滤

## PasteApeartModelBuilderConfigurationOptions

```csharp

    /// <summary>
    /// 配置表前缀 Schema等 按需配置
    /// </summary>
    public class PasteApeartModelBuilderConfigurationOptions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablePrefix"></param>
        /// <param name="schema"></param>
        public PasteApeartModelBuilderConfigurationOptions(
            string tablePrefix = "",
            string schema = null)
        {
            TablePrefix = tablePrefix;
            Schema = schema;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TablePrefix { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Schema { get; set; } = null;
    }

```

## PasteApeartDbContextModelCreatingExtensions

```csharp

    /// <summary>
    /// 
    /// </summary>
    public static class PasteApeartDbContextModelCreatingExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        public static void ConfigurePasteApeart(
            this ModelBuilder builder,
            Action<PasteApeartModelBuilderConfigurationOptions> optionsAction = null)
        {
            var options = new PasteApeartModelBuilderConfigurationOptions(
                PasteApeartDbProperties.DbTablePrefix,
                PasteApeartDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            /* Configure all entities here. Example:

            builder.Entity<Question>(b =>
            {
                //Configure table & schema name
                b.ToTable(options.TablePrefix + "Questions", options.Schema);
            
                //b.ConfigureByConvention();
            
                //Properties
                b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);
                
                //Relations
                b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

                //Indexes
                b.HasIndex(q => q.CreationTime);
            });
            */

            // *UserInfo*
            builder.Entity<UserInfo>(b =>
            {
                b.ToTable(options.TablePrefix + "UserInfo", options.Schema);
                //b.ConfigureByConvention();
            });


        }
    }

```

## IPasteApeartDbContext

```csharp

    /// <summary>
    /// 
    /// </summary>
    public interface IPasteApeartDbContext : ICollectAuditDb, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        int CurrentUserId { get; set; }

        /// <summary>
        /// 假设需要基于查询获取
        /// </summary>
        IQueryable<int> UserIds { get; set; }

        /// <summary>
        /// 是否要过滤
        /// </summary>
        bool FilterQuery { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public DbSet<UserInfo> UserInfo { get; set; }


    }

```

## IFilterQueryDbContext

```csharp


    /// <summary>
    /// 
    /// </summary>
    public abstract class IFilterQueryDbContext<T> : CollectAuditDb<T>, IPasteApeartDbContext where T : IFilterQueryDbContext<T>
    {
        /// <summary>
        /// 在中间件赋值
        /// </summary>
        public int CurrentUserId { get; set; } = 0;

        /// <summary>
        /// 假设需要基于查询获取 在中间件赋值
        /// </summary>
        public IQueryable<int> UserIds { get; set; }

        /// <summary>
        /// 是否要过滤 在中间件赋值
        /// </summary>
        public bool FilterQuery { get; set; } = false;

        /// <summary>
        /// 设计模式 add-migration 
        /// </summary>
        private bool IsDesignModel { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="isDesign"></param>
        public IFilterQueryDbContext(DbContextOptions<T> options, bool isDesign = false) : base(options)
        {
            UserIds = new int[] { 0 }.AsQueryable();
            IsDesignModel = isDesign;
            if (IsDesignModel)
            {
                return;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigurePasteApeart();
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
            if (UserIds == null || !FilterQuery)
            {
                return;
            }
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IUserIdEntiry).IsAssignableFrom(entityType.ClrType))
                {
                    var entityParam = Expression.Parameter(entityType.ClrType, "e");
                    var userIdProperty = Expression.Property(entityParam, nameof(IUserIdEntiry.UserId));

                    // 关键：获取 UserIds 的查询表达式
                    var userIdsQuery = Expression.Property(Expression.Constant(this), nameof(UserIds));

                    // 使用 Any 方法：UserIds.Any(id => id == e.UserId)
                    var anyMethod = typeof(Queryable)
                        .GetMethods()
                        .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(int));

                    var idParam = Expression.Parameter(typeof(int), "id");
                    var equality = Expression.Equal(idParam, userIdProperty);
                    var anyLambda = Expression.Lambda(equality, idParam);

                    // 构建 UserIds.Any(id => id == e.UserId)
                    var subquery = Expression.Call(
                        anyMethod,
                        userIdsQuery,           // IQueryable<int>
                        Expression.Quote(anyLambda) // Lambda 表达式
                    );

                    // 加上开关
                    var filterSwitch = Expression.Property(Expression.Constant(this), nameof(FilterQuery));
                    var notFilter = Expression.Not(filterSwitch);
                    var body = Expression.OrElse(notFilter, subquery);

                    var lambda = Expression.Lambda(body, entityParam);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        #endregion

        /// <summary>
        /// 用户信息
        /// </summary>
        public DbSet<UserInfo> UserInfo { get; set; }



    }

```

## SqliteDb

```
    /// <summary>
    /// 表示使用Sqlite数据库
    /// </summary>
    public class SqliteDb : IFilterQueryDbContext<SqliteDb>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="isDesign"></param>
        public SqliteDb(DbContextOptions<SqliteDb> options, bool isDesign = false) : base(options, isDesign) { }
    }

```

## SqliteDbFactory

```

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

            return new SqliteDb(builder.Options, true);
        }


    }

```

## 读写分离如何引入

1.
```csharp


//下面这行不要... .. .
    services.AddScoped<IConnectionStringSelector, ConnectionStringSelector>();

    services.AddSingleton<ReadWriteSplitInterceptor>();


    services.AddDbContext<SqliteDb>((sp, options) =>
    {
        options.UseSqlite(configuration.GetConnectionString(PasteApeartDbProperties.SqliteConnectionStringName));
        //读写分离 如果不需要那么用上面的，或者下面2行注释掉即可
        var rwp = sp.GetRequiredService<ReadWriteSplitInterceptor>();
        options.AddInterceptors(rwp);
    });

```

2.
在需要的地方标注特性[UseReadOnlyConnection]

3.
需要在中间件中读取特性，如果有，则进行标记
```
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<UseReadOnlyConnectionAttribute>() != null)
            {
                context.Items["IsReadOnly"] = true;
            }

```


