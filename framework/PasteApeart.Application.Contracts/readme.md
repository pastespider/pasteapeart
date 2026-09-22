## 26.09.12
	1.为InputSearchBase的各个字段引入[PasteQuery]


## 26.09.09
	1.为了配合Handler，主要是引入了PastePagedResult

## 26.09.03
	1.为了和ABP不冲突，把底层的Entity改成对应的PasteEntity，比如IEntity=>IPasteEntity,Entity<int>=>PasteEntity<int>以此类推

## 文件列表

```

│  EntityDto.cs
│  PasteApeart.Application.Contracts.csproj
│  PasteApeart.Application.Contracts.xml
│  PasteApeartApplicationContractsModule.cs
│  readme.md
│
├─basemodels
│      ExceptionCode.cs
│      ExceptionModel.cs
│      InputQueryUpdateState.cs
│      InputSearchBase.cs
│      InputSearchDate.cs
│      PasteCodeException.cs
│      RabbitConfig.cs
│
├─bin
│  ├─Debug
│  │  └─net8.0
│  └─Release
│      ├─net10.0
│      │      PasteApeart.Application.Contracts.deps.json
│      │      PasteApeart.Application.Contracts.dll
│      │      PasteApeart.Application.Contracts.pdb
│      │      PasteApeart.Application.Contracts.xml
│      │
│      └─net8.0
│              PasteApeart.Application.Contracts.deps.json
│              PasteApeart.Application.Contracts.dll
│              PasteApeart.Application.Contracts.pdb
│              PasteApeart.Application.Contracts.xml
│
├─eventmodels
│      AbsEventModel.cs
│      EventDemoModel.cs
│      IEventModel.cs
│
└

```

主要是IEventHandler的引入

异常信息模型等

## 添加ReadOnlyOption
用于标注当前主数据库链接名称和只读链接串名称

## 26.8.2
	1.添加InputQueryUpdateState