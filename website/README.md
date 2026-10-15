# PasteApeart 官网

> **All in Dto + All in Handler** —— DTO 元数据驱动 UI，Handler 聚合业务场景。

原生静态站点：`HTML + 原生 JS + LESS`，无框架、无构建工具依赖，双击 `index.html` 即可浏览。

---

## 文件结构

```
pasteapeart-website/
├── index.html      # 单页站点，15 个 section
├── styles.less     # 样式源文件（LESS）
├── styles.css      # 编译产物 —— 页面实际加载的就是它
├── main.js         # 交互脚本（原生 JS，无依赖）
└── README.md
```

> ⚠️ **修改样式请改 `styles.less`，然后重新编译出 `styles.css`。**
> 页面通过 `<link rel="stylesheet" href="styles.css">` 加载样式，**不会**在浏览器里
> 运行时编译 LESS。原因：浏览器端 `less.js` 需要 XHR 拉取 `.less` 文件，用 `file://`
> 直接打开页面时会被 CORS 拦截，导致页面完全没有样式。

---

## 本地预览

直接双击 `index.html` 即可（无需服务器）。

需要本地服务器时（例如想用 http 调试）：

```bash
# 任选其一
npx serve pasteapeart-website
python -m http.server -d pasteapeart-website 8080
```

---

## 编译 LESS

```bash
npm install less            # 一次性
npx lessc styles.less styles.css
```

或全局安装：

```bash
npm i -g less
lessc styles.less styles.css
```

### LESS 踩坑提醒

`.container` 的宽度写法必须转义，否则 LESS 会把它当算术式计算：

```less
// ❌ 错误：LESS 算出 100% - 40 = 60，编译成 min(60%, 1200px)
width: min(100% - 40px, @container-width);

// ✅ 正确：用 ~"" 转义字符串，LESS 原样输出
width: min(@container-width, ~"calc(100% - 40px)");
```

`calc()` 内的运算 LESS 不会计算，但 `min()` / `max()` / `clamp()` 会。

---

## 页面板块

| # | id | 内容 |
|---|----|----|
| 1 | `hero` | 首屏：`All in Dto + All in Handler` + 浏览器窗口预览 |
| 2 | `pillars` | 双支柱：PasteForm（All in Dto）/ PasteApeart（All in Dto + All in Handler） |
| 3 | `evolution` | PasteForm 演进路径 |
| 4 | `problem` | 痛点 |
| 5 | `solution` | 方案对照 |
| 6 | `architecture` | 三层架构：`Host → Application → Handler` |
| 7 | `multihost` | 6 种宿主：HTTP / MQ / Timer / Job / Event / Background |
| 8 | `features` | 核心特性 |
| 9 | `attributes` | 特性系统，51 个特性、12 个分组，支持实时搜索 |
| 10 | `eventmodel` | 事件模型 IEventModel + 代码示例 + 事件流程 |
| 11 | `toolchain` | 工具链 |
| 12 | `howitworks` | 工作流 |
| 13 | `usecases` | 适用场景 |
| 14 | `ecosystem` | 生态与链接 |
| 15 | `cta` | CTA + Footer |

---

## 交互说明（`main.js`）

| 功能 | 说明 |
|------|------|
| 移动端导航 | 汉堡按钮展开/收起；支持点击外部、Esc、窗口 > 1080px 自动关闭 |
| 平滑滚动 | 锚点跳转带 72px 导航栏偏移 |
| 滚动入场 | `IntersectionObserver` 给 `.fade-up` 加 `.in`，逐个触发后自动取消观察 |
| 导航高亮 | 当前板块对应的导航项高亮 |
| 特性搜索 | 按名称 / 说明 / `data-keywords` 实时过滤；无结果显示空状态；一键清空 |
| 代码复制 | `navigator.clipboard`，不可用时回退 `execCommand('copy')` |
| 回顶按钮 | 滚动超过 400px 出现 |
| 年份 | Footer 自动取当前年份 |

无障碍：`nav-toggle` 带 `aria-expanded`，`prefers-reduced-motion: reduce` 下关闭所有过渡与位移动画。

---

## 外部依赖（CDN）

- Google Fonts：Inter、JetBrains Mono
- Bootstrap Icons 1.11.3
- Prism.js 1.29.0（代码高亮）

离线环境下以上三项会失效，页面结构与样式不受影响（图标会缺失、代码块无高亮）。

---

## 已验证

- `lessc` 编译 exit 0；`node --check main.js` exit 0
- 360 / 390 / 768 / 820 / 1024 / 1440 六个断点**均无页面级横向滚动**
- 控制台报错 0、页面异常 0
- 特性搜索：51 → 搜 `image` 剩 1 项且只显示 1 个分组 → 无匹配显示空状态 → 清空恢复 51 项
- 移动端汉堡菜单可正常展开

---

## 相关链接

- README：<https://gita.pastecode.cn/apeart/paste-apeart/full/master/readme.md>
- 特性文档：<https://gita.pastecode.cn/apeart/paste-apeart/full/master/framework/PasteApeart.Helper/docs/md.md>
- PasteForm 源码：<https://gitee.com/pastecode/paste-form>