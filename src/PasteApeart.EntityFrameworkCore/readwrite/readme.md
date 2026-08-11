

## 如何引入

```csharp


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

## 标注特性
然后再需要的地方标注对应特性即可！
