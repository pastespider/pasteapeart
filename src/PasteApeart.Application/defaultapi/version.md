

## 2025.11.23
	1.添加对Include的支持，子表需要对应的Dto，比如TableAddDto，则对应的子表AddDto

	
## 2025.11.09
	1.添加对新增更新后立马可见的支持，就是新增后返回xxxListDto数据，渲染到表格UI中，这个需要后端也支持，DefaultAppService默认返回XXXListDto了！


## 2025.11.28
	1.优化FirstListDto的返回调用，免去再次查找entityType,由于一般是新增编辑后，所以listDtoType的查找难免!


## 组件化
```csharp

public class TypeCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly Func<Type[]> _typeScanner;

    // 构造函数接收扫描委托
    public TypeCacheService(IMemoryCache memoryCache, Func<Type[]> typeScanner)
    {
        _memoryCache = memoryCache;
        _typeScanner = typeScanner;
    }

    public Type[] ListDomainTypes()
    {
        return _memoryCache.GetOrCreate("domain.types", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(450);
            return _typeScanner(); // 调用委托
        });
    }
}

// 使用方注册：
services.AddSingleton(sp => 
    new TypeCacheService(
        sp.GetRequiredService<IMemoryCache>(),
        () => typeof(RoleInfo).Assembly.GetTypes()
    ));

```

