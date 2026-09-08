

## 26.09.03
	1.为了和ABP不冲突，把底层的Entity改成对应的PasteEntity，比如IEntity=>IPasteEntity,Entity<int>=>PasteEntity<int>以此类推


## 文件列表

```
D:.
│  PasteApeart.Domain.csproj
│  PasteApeart.Domain.xml
│  PasteApeartDbProperties.cs
│
├─apeart
│      CurrentUser.cs
│      Entity.cs
│      EntityKey.cs
│      IEntity.cs
│      IEntityKey.cs
│
├─auditmodels
│      ApAuditLog.cs
│      ApEntityChangeInfo.cs
│      ApPropertyChangeInfo.cs
│      AuditingConfigOptions.cs
│      DisableAuditingAttribute.cs
│
├─basicmodels
│      IUserIdEntiry.cs
```
主要是一些基本的Entity接口，便于后续的操作！


## 只读链接
配置到PasteApeartDbProperties中
CurrentMainStringName
CurrentReadStringName

