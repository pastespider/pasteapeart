## 版本说明

## 26.9.6
	1.PasteDefaultAppService添加DirectPageAsync，这样你自己的接口可以直接复用 表示获取某一页的数据
	2.AppService转API的时候，只有结尾的Async才替换，var actionName = action.ActionName.EndsWith("Async") ? action.ActionName.Replace("Async", "") : action.ActionName;

### 26.8.5
	1.取消IPasteDefaultHelper的静态函数，全部采用非静态模式，便于用户端二开
	2.取消Func的外载模式，需要的话，你就使用重写PasteDefaultAppService,或者继承对应接口，全部自己实现
	3.PasteDefaultHelper的构造改成直接注入types，不再是函数

### 26.8.3
	1.移除接口中的静态函数，放具体实现类中去

### 26.8.2
	1.PasteApeartDefaultHelper改名为PasteDefaultHelper同时提取接口
	2.PasteApeartDefaultAppService改名为PasteDefaultAppService同时提取接口

## 文件列表

```

│  DefaultAppService.cs
│  DefaultHelper.cs
│  PasteApeart.Application.csproj
│  PasteApeart.Application.xml
│  PasteThrowExceptionAttribute.cs
│  readme.md
│
├─apeart
│      ApeartAppServiceAutoConvention.cs
│      AppServiceControllerFeatureProvider.cs
│      IAppService.cs
│      PagedResultDto.cs
│      PasteResultDto.cs
│

```

