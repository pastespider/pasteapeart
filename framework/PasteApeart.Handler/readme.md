
## 26.08.01
    1.MQ消费者的静默关闭

## 26.7.6
1引入LevelHandler主要针对多层级的查询，避免递归查询！

## 26.7.9
1.MQ消费者，有序推出

## 26.8.2
1.redis加HashGetIntAsync,HashGetLongAsync等

## 文件列表

```

│  JsonConfig.cs
│  PasteApeart.Handler.csproj
│  PasteApeartHandlerModule.cs
│  readme.md
│  SerilogCustomHandler.cs
│
├─apeart
│      ApeartExtensions.cs
│      AutoDependencyInjectionExtensions.cs
│      Check.cs
│      ClaimsPrincipalExtensions.cs
│      ILazyServiceProvider.cs
│      IScopedDependency.cs
│      ISingletonDependency.cs
│      ITransientDependency.cs
│      LazyServiceProvider.cs
│
├─common
│      CacheHandler.cs
│      ChannelHelper.cs
│      EventActionHandler.cs
│      ImageHelper.cs
│      LinqExtend.cs
│      ObjectExtend.cs
│      PublicString.cs
│
├─eventmodels
│      AbsEventModelExtensions.cs
│      EventDemoModelHandler.cs
│      IEventHandler.cs
│
├─modelhandler
│      LevelHandler.cs
│      UserHandler.cs
│
│
├─rabbitmodel
│      RabbitMQProducer.cs
│      RabbitMultiConsumerHandler.cs
│
└─redismodel
        HashKeyValue.cs
        IAppCache.cs
        MemoryClient.cs
        RedisClient.cs
        RedisConfig.cs

```

## 说明

