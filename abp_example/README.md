
# ExampleForm 框架：敏捷开发的革命性工具

## 简介

DeepSeek 评价 ExampleForm 为“敏捷开发的天花板”，这并非过誉。ExampleForm 不仅仅是一个框架，更是一种**开发思想**的创新实践。

### 核心理念：All In Dto！

ExampleForm 的核心思想是**以 Dto（数据传输对象）为中心**。通过反射机制解析 Dto 中的特性和字段信息，自动生成管理端所需的表单、表格甚至图表组件，实现从数据模型到UI界面的自动化映射。

## 项目结构概览

使用 VS2022 打开项目后，你会看到以下结构：

![项目结构](https://soft.pastecode.cn/upload/bookmark/202505/4be9ce8f6e913f40f4accba282eb7186.webp)

**核心说明：**
- `dynamicmodels`、`newsmodels`、`testmodels`、`usermodels` 是示例模块，可按需删除
- 所有模块都围绕 `Domain` 子项目构建
- 管理端位于 `ExampleForm.HttpApi.Host/wwwroot` 文件夹中

## 快速开始

### 第一步：环境准备
1. 等待 VS2022 自动还原 NuGet 包
2. 设置 `ExampleForm.HttpApi.Host` 为启动项目

![启动项目设置](https://soft.pastecode.cn/upload/bookmark/202505/fe7246ebf2dcca866e3913ec0333f2f2.webp)

### 第二步：数据库初始化
在包管理器控制台中执行：
```powershell
add-migration init_database -Context SqliteDbContext
```

### 第三步：启动项目
默认使用 SQLite 数据库，访问地址：
```
http://localhost:22222/page/index.html
```

**注意：** 如遇 404 错误，请将 `wwwroot` 文件夹复制到 `bin\Release\net6.0\`（或对应 .NET 版本）目录下。

### 第四步：登录管理端
使用默认账号密码登录，即可进入功能完整的管理后台。

![管理端界面](https://soft.pastecode.cn/upload/bookmark/202505/c1c66f2bf378f567c087caa5b135c195.webp)

## 核心配置：Template 文件夹

`ExampleForm.Domain/template` 文件夹是关键配置所在：

![模板文件](https://soft.pastecode.cn/upload/bookmark/202505/707decc0b1e33c7c96018173fba88ce7.webp)

重点关注 `dto.txt` 和 `server.txt` 模板文件。

## 开发工作流示例

### 1. 创建数据模型
```csharp
[PasteShortModel("TestShort", "ToTestShort", "Id", "Name")]
public class TestTable : Entity<int>
{
    [Required]
    [MaxLength(16)]
    public string Name { get; set; }
    
    [PasteImage(1, "head", "120*120")]
    public string Head { get; set; }
    
    [PasteUnit("周岁")]
    public int Age { get; set; }
    
    [PasteDaterange("dateStart", "dateEnd")]
    public DateTime DateStart { get; set; }
    
    [PasteHidden]
    public DateTime DateEnd { get; set; }
    
    [PasteFenToYuan]
    public int Balance { get; set; }
    
    [PasteShort("UserInfo", "ExtendUser", "ToUserShort()")]
    public int UserId { get; set; }
}
```

### 2. 一键生成代码
右键点击模型文件，选择 **PasteBuilder → 代码生成**，系统将自动创建：

- **数据库上下文配置**（DbContext 和模型构建）
- **DTO 文件**（包含完整的 CRUD Dto）
- **映射扩展方法**（在 LinqExtend.cs 中）
- **应用服务层**（AppService 和 AutoMapper 配置）

![代码生成结果](https://soft.pastecode.cn/upload/bookmark/202505/cee86fd009106e657563186c028a78d1.webp)

**智能防覆盖机制：** 已修改的文件不会因重复生成而被覆盖，确保自定义代码的安全。

## 模板自定义

ExampleForm 使用 Liquid 模板引擎，允许深度自定义生成的代码结构。

**模板位置：** `ExampleForm.Domain/template/`
- `dto.txt` - 控制 DTO 生成
- `server.txt` - 控制服务层生成

**学习资源：**
- [DotLiquid 官方文档](http://dotliquidmarkup.org/)
- [Liquid 模板语言参考](https://shopify.github.io/liquid/)

## 生态工具集

### 1. PasteBuilder（代码生成器）
- VS2022 扩展插件
- 专为 ExampleForm 设计的一键生成工具
- [开源地址](https://gitee.com/pastecode/paste-builder)

### 2. PasteDocument（智能文档）
- 基于标记特性的在线文档系统
- 实现“哪里不会点哪里”的交互体验
- [开源地址](https://gitee.com/pastecode/paste-document)

### 3. PasteSpider（部署工具）
- 跨平台部署解决方案
- 支持多种运行方式：Docker、IIS、系统服务等
- [开源地址](https://gitee.com/pastecode/PasteSpider)

## 架构优势

基于 ABP 框架的最佳实践：
- **Domain** - 领域模型层
- **Application.Contracts** - 应用契约层
- **EntityFramework** - 数据持久层
- **Application** - 应用实现层
- **HttpApi.Host** - API 宿主层
- **Handler** - 业务处理层（创新设计）

这种分层架构将不同职责规范分离，同时保持高度的内聚性。

---

ExampleForm 通过“All In Dto”的核心理念，结合强大的代码生成能力和完整的工具链，真正实现了从数据模型到完整应用的快速开发，无愧于“敏捷开发天花板”的评价。