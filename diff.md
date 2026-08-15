# PasteApeart和PasteForm的区别，看这一篇就够了

> 如果说PasteForm是All in Dto
> 那么PasteApeart就是All in Dto and Handler

所以，PasteApeart是PasteForm的升级版！

## 项目模块

如果你使用我提供的项目模板进行项目创建，比如命名为Project1.则会生成以下子项目
```
Project1.Domain
Project1.EntityFrameworkCore
Project1.Application.Contracts
Project1.Handler
Project1.Applciation
Project1.HttpApi.Host
```


|模块|说明|
|--|--|
|.Domain|存放Entity和Enum等，表示数据库的表|
|.Application.Contracts|Dto存放，EventModel存放，不附带逻辑的Model|
|.EntityframeworkCore|数据库EF模块，像多租户，默认填充，审计日志存储等都在这|
|.Handler|业务逻辑存放处，IEventHandler实现，消息队列，等|
|.Application|主要是CRUD的接口存放|
|.HttpApi.Host|主要是非CRUD的接口，还可能涉及服务端渲染的UI等|



## PasteForm

严格来说，PasteForm有2个版本，第一个版本是PasteForm.Helper(PasteFormHelper)等，后面由于PasteApeart的引入，干脆再升级了一个PasteForm

### PasteForm.Helper

>最早之前是PasteFormHelper,后面改成了PasteForm.Helper
这个dll的主要作用就是帮助你吧Dto的信息转化成元数据(VoloModelInfo)的
那时候的PasteForm也就只有这个东西，

### PasteForm.2

>这个版本是在PasteApeart出来后，和PasteApeart相互对应的，区别主要在于PasteForm基于ABP，而PasteApeart是非基于ABP的！

**PasteForm.2和PasteApeart的项目，在使用Dto转元数据都使用的是PasteApeart.Helper**

#### PasteForm.Helper和PasteApeart.Helper的区别就是namespace不一样，其他一样，所以你后续都可以改成PasteApeart.Helper,然后把命名空间替换一下即可！

**注意后续PasteForm.Helper将不再维护，后续全部引用PasteApeart.Helper即可**

## PasteApeart

这个的推出，主要是之前的我的项目基于ABP的，启动太慢了，所以把不需要的都丢弃后，按照ABP的规则自己写一个项目模板具体的你可以查看PasteApeart项目中的example案例

## PasteForm和PasteApeart对比

|模块|PasteForm.1|PasteForm.2|PasteApeart|
|:--|:--|:--|:--|
|Helper|PasteFormHelper|PasteApeart.Helper|PasteApeart.Helper|
|Domain||PasteForm.Domain|PasteApeart.Domain|
|Dto||PasteForm.Application.Contracts|PasteApeart.Application.Contracts|
|EF|||PasteApeart.Entityframework|
|Handler||PasteForm.Handler|PasteApeart.Handler|
|Application||PasteForm.Application|PasteApeart.Application|

|模块|说明|
|--|--|
|Helper|帮助项目把Dto转化成元数据|
|EF|主要引入比如审计日志，多租户，默认填充等|
|Dto|主要引入一些默认的Model,比如状态变更InputUpdateStateModel，查询默认InputQueryBase，异常PasteCodeException等|
|Handler|实现默认的redis,rabbitmq,channel,IEventHandler|
|Application|主要引入DefaultAppService,也就是你的表不需要写默认接口,是这个框架的核心！！！|

### DefaultAppService

为所有的Entity提供的一个对外的接口，这是一个很重要的CRUD实现方式!默认管理端的页面就是通过以下接口通讯的！

|Method|Api|说明|入参|
|--|--|--|--|
|GET|/api/app/{entityName}/readAddModel|获取新增元数据||
|GET|/api/app/{entityName}/{id}/readUpdateModel|获取更新元数据||
|GET|/api/app/{entityName}/{id}/readDetailModel|获取详细元数据||
|GET|/api/app/{entityName}/readListModel|获取列表元数据||
|GET|/api/app/{entityName}/page|获取列表|InputQueryEntityName|
|POST|/api/app/{entityName}/item|创建|EntityNameAddDto|
|POST|/api/app/{entityName}/updateItem|更新|EntityNameUpdateDto|
|POST|/api/app/{entityName}/updateState|状态更新|InputQueryUpdateState|
|POST|/api/app/{entityName}/{id}/remove|删除||


### 什么时候不用DefaultAppService

假设你得UserInfo的获取列表很复杂，你要自己写
那么你就应该在.Application的项目下，对应模块文件夹下的UserInfoAppService.cs
中写入以下函数
```csharp
public async Task<PagedResultDto<UserInfoListDto>> Page([FromQuery] InputQueryUserInfo input)
{
    //你自己实现业务查询逻辑
}
```

这样路由/api/app/userInfo/page就会覆盖DefaultAppService默认提供的/api/app/{entityName}/page,因为DefaultAppService的Order(排序)更大，小的覆盖大的！










