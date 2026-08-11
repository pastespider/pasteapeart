以下是根据您提供的代码整理成的Markdown格式文档，包含各个特性的前端名称（Name）以及参数说明。

---

# PasteApeart 特性参考文档

> **宏约定说明**
>
> 写法：`{{:=item.xxx}}`，例如：`./pasteform/index.html?path=userInfo&role_id={{:=item.id}}`
>
> 1. 格式固定为 `{{:=item.}}`，适用于表格中当前行数据
> 2. 取值顺序：先表单（form），后查询参数（query）
> 3. 表格数据仅从当前行获取

---

## PasteMarkAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `mark` |
| **说明** | 书签特性，用于标记字段为书签 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_model` | string | `""` | 书签模块，如 `product`，为空表示当前模块 |
| `_value` | string | `""` | 书签项，如 `code`，为空表示当前字段 |
| `args3` | string | `""` | 备用（如填写 `popup` 则等同于 Popup 版本） |
| `args4` | string | `""` | 备用 |

---

## PasteMarkPopupAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `mark` |
| **说明** | 书签特性的弹窗形式，需在 `../lib/api.js` 中配置 `global_tap_mark` 的 `apiRoute` |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_model` | string | `""` | 书签模块，为空表示当前模块 |
| `_value` | string | `""` | 书签项，为空表示当前字段 |
| `args3` | string | `"popup"` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteRichTextAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `richtext` |
| **说明** | HTML 富文本编辑器 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_height` | int | `0` | UI 高度设置 |
| `_txt_save_column` | string | `""` | 对应的文本提交字段（一般设置为 hidden） |
| `_image_upload_path` | string | `""` | 上传图片的 API 地址 |
| `args4` | string | `""` | 备用 |

---

## PasteFieldAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `field` |
| **说明** | 字段映射/关联，用于查询时指定实际字段名 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `field` | string | 必填 | 映射的目标字段名，如 `UserId` |
| `operatorName` | string | `"="` | 查询运算符：`=` / `!=` / `>` / `<` / `>=` / `<=` / `()`（LIKE） |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteMutexAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `mutex` |
| **服务端特性** | ✅ 是 |
| **说明** | 排重键，表示字段唯一，用于更新写入时的排重判断。多个字段默认 OR 关系，如需 AND 关系请配合 `PasteBatchMutex` |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `""` | 备用 |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteBatchMutexAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `batchmutex` |
| **服务端特性** | ✅ 是 |
| **说明** | 标注在 Class 上，表示新增或更新时多个排重字段的关系为 AND（默认为 OR） |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `""` | 备用 |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteExtendBindAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `extendbind` |
| **服务端特性** | ✅ 是 |
| **说明** | 扩展绑定，例如为角色分配权限。标注在 InputQueryXXX 上表示查询扩展（仅允许一个），标注在 ListDto 上表示更新绑定数据（允许多个） |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | 必填 | 如 `GradeBindRole`（实体名称） |
| `args2` | string | 必填 | 如 `RoleId`（当前表的字段） |
| `args3` | string | 必填 | 如 `GradeId` |
| `args4` | string | 必填 | 如 `grade_id`（来源于参数） |

---

## PasteMarkDownAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `markdown` |
| **说明** | Markdown 编辑器 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_height` | int | `0` | UI 高度设置 |
| `_html_save_column` | string | `""` | 对应的 HTML 提交字段（一般设置为 hidden） |
| `_image_upload_path` | string | `""` | 上传图片的 API 地址 |
| `args4` | string | `""` | 备用 |

---

## PasteDaterangeAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `daterange` |
| **说明** | 日期区间/时间区间，最终转换为 `yyyy-MM-dd HH:mm:ss` 格式提交 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_sdate` | string | `"sdate"` | 开始日期字段名 |
| `_edate` | string | `"edate"` | 结束日期字段名 |
| `args3` | string | `""` | 备用 |
| `out_format` | string | `""` | 输出格式：`times`（秒级时间戳）/ `timems`（毫秒级时间戳）/ 其他格式 |

---

## PasteSearchRangeAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `searchrange` |
| **服务端特性** | ✅ 是 |
| **说明** | 查询范围，用于数值类型（如 80-90 分），小值包含，大值不包含 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_sdate` | string | `"minLimit"` | 开始值字段（包含） |
| `_edate` | string | `"maxLimit"` | 结束值字段（不包含） |
| `step` | string | `"1"` | 输入粒度，如 `1` 或 `0.01` |
| `args4` | string | `""` | 备用 |

---

## PasteDateAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `datetime` |
| **说明** | 时间输入控件 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `in_format` | string | `""` | 输入格式（UI 格式），如 `yyyy-MM-dd HH:mm:ss` |
| `out_format` | string | `""` | 输出格式（后端格式）：`times` / `timems` / 其他 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteClassAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `class` |
| **说明** | 应用 CSS 样式类 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `className` | string | `"fleft"` | 样式名称 |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteSwitchAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `switch` |
| **说明** | Switch 开关控件，支持交互 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `onlyModel` | string | `"view"` | 生效模式，留空为全部生效 |
| `confirm` | bool | `false` | 提交前是否弹出确认框 |
| `func` | string | `""` | 函数名称（无参数），用于改变前提问 |
| `remote` | bool | `true` | 列表中点击后是否提交 API，`false` 时不提交（用于表格编辑） |

---

## PasteHiddenAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `hidden` |
| **说明** | UI 中隐藏该字段，但提交时会正常提交（相当于只读的隐藏版） |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_unhidden_model` | string | `""` | 某个模式下不隐藏（查看 `_config.model`） |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteShowAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `hidden` |
| **说明** | `PasteHidden` 的权限版本，有权限时不下发 hidden 特性 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_unhidden_model` | string | `""` | 某个模式下不隐藏 |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |
| `btnCode` | string | `""` | 权限代码，有权限时不隐藏 |

---

## PasteHideAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `hide` |
| **说明** | 完全隐藏，数据返回前端但不回传（回传时使用 API 端默认值），类似 `JsonIgnore` |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `""` | 备用 |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteMenuAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `menu` / `ifmenu` |
| **说明** | 菜单或条件菜单按钮 |

**构造函数 1（菜单 A 标签模式）**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_name` | string | 必填 | 名称 |
| `_script` | string | 必填 | 执行的函数 |
| `_iconfont` | string | `""` | 图标样式类 |
| `_box` | bool | `false` | 是否放置于按钮盒子中 |
| `btnCode` | string | `""` | 按钮权限码 |

**构造函数 2（直接按钮模式，恒成立）**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `name` | string | 必填 | 菜单名称 |
| `func` | string | 必填 | 执行函数，如 `func_read_abc({{:=item.id}});` |
| `className` | string | 必填 | 样式名称 |
| `iconFont` | string | `""` | 图标样式类 |
| `_box` | bool | `false` | 是否放置于按钮盒子中 |
| `args4` | string | `""` | 备用 |
| `btnCode` | string | `""` | 按钮权限码 |

---

## PasteIfMenuAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `ifmenu` |
| **说明** | 条件菜单按钮 |

**构造函数 1（条件按钮）**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_expresion` | string | 必填 | 条件表达式，如 `item.isReady && item.stype==1` |
| `_insertHtml` | string | 必填 | 插入的 HTML 内容 |
| `_box` | bool | `false` | 是否在按钮盒子中 |
| `args4` | string | `""` | 备用 |
| `btnCode` | string | `""` | 按钮权限码 |

**构造函数 2（条件模式的直接按钮）**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `expiresion` | string | 必填 | 条件表达式 |
| `name` | string | 必填 | 菜单名称 |
| `func` | string | 必填 | 执行函数 |
| `iconFont` | string | `""` | 图标样式类 |
| `_box` | bool | `false` | 是否在按钮盒子中 |
| `className` | string | `""` | 样式名称 |
| `args4` | string | `""` | 备用 |
| `btnCode` | string | `""` | 按钮权限码 |

---

## PasteBatchAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `batch` |
| **说明** | 批量操作按钮，使用 `FuncReadBitchCheckedNumber` 或 `FuncReadBitchCheckedString` 读取选中值 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_name` | string | 必填 | 按钮名称 |
| `_script` | string | 必填 | 执行的脚本 |
| `_iconfont` | string | `""` | 图标样式类 |
| `title` | string | `""` | 鼠标悬浮标题 |
| `btnCode` | string | `""` | 按钮权限码 |

---

## PasteLinkQueryAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `linkquery` |
| **说明** | 传递参数给下一级，一般放置于 ListDto 中 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_querys` | string | 必填 | 参数名称，多个用逗号分隔，如 `vname` |
| `_args2` | string | `""` | 备用 |
| `_args3` | string | `""` | 备用 |
| `_args4` | string | `""` | 备用 |

---

## PasteLinkAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `link` |
| **服务端特性** | ✅ 是 |
| **说明** | 链接外表，如 `UserGrade.UserId` 链接到 `UserInfo.Id`，标注于 Entity |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `tableName` | string | 必填 | 表名，如 `UserInfo` |
| `key` | string | `"Id"` | 关联键 |
| `_args3` | string | `""` | 备用 |
| `_args4` | string | `""` | 备用 |

---

## PasteIncludeAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `include` |
| **服务端特性** | ✅ 是 |
| **说明** | 类似 `PasteLink`，表示 Include 表，标注于 Entity |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `tableName` | string | `""` | 表名，如 `UserInfo` |
| `key` | string | `"Id"` | 关联键 |
| `_args3` | string | `""` | 备用 |
| `_args4` | string | `""` | 备用 |

---

## PasteQueryAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `query` |
| **说明** | 读取 query 中的某个值，常与 `PasteQueryLink` 合并使用 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_query` | string | 必填 | 参数名，如 `id` |
| `_hide` / `hide` | string/bool | `""` / `false` | 获取数据后是否隐藏，建议填写 `hide` |
| `_args3` | string | `""` | 备用 |
| `_args4` | string | `""` | 备用 |

---

## PasteFieldQueryAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `filedquery` |
| **服务端特性** | ✅ 是 |
| **说明** | 创建字段查询项，标注在属性上后，对应的 `InputQueryXXX` 会添加该查询项 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `""` | 备用 |
| `args2` | string | `""` | 备用 |
| `_args3` | string | `""` | 备用 |
| `_args4` | string | `""` | 备用 |

---

## PasteSearchAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `search` |
| **服务端特性** | ✅ 是 |
| **说明** | 标记当前 string 类型字段要进行搜索（word 字段匹配），标注于 Entity |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `""` | 备用 |
| `args2` | string | `""` | 备用 |
| `_args3` | string | `""` | 备用 |
| `_args4` | string | `""` | 备用 |

---

## PasteTemplateAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `template` |
| **说明** | 标注使用哪个模板，用于 ListDto |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `headTemplateId` | string | 必填 | 表格头部模板 ID，如 `template_table_head` |
| `bodyTemplateId` | string | 必填 | 表格内容模板 ID，如 `template_table_body` |
| `_args3` | string | `""` | 备用 |
| `_args4` | string | `""` | 备用 |

---

## PasteTextAreaAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `textarea` |
| **说明** | 使用 TextArea 渲染内容 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `""` | 自定义样式，如 `height:500px;` |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteTextAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `text` |
| **说明** | 使用 Input text 渲染 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `""` | 是否单行独占（如 `true`，同 `PasteSingleRow`） |
| `args2` | string | `""` | 外样式，如 `menubtn` |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteSingleRowAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `singlerow` |
| **说明** | 标记字段独占一行 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `""` | 备用 |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteHalfRowAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `halfrow` |
| **说明** | 标记字段占半行（主要针对 `maxlength > 64` 的字段） |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `""` | 备用 |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteEnableAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `enable` |
| **说明** | 是否启用 CRUD 功能（`PasteDisable` 的反向配置）。无值时使用 UI 默认配置 |

**构造函数 1（int 参数）**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `add` | int | 必填 | `1`=启用，`0`=禁用，`2`=忽略 |
| `edit` | int | 必填 | `1`=启用，`0`=禁用，`2`=忽略 |
| `del` | int | 必填 | `1`=启用，`0`=禁用，`2`=忽略 |
| `detail` | int | 必填 | `1`=启用，`0`=禁用，`2`=忽略 |

**构造函数 2（string 参数）**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `add` | string | 必填 | `true`/`false`/留空 |
| `edit` | string | 必填 | `true`/`false`/留空 |
| `del` | string | 必填 | `true`/`false`/留空 |
| `detail` | string | 必填 | `true`/`false`/留空 |

---

## PasteAuthAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `auth` |
| **服务端特性** | ✅ 是 |
| **说明** | 权限标记，标注于 Class。用于 DefaultAppService 权限控制 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `method` | string | `"*"` | 动作：`*` / `add` / `update` / `list` / `detail` / `del`，多个用逗号分隔 |
| `auth` | bool | `true` | 是否有权限 |
| `Model` | string | `""` | 权限模块 |
| `Role` | string | `""` | 权限角色 |

---

## PasteUnitAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `unit` |
| **说明** | 为计量添加单位 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_unit` | string | 必填 | 单位，如 `元` |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PastePrefixAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `prefix` |
| **说明** | 前缀特性（如有 `FenToYuan`，需在其后标注） |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_prefix` | string | 必填 | 前缀，如 `￥` |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteEmptyAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `empty` |
| **说明** | 留空显示（占位置，与 `Hidden` 不同） |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `""` | 备用 |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteLocationAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `location` |
| **说明** | 获取定位信息（坐标 + 地址），对应微信 `wx.chooseLocation` |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `City` | string | `""` | 城市存放字段 |
| `Longitude` | string | `"longitude"` | 经度存放字段（double） |
| `Latitude` | string | `"latitude"` | 纬度存放字段（double） |

---

## PasteGpsAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `gps` |
| **说明** | 获取定位信息（仅坐标），对应微信 `wx.getLocation` |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Longitude` | string | `"longitude"` | 经度存放字段（double） |
| `Latitude` | string | `"latitude"` | 纬度存放字段（double） |

---

## PasteHtmlAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `html` |
| **说明** | 渲染 HTML，一般用于 ListDto |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_html_code` | string | 必填 | HTML 代码，如 `<div class="idesc">{{:=item.name}}</div>` |

---

## PasteSelectAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `select` |
| **说明** | Select 下拉选择，支持枚举类型 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_selectvalues` | string | `""` | 选项 JSON 字符串，如 `[{name,value,selected}]`；枚举类型可为空 |
| `_ignores` | string | `""` | 要去掉的值（枚举适用），如去掉 `-1`，多个用逗号分隔 |
| `args2` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteOkactionAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `okaction` |
| **说明** | 表单提交成功后的后续动作 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `page` | string | `"parent"` | 页面：`parent` / `current` |
| `action` | string | `""` | 动作：`href` / `close` / `replace` / `open` / `reload` |
| `url` | string | `""` | 跳转 URL，可使用 `{{id}}` 带入返回值 |
| `args4` | string | `""` | 备用 |

---

## PasteDatalistAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `datalist` |
| **说明** | DataList 特性，支持枚举 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_selectvalues` | string | `""` | 选项 JSON 字符串；枚举类型可为空 |
| `_ignores` | string | `""` | 要去掉的值，多个用逗号分隔 |

---

## PasteLselectAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `lselect` |
| **说明** | 多选下拉框 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_selectvalues` | string | `""` | 选项 JSON 字符串；枚举类型可为空 |
| `_ignores` | string | `""` | 要去掉的值 |
| `_num` | int | `0` | 允许选择个数，`0` 表示不限 |

---

## PasteSelectsAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `selects` |
| **说明** | 多选下拉框（带分隔符） |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_selectvalues` | string | `""` | 选项 JSON 字符串 |
| `_splitchar` | string | `","` | 返回值分隔符 |
| `_ignores` | string | `""` | 要去掉的值 |
| `_num` | int | `0` | 允许选择个数，`0` 表示不限 |

---

## PasteIgnoreAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `ignore` |
| **服务端特性** | ✅ 是 |
| **说明** | 忽略字段，不返回前端。与 `JsonIgnore` 的区别：后端仍可接收 JSON，但不返回。标注于 Class 时表示不进行描述处理 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `""` | 备用 |
| `args2` | string | `""` | 备用 |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteImageAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `image` |
| **说明** | 图片上传（配合 `PasteImageSelectAttribute` 可从图库选择） |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `_num` | int | `1` | 可上传数量 |
| `_path` | string | `""` | 存放路径，如 `icon` |
| `_size` | string | `""` | 转化尺寸，如 `60*60` / `1920*0` |
| `className` | string | `""` | 样式名称 |

---

## PasteImageSelectAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `simg` |
| **说明** | 图片选择器选择 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `args1` | string | `"./images/index.html"` | 选择页面路径 |
| `args2` | string | `""` | 参数，支持宏，如 `?type=head&cid={{:=item.id}}` |
| `args3` | string | `""` | 备用 |
| `args4` | string | `""` | 备用 |

---

## PasteImageExpendAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `imgexpend` |
| **说明** | 针对 `mediaid` 和 `url` 的情况 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `mediaId` | string | `""` | 表单中图片的实际值 |
| `url` | string | `""` | 图片预览地址（相对地址），默认 value 即为 url |
| `field` | string | `"url"` | 值字段名（mediaid 字段名），需上传返回值支持 |
| `args4` | string | `""` | 备用 |

---

## PasteOuterAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `outer` |
| **说明** | 链接外表显示和输入 |

**参数**

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `path` | string | 必填 | 外表名称或路径，如 `serviceInfo` 或 `./pasteform/index.html?path=xxxx&model=select`，支持宏 |
| `extendName` | string | `""` | 显示时表达式，如 `extendService` |
| `_keyColumn` | string | `"id"` | 回传列名，多个用逗号分隔，第一个为主键 |
| `_showColumn` | string | `"name"` | 显示列名，支持 `+` 拼接 |

---

## PasteShortAttribute

| 项目 | 说明 |
|------|------|
| **前端 Name** | `short` |
| **服务端特性** | ✅ 是 |
| **说明** | 为字段扩展外表，标注在外表 Key（如 `UserId`）上 |

**参数**

| 参数 | 类型 | 默认值 | 说明