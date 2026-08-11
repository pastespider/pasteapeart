基于您提供的代码，我为您整理了一份**商城功能特性文档**。这份文档将代码中的特性（Attributes）按功能模块分类，并说明了它们在商城系统中的应用场景。

---

# 商城系统——特性（Attribute）功能参考文档

## 1. 文档说明
本文档基于 `PasteApeart.Helper` 命名空间下的数据特性（Data Annotation Attributes）编写，旨在为商城系统的前端UI渲染、后端服务逻辑、数据校验、权限控制及特殊功能提供标准化支持。

**宏约定说明：**
- 前端支持 `{{:=item.xxx}}` 格式的宏替换。
- 取值优先级：`Form` > `Query` > 表格当前行数据。

---

## 2. 核心数据展示与编辑

### 2.1 基础组件
| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteTextAttribute` | 文本输入框 | 商品名称、收货地址 |
| `PasteTextAreaAttribute` | 多行文本域 | 商品描述、备注信息 |
| `PasteHiddenAttribute` | 隐藏字段（UI隐藏，但提交数据） | 商品ID、用户Token |
| `PasteReadOnlyAttribute` | 只读模式 | 订单编号、创建时间 |
| `PastePasswordAttribute` | 密码输入框 | 支付密码、登录密码 |
| `PasteNumbericAttribute` *(注：代码中未显式列出，但常见于商城)* | 数字输入框 | 商品价格、库存数量 |

### 2.2 选择与关联
| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteSelectAttribute` | 下拉选择框（单选） | 商品分类、订单状态 |
| `PasteSelectsAttribute` / `PasteLselectAttribute` | 多选下拉/列表框 | 商品标签、多规格属性 |
| `PasteOuterAttribute` | 外键选择器（弹窗选择） | 选择所属店铺、选择优惠券模板 |
| `PasteOutersAttribute` / `PasteObjectsAttribute` | 多选外键或对象录入 | 批量选择商品、组合套餐 |

### 2.3 特殊输入
| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteImageAttribute` | 图片上传 | 商品主图、详情图、用户头像 |
| `PasteImageSelectAttribute` | 从图库中选择图片 | 选取已上传的商品素材 |
| `PasteFileAttribute` | 文件上传 | 商品说明书、物流凭证 |
| `PasteRichTextAttribute` | 富文本编辑器 | 商品详情页、活动公告 |
| `PasteMarkDownAttribute` | Markdown编辑器 | 商品长篇介绍、帮助文档 |
| `PasteLocationAttribute` / `PasteGpsAttribute` | 地理位置/坐标获取 | 门店定位、配送地址 |

---

## 3. 数据展示与格式化

### 3.1 格式化显示
| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteUnitAttribute` | 添加单位 | 价格后显示“元”，重量后显示“克” |
| `PastePrefixAttribute` | 添加前缀 | 金额前显示“￥” |
| `PasteFenToYuanAttribute` | 分转元 | 订单金额（数据库存分，页面显示元） |
| `PasteToKmAttribute` | 米转千米 | 配送距离显示 |
| `PasteToSizeAttribute` | 字节转可读大小 | 文件资源大小显示 |
| `PasteDateAttribute` | 日期时间格式化 | 下单时间、支付时间 |

### 3.2 表格与列表增强
| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteHtmlAttribute` | 自定义HTML渲染 | 状态标签（已支付/待发货） |
| `PasteDisplayAttribute` | 显示对象的某个字段 | 显示关联商品名称 |
| `PasteSumAttribute` | 字段统计（求和） | 订单总金额、商品总销量 |
| `PasteScoreAttribute` | 评分显示 | 商品评价星级 |
| `PasteTrclassAttribute` / `PasteTrhtmlAttribute` | 表格行样式/属性 | 库存不足行标红、活动商品高亮 |
| `PasteMarkAttribute` | 书签功能 | 标记重要订单、收藏商品 |

---

## 4. 功能按钮与操作

### 4.1 行内操作
| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteButtonAttribute` | 自定义按钮 | “立即购买”、“加入购物车” |
| `PasteMenuAttribute` | 普通菜单项 | 订单列表的“查看详情” |
| `PasteIfMenuAttribute` | 条件菜单（根据数据控制显示） | 仅“待付款”订单显示“去支付” |
| `PasteSwitchAttribute` | 开关（启用/禁用） | 商品上下架、优惠券启用状态 |
| `PasteMinbtnAttribute` | 表格行内小按钮 | 编辑数量、快速修改价格 |

### 4.2 批量操作
| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteBatchAttribute` | 批量操作按钮 | 批量上架、批量发货、批量删除 |

---

## 5. 数据校验与约束

| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteRequiredAttribute` | 必填校验 | 商品名称、价格必填 |
| `PasteMutexAttribute` | 唯一键（排重） | 商品编码、用户手机号唯一 |
| `PasteBatchMutexAttribute` | 联合唯一约束（And关系） | 同一店铺下分类名称唯一 |
| `PasteIgnoreAttribute` | 忽略该字段（不参与前端生成或后端映射） | 内部状态字段不展示 |

---

## 6. 查询与过滤

| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteFieldAttribute` | 字段映射与查询运算符 | 按价格区间查询（> < >= <=） |
| `PasteSearchAttribute` | 全文/模糊搜索标记 | 商品名称关键词搜索 |
| `PasteDaterangeAttribute` | 日期区间查询 | 订单创建时间筛选 |
| `PasteSearchRangeAttribute` | 数值范围查询 | 价格区间（80-90元） |
| `PasteNextkeyAttribute` | 游标分页 | 商品列表无限滚动加载 |
| `PasteOrderbyAttribute` | 排序字段配置 | 按销量/价格排序 |

---

## 7. 页面布局与交互

| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteGroupAttribute` / `PasteGroupItemAttribute` | 字段分组展示 | 商品信息分“基本信息”、“销售信息”、“物流信息” |
| `PasteSingleRowAttribute` / `PasteHalfRowAttribute` | 独占一行/半行 | 商品详情（独占一行）、价格（半行） |
| `PasteStyleAttribute` | 自定义样式 | 设置文本框高度、按钮颜色 |
| `PasteClassAttribute` | 自定义CSS类 | 浮动样式、边距调整 |
| `PasteTitleAttribute` | 自定义Title提示 | 鼠标悬停显示帮助信息 |
| `PasteChangeuiAttribute` | 联动显隐 | 选择“虚拟商品”时显示“下载链接”字段 |
| `PasteOkactionAttribute` | 表单提交后动作 | 支付成功后跳转订单详情页 |
| `PasteConfigAttribute` | 页面级配置 | 开启拖拽排序、设置默认图表类型 |
| `PasteUIAttribute` | 列表展示模式（表格/图表） | 销售报表以图表显示 |

---

## 8. 权限与安全

| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteAuthAttribute` | 方法级权限控制 | 只有“管理员”可删除商品 |
| `PasteShowAttribute` | 根据权限显示/隐藏 | 有权限才显示“导出报表”按钮 |
| `PasteEnableAttribute` | 启用/禁用CRUD功能 | 针对不同角色配置是否允许“新增”、“编辑”、“删除” |
| `PasteHalfHideAttribute` | 数据脱敏 | 手机号显示为 138****1234 |
| `PasteDisableAttribute` *(已过时)* | 禁用功能（建议使用Enable） | - |

---

## 9. 服务端扩展与高级功能

| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteExtendBindAttribute` | 服务端扩展绑定（如角色权限分配） | 为角色分配菜单权限 |
| `PasteLinkAttribute` | 关联外键（导航属性） | 订单关联用户信息 |
| `PasteIncludeAttribute` | 预加载关联数据 | 查询商品时一并加载分类信息 |
| `PasteShortAttribute` / `PasteShortModelAttribute` | 生成简略模型（DTO） | 订单列表中仅展示用户头像和姓名 |
| `PasteLinkQueryAttribute` | 传递查询参数到下一级 | 点击商品分类跳转列表时传递分类ID |
| `PasteFieldQueryAttribute` | 自动生成查询字段 | 根据实体字段自动生成查询条件 |

---

## 10. 特殊业务组件

| 特性 | 说明 | 商城应用示例 |
| :--- | :--- | :--- |
| `PasteMarkPopupAttribute` | 弹窗式书签选择 | 选择商品标签、选择会员等级 |
| `PasteSplitAttribute` | 字符串拆分为数组 | 输入“红,蓝,黄”解析为颜色数组 |
| `PasteSunformAttribute` | 嵌入Iframe子表单 | 编辑商品规格组合 |
| `PasteDirectsunAttribute` | 多级分拆（支持层级结构） | 商品属性分组（如：内存__容量） |
| `PasteLoaditemAttribute` | 关联加载数据 | 选择店铺后自动加载该店铺的运费模板 |
| `PasteLoadlibAttribute` | 动态加载JS/CSS资源 | 加载第三方支付组件库 |
| `PasteTemplateAttribute` | 自定义模板渲染 | 定制表格头部/行模板 |

---

## 11. 备注与建议

1. **特性选择原则**：优先使用明确语义的特性（如 `PasteEnable` 替代过时的 `PasteDisable`）。
2. **性能考虑**：服务端特性（`IsServerAttribute = true`）会影响API生成逻辑，请合理使用。
3. **安全性**：敏感字段（如密码、身份证号）务必使用 `PasteHalfHideAttribute` 或 `PasteIgnoreAttribute`。
4. **兼容性**：宏 `{{:=item.xxx}}` 在表单、表格、链接参数中广泛使用，请确保字段名称匹配。

---

> **文档版本**：1.0  
> **适用范围**：基于 PasteApeart 框架的商城后台及前端页面开发。  
> **更新日期**：2026-07-27