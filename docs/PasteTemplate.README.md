# 📋 PasteTemplate — PasteForm 框架 v1 官方示例模板

> **【202X 年 .NET 低代码革命的起点】**
> 
> 当别人还在用三层架构写 Controller → Service → Repository → DAL 写吐的时候，
> PasteForm **第一次**把「DTO 驱动前后端 + 自动 API + 通用 CRUD」带到了 .NET 世界。
> 
> PasteTemplate 是这套革命性框架的 **社区官方示例**，是 PasteApeart（v2）的前身与基石。
> 
> 👉 **新项目建议直接用 [PasteApeart (ApeartTemplate)](./apeart/README.md) 升级版**，本仓库主要用于历史对照与学习 PasteForm v1 体系。

---

## ⚡ 它的历史地位：第一次让 .NET 开发者感受到「30 秒出模块」

| 工作项 | 传统 .NET + 三层架构 | PasteForm v1 (PasteTemplate) |
|---|---|---|
| 写实体 | ✅ 20 行 | ✅ 20 行 |
| 写 DTO × 3 | ✅ 60 行 | ✅ 60 行（**首次实现「贴特性 = 驱动前端 UI」**） |
| 写 Service 接口 + 实现 + CRUD | ✅ 400 行 | ❌ 不用写 |
| 写 Controller + Action | ✅ 250 行 | ❌ 不用写（ABP AutoAPI 自动生成） |
| 写前端列表/表单页面 | ✅ 600 行（Vue/React） | ❌ 不用写（VoloModelInfo + PasteForm 前端自动渲染） |
| 写 SQL / EF 迁移 | ✅ 100 行 | ✅ 一键生成（PasteBuilder 插件） |
| **总代码** | **~1430 行** | **80 行** |
| **耗时** | **3 天+** | **30 秒** |

🎯 **在当时这是颠覆性的**：PasteForm v1 第一次证明了「基于 DDD 分层的低代码开发」在 .NET 生态是完全可行的，不需要写 80% 的胶水代码。

---

## 🔥 PasteForm 框架 v1 的核心创新点（当时行业领先）

### ✨ 创新1：首次提出「DTO = 契约 + UI 元数据」双驱动概念
这是 PasteForm 整个生态的灵魂思想：
```csharp
// —— PasteForm v1 首创的 DTO 特性驱动模式 ——
public class OrderListDto : EntityDto<long>
{
    [PasteSearch]                           // 🆕 首创：自动加搜索框
    [Description("订单号")]                 // 🆕 Description 一举三得（Swagger+表格+表单Label）
    public string OrderNo { get; set; }

    [ColumnDataType("dropdown")]            // 🆕 首创：声明式前端组件类型
    [DataSource("OrderStatusDictionary")]   // 🆕 首创：自动绑定字典数据源
    [Description("订单状态")]
    public int Status { get; set; }

    [PasteUpload]                           // 🆕 首创：文件/图片上传声明
    [Description("凭证")]
    public string VoucherUrl { get; set; }
}
```
→ **同一套 C# 特性，既做后端校验，又驱动前端 UI 渲染**。这一模式在当时的 .NET 开源生态中找不到第二家。

### ✨ 创新2：基于 Volo.Abp + 自研 DefaultAppService 实现「零 CRUD 代码」
PasteForm v1 站在 ABP 的肩膀上（ABP 提供 DDD 分层 + 模块化 + 工作单元 + AutoAPI），再补上**最关键的一块拼图**：`DefaultAppService`

DefaultAppService 让任何业务模块**只写 Entity + DTO** 就能立刻拥有：
```
GET    /api/app/{entity}/readAddModel          读新增元数据
GET    /api/app/{entity}/{id}/readUpdateModel  读更新元数据
GET    /api/app/{entity}/readListModel         读列表页元数据
GET    /api/app/{entity}/page                  分页+模糊搜索
GET    /api/app/{entity}/{id}                  查详情
POST   /api/app/{entity}/create                新增
POST   /api/app/{entity}/update                更新
DELETE /api/app/{entity}/{id}                  删除
```

→ **90% 的后台管理模块，一行业务代码不写就能上线**。

### ✨ 创新3：配套 PasteBuilder VSCode 插件 —— 一键生成全链路
PasteForm v1 是**国内首个配套 VSCode 插件进行全栈代码生成的 .NET 框架**。

在 `Domain/template/config.json` 中配置字段（类型、长度、特性映射、忽略列表）：
```json
{
  "ignore": {
    "all": { "add": ["CreateDate","AdminUid","CreateUid"], "update": ["UpdateDate","CreateDate"] }
  },
  "propertyAttributeMap": { "ProductName": ["PasteSearch", "MaxLength(100)"] }
}
```
点击 PasteBuilder 插件的「生成代码」按钮：
1. ✅ 自动生成强类型 Entity（带审计字段继承 Entity<TPrimaryKey>）
2. ✅ 自动生成 4 个 DTO（ListDto/AddDto/UpdateDto/Dto，特性贴好）
3. ✅ 自动生成 DbSet 到 DbContext
4. ✅ 自动生成 AppService 骨架（可选，如需特殊逻辑）
5. ✅ 自动生成迁移脚本

→ 当时被用户形容为「Paste 一下，Form 就出来了」→ **PasteForm 名字的由来**。

### ✨ 创新4：企业级基础设施开箱即用（基于 ABP 生态）
基于 Volo.Abp 8.x 提供了完整的企业级底座：
| 功能 | 实现方式（PasteForm v1 时代） |
|---|---|
| 登录认证 | 验证码 + JWT Bearer Token + 鉴权中间件 |
| RBAC 权限 | 用户-角色-功能权限三级（RoleAttribute 过滤器） |
| 审计日志 | ABP 内置审计日志（AuditLogging 模块） |
| 对象映射 | Volo.Abp.AutoMapper（基于 AutoMapper + Profile 配置） |
| 依赖注入 | 强制 Autofac（`UseAutofac()` 必开），ABP 模块化自动注册 |
| 工作单元 UoW | ABP 内置 UnitOfWork 拦截器（SaveChanges 自动调用） |
| 多租户 | ABP 内置多租户支持（可选） |
| 数据过滤 | ABP 内置软删除/多租户过滤（ISoftDelete 自动过滤） |
| 多数据库 | SQLite / PostgreSQL 双 Provider（ABP 封装） |
| 缓存 | ABP 分布式缓存抽象（IDistributedCache） |

---

## 🛠️ 技术栈（PasteForm v1 时代）

| 类别 | 技术 | 版本 | 说明 |
|---|---|---|---|
| **核心框架** | **Volo.Abp** | **8.3.4** | PasteForm v1 基于 ABP 构建，提供模块化/DDD/工作单元/AutoAPI 底座 |
| 运行时 | .NET | 8.0 | LTS |
| Web 框架 | ASP.NET Core | 8.0 | ABP 封装 |
| ORM | Entity Framework Core | 8.0 + Volo.Abp.EntityFrameworkCore | ABP 封装的 EF Core Provider |
| 数据库 Provider | Volo.Abp.EntityFrameworkCore.Sqlite / PostgreSql | 8.3.4 | ABP 官方数据库包 |
| **对象映射** | **Volo.Abp.AutoMapper** | **8.3.4** | 封装 AutoMapper，写法 `ObjectMapper.Map<TSource, TDest>(src)` |
| DI 容器 | **Volo.Abp.Autofac** | **8.3.4** | 强制 Autofac，不支持原生 DI 模式 |
| Swagger | Volo.Abp.Swashbuckle | 8.3.4 | ABP 封装的 Swagger |
| 代码生成 | PasteBuilder VSCode 插件 | v1.x | 配套 Domain/template/server.txt 模板 |
| 日志 | Serilog + ABP 日志集成 | 4.x+ | |

> 📝 **与 PasteApeart v2 的核心差异**：v1 **重度绑定 Volo.Abp 8.x 全家桶**，v2 **完全自研内核 + 移除所有 ABP 依赖**。

---

## 🏗️ 架构设计（PasteForm v1 经典 6 层）
```
┌──────────────────────────────────────────────────────────────┐
│              PasteTemplate.HttpApi.Host (入口)                │
│  Program.cs (UseAutofac 必开) + 中间件 + Swagger + 前端资源    │
├──────────────────────────────────────────────────────────────┤
│              PasteTemplate.HttpApi (ABP Controllers)         │
│          (ABP AutoAPI 自动生成，AppService 自动注册)           │
├──────────────────────────────────────────────────────────────┤
│                 PasteTemplate.Application                    │  ← ABP 应用服务层
│  DefaultAppService (v1 版，6 个基础 CRUD 方法)                │
│  自定义 AppService (继承 ABP CrudAppService / ApplicationService)
├──────────────────────────────────────────────────────────────┤
│             PasteTemplate.Application.Contracts              │  ← 契约层
│  DTO (PasteForm 特性驱动 UI) + IAppService 接口 + 枚举        │
├──────────────────────────────────────────────────────────────┤
│  PasteTemplate.Domain (ABP 领域层)                            │  ← 领域层
│  Entity (继承 ABP Entity<TPrimaryKey>/AuditedEntity)          │
│  Domain Service + PasteBuilder 模板 + 领域枚举                │
├──────────────────────────────────────────────────────────────┤
│  PasteTemplate.EntityFrameworkCore (ABP EF 模块)              │  ← 基础设施层
│  AbpDbContext 派生类 + 仓储模式 (IRepository<TEntity, TKey>)   │
├──────────────────────────────────────────────────────────────┤
│  PasteTemplate.Handler (PasteForm 特色层)                     │  ← 共享处理器层
│  缓存抽象 + 业务 Handler + 自动 DI 扩展 + 特性辅助             │
└──────────────────────────────────────────────────────────────┘

ABP 模块依赖顺序：
HttpApiHostModule → ApplicationModule → DomainModule
                            ↘  EntityFrameworkCoreModule
```

---

## 📁 PasteTemplate v1 目录结构（与 ApeartTemplate 对比）

```
PasteTemplate/
├── PasteTemplate.Domain/
│   ├── PasteTemplateDomainModule.cs       // 继承 AbpModule（依赖 AbpDddDomainModule）
│   ├── Base/Entity.cs                     // 继承 Abp AuditedEntity<TPrimaryKey>
│   └── template/
│       ├── server.txt  / server.v1.txt    // 👀 PasteForm v1 模板：ABP + ObjectMapper.Map
│       ├── dto.txt / entity.txt           // 生成模板
│       └── config.json                    // 字段生成配置（忽略列表/特性映射）
│
├── PasteTemplate.Application.Contracts/
│   ├── IMyAppService.cs                   // 继承 IApplicationService (ABP 接口)
│   └── DTOs/                              // 基于 Volo.Abp.Application.Dtos.EntityDto
│
├── PasteTemplate.Handler/
│   ├── *.csproj                           // <PackageReference Include="Volo.Abp.AutoMapper" />
│   └── modelhandler/                      // 全是 ObjectMapper.Map<,> 代码
│
├── PasteTemplate.EntityFrameworkCore/
│   ├── PasteTemplateDbContext.cs          // 继承 AbpDbContext
│   ├── *.csproj                           // <PackageReference Include="Volo.Abp.EntityFrameworkCore.*" />
│   └── Migrations/                        // EF 迁移
│
├── PasteTemplate.Application/
│   ├── defaultapi/DefaultAppService.cs    // v1 版，6 个基础方法
│   └── usermodels/*.AppService.cs         // 继承 CrudAppService（ABP）
│
└── PasteTemplate.HttpApi.Host/
    ├── Program.cs                         // .UseAutofac() 必开，ABP 启动模式
    ├── PasteTemplateHttpApiHostModule.cs  // 依赖 AbpAspNetCoreMvcModule
    └── *.csproj                           // <PackageReference Include="Volo.Abp.Autofac" />
                                           // <PackageReference Include="Volo.Abp.Swashbuckle" />
```

---

## 🚀 PasteTemplate v1 快速开始

### 第 1 步：还原依赖
```bash
cd PasteTemplate.HttpApi.Host
dotnet restore
```

### 第 2 步：初始化数据库
```bash
cd ../PasteTemplate.EntityFrameworkCore
dotnet ef migrations add Init --startup-project ../PasteTemplate.HttpApi.Host
dotnet ef database update    --startup-project ../PasteTemplate.HttpApi.Host
```

### 第 3 步：运行
```bash
cd ../PasteTemplate.HttpApi.Host
dotnet run
```

打开：
- 🌐 **系统**: https://localhost:5001
- 📖 **Swagger**: https://localhost:5001/swagger
- 👤 **默认账户**: `admin` / `123456`

---

## 💻 PasteForm v1 开发示例（基于 ABP + AutoMapper 时代写法）

### 写 AppService（PasteForm v1 标准写法）
```csharp
// 依赖：Volo.Abp.Application.Dtos + Volo.Abp.AutoMapper
public class ProductAppService : PasteTemplateAppService
{
    private readonly IRepository<Product, int> _repo; // ABP 仓储模式

    public ProductAppService(IRepository<Product, int> repo)
    {
        _repo = repo;
    }

    public async Task<ProductDto> Create(ProductAddDto input)
    {
        // PasteForm v1 标准写法：用 ObjectMapper.Map（ABP + AutoMapper 封装）
        var entity = ObjectMapper.Map<ProductAddDto, Product>(input);
        await _repo.InsertAsync(entity);

        var backDto = ObjectMapper.Map<Product, ProductDto>(entity);
        return backDto;
    }

    public async Task<ProductDto> Update(ProductUpdateDto input)
    {
        var entity = await _repo.GetAsync(input.Id);
        // PasteForm v1 更新：Map 到已有实体
        ObjectMapper.Map(input, entity);
        await _repo.UpdateAsync(entity);

        return ObjectMapper.Map<Product, ProductDto>(entity);
    }
}
```

👉 这种 `ObjectMapper.Map<,>()` + `IRepository<,>` 的写法是 **PasteForm v1 的标志性写法**，在 ApeartTemplate v2 中被全部注释掉，统一换成 Mapster 的 `.Adapt<,>()` + 直接用 DbContext。

---

## 📊 对象映射写法对比（v1 vs v2）

| 场景 | PasteForm v1 (AutoMapper) | **PasteApeart v2 (Mapster)** |
|---|---|---|
| 单对象映射 | `ObjectMapper.Map<Product, ProductDto>(product)` | `product.Adapt<ProductDto>()` |
| 更新到已有对象 | `ObjectMapper.Map(updateDto, entity)` | `updateDto.Adapt(entity)` |
| 集合映射 | `ObjectMapper.Map<List<Product>, List<ProductDto>>(list)` | `list.Adapt<List<ProductDto>>()` |
| 配置方式 | 需要写 `Profile : Profile` + `CreateMap<,>()` | 默认约定即可，复杂映射用 `TypeAdapterConfig<,>` |
| 包依赖 | `Volo.Abp.AutoMapper 8.3.4` → AutoMapper → 10+ 依赖 | `Mapster` → 0 额外依赖 |
| 基准性能 (10万次) | 基准线 1.0x | **3-10x 更快** |

---

## ⬆️ 从 PasteTemplate v1 升级到 PasteApeart (ApeartTemplate) v2 指南

### 为什么要升级？12 个硬核理由
| 维度 | PasteTemplate (v1) | ApeartTemplate (v2) | 升级价值 |
|---|---|---|---|
| **核心依赖** | 10+ 个 Volo.Abp 包（全家桶绑定） | ✅ 0 个 ABP 包，完全自研 | 启动 +40%、体积 -20MB、不被 ABP 升级绑定 |
| **对象映射** | AutoMapper + ABP 封装 | ✅ Mapster | 性能 3-10x、代码更简洁 |
| **AutoAPI 引擎** | ABP 自带 AutoAPI（黑盒） | ✅ 自研 ApeartAppServiceAutoConvention | 参数自动绑定、可配置、可调试 |
| **DefaultAppService** | 6 个基础方法 | ✅ 双版本号迭代 + 9 方法 + 详情模式 + PasteShort JOIN + 扩展绑定 | 兜底能力 100% 提升 |
| **审计日志** | ABP 内置（黑盒/合并主库） | ✅ 自研 AuditMiddleware + 独立 DbContext + 字段级变更追踪 | 合规级审计、分库独立存储 |
| **读写分离** | ❌ 不支持 | ✅ EF Core 拦截器 + 特性方案 | 读多写少场景性能翻倍 |
| **DI 容器** | 强制 Autofac | ✅ 原生 DI 默认 + Autofac 可选 | 小项目更快、大项目灵活 |
| **循环依赖** | 构造函数死循环常发 | ✅ ILazyServiceProvider 延迟解析 | 彻底解决 |
| **安全加固** | 基础 | ✅ SafeString/SafeDateTime + 全局异常 + 有序 GUID | 生产级防护 |
| **UI 驱动特性** | ~10 种组件类型 | ✅ 20+ 种（cascader/richtext/tag/color/rate/slider...） | 复杂表单无压力 |
| **模板体系** | 单套 server.txt | ✅ v1-v4 归档 + 智能分支 | 成熟稳定 |
| **跨平台** | AnyCPU 默认 | ✅ ARM64 显式支持 | 国产服务器/树莓派可部署 |

### 升级 8 步走（迁移清单）

```
1️⃣ 改 csproj：删所有 Volo.Abp.* 引用，加 Mapster、原生 EF Core Provider
2️⃣ 改 Module：所有 AbpModule → 自研 PasteApeartModule 风格
3️⃣ 改 Program.cs：.UseAutofac() 可注释掉（默认原生 DI），改用 AddControllers + 自己的 Convention
4️⃣ 全局替换：ObjectMapper.Map<TSource, TDest>(src) → src.Adapt<TDest>()
5️⃣ 全局替换：ObjectMapper.Map(src, dest) → src.Adapt(dest)
6️⃣ Repository 模式 → 直接用 DbContext.DbSet<T>（或保留 ABP 仓储过渡）
7️⃣ Entity 基类：ABP Entity/AuditedEntity → 自研 Entity（审计字段同名，兼容）
8️⃣ DefaultAppService 升级：替换为 ApeartTemplate v2 版（9 接口 + 新 Helper）
```

👉 **不想手动迁移？** 推荐直接 Clone ApeartTemplate 新项目，用 PasteBuilder 重新生成模块代码（因为 DTO 写法兼容，只需要改贴特性即可），30 分钟即可完成整个项目迁移。

---

## 📜 PasteForm 框架演化历史（时间线）

| 版本 | 代号 | 关键特征 |
|---|---|---|
| **v0.9** | PasteForm 早期原型 | 基础 DTO 特性驱动 + 代码生成插件 |
| **v1.0** | **PasteTemplate 发布**（本仓库） | 绑定 Volo.Abp 8.x、ABP AutoAPI、AutoMapper、server.v1 模板、DefaultAppService 6 方法 |
| **v1.5** | PasteTemplate 迭代 | server.v2/server.v3/v4 模板，PasteShort 外键字段 JOIN，PasteBuilder 插件完善 |
| **v2.0** | **ApeartTemplate 发布**（🚩 框架改名 PasteApeart） | ✅ 完全移除所有 ABP 依赖 + ✅ Mapster 替换 AutoMapper + ✅ 自研 AutoAPI Convention + ✅ 审计日志独立 DbContext + ✅ 读写分离 + ✅ LazyServiceProvider + ✅ ARM64 支持 |
| **v2.1** | ApeartTemplate 小版本迭代（当前） | DefaultAppService 双版本号（2025.07 + 2026.01）、PasteExtendBind 扩展绑定 |

---

## 🌐 相关资源

- **PasteForm 社区仓库**: https://gitee.com/pastecode/paste-template  （example/PasteTemplate + example/ApeartTemplate 都在这）
- **PasteApeart v2 升级模板**: 见同仓库 `example/ApeartTemplate`，或本项目同级 [apeart/README.md](./apeart/README.md)
- **PasteBuilder VSCode 插件**: VSCode 商店搜索「PasteBuilder」安装（兼容 v1/v2 双模板体系）

---

> 🧭 **选型建议**
> - 如果你的项目**已经基于 PasteTemplate 跑起来且稳定**：继续维护即可，按需局部升级
> - 如果你的项目**是新项目或计划重构**：**直接上 PasteApeart (ApeartTemplate) v2**，性能、可维护性、可控性全面碾压
