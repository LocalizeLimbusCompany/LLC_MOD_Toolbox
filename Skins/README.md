# 皮肤系统使用说明

皮肤系统 v2 在完整兼容旧版 `images`、`visibility`、`margins` 的基础上，支持修改现有元素的通用属性，以及向指定页面添加任意数量的静态图片。

## 目录结构

```text
Skins/
├── default/
│   └── skin.json
└── my_skin/
    ├── skin.json
    └── images/
        ├── background.png
        └── decoration.png
```

`name` 必须是皮肤唯一标识，通常应与皮肤目录名相同。`displayName`、`author`、`version` 和 `desc` 用于皮肤选择界面展示。

## v2 完整示例

```json
{
  "schemaVersion": 2,
  "name": "my_skin",
  "displayName": "我的皮肤",
  "author": "你的名字",
  "version": "2.0.0",
  "desc": "支持通用属性和动态图片。",
  "images": {
    "Background": "images/background.png"
  },
  "visibility": {
    "ZALogo": false
  },
  "margins": {
    "TitleBar": "0,13,0,0"
  },
  "elements": {
    "autoInstall/LoadingText": {
      "fill": "#FF9BCB45",
      "stroke": "Black",
      "strokeThickness": 1.5,
      "fontFamily": "/Fonts/subset/#Source Han Sans CN",
      "fontSize": 18,
      "fontWeight": "Heavy",
      "width": 700,
      "height": 32,
      "margin": [0, 340, 0, 0]
    },
    "window/TitleBar": {
      "source": "images/titlebar.png",
      "width": 960,
      "height": 125,
      "stretch": "Fill"
    }
  },
  "dynamicImages": [
    {
      "name": "AutoInstallDecoration",
      "host": "autoInstall",
      "source": "images/decoration.png",
      "properties": {
        "width": 65,
        "height": 100,
        "margin": [20, 160, 0, 0],
        "horizontalAlignment": "Left",
        "verticalAlignment": "Top",
        "opacity": 1,
        "zIndex": 6
      }
    }
  ]
}
```

`schemaVersion` 可省略；旧皮肤无需修改即可继续加载。若同一元素的同一属性同时出现在旧字段和 `elements` 中，`elements` 优先。例如 `images.TitleBar` 会被 `elements["TitleBar"].source` 覆盖。

## 元素定位

`elements` 的键支持两种写法：

- `LoadingText`：在整个窗口中查找。该名称必须唯一。
- `autoInstall/LoadingText`：只在 `autoInstall` 宿主中查找，适合处理不同页面中的同名元素。

元素名区分大小写。找不到 v2 元素、简单名称不唯一或宿主不存在时，整次加载会失败并保留上一次成功效果。为了兼容历史皮肤，旧字段中已经从界面移除的元素只会产生警告并跳过。

### 可用宿主

| 宿主 | 范围 |
| --- | --- |
| `window` | 主窗口及其全部内容 |
| `navigation` | 左侧导航菜单 |
| `installTabs` | 安装、字体、皮肤、抽卡分页栏 |
| `autoInstall` | 自动安装页 |
| `fontReplace` | 字体替换页 |
| `skin` | 皮肤页 |
| `gacha` | 抽卡模拟页 |
| `link` | 常用链接页 |
| `greytest` | 灰度测试页 |
| `settings` | 设置页 |
| `about` | 关于页 |
| `easterEgg` | 彩蛋页 |
| `announcement` | 公告页 |

## 支持的属性

所有属性均经过白名单转换，不支持通过 JSON 设置事件、命令、`DataContext` 或 Binding。

| 分类 | 属性 | 值示例 |
| --- | --- | --- |
| 尺寸 | `width`、`height`、`minWidth`、`minHeight`、`maxWidth`、`maxHeight` | `320` |
| 间距 | `margin`、`padding` | `[10, 20, 10, 20]` 或 `"10,20,10,20"` |
| 布局 | `horizontalAlignment`、`verticalAlignment` | `"Left"`、`"Top"` |
| 显示 | `opacity`、`visibility`、`zIndex` | `0.8`、`"Collapsed"`、`5` |
| Grid | `gridRow`、`gridColumn`、`gridRowSpan`、`gridColumnSpan` | `0`、`2` |
| 字体 | `fontFamily`、`fontSize`、`fontWeight`、`fontStyle` | `"Arial"`、`18`、`"Bold"`、`"Italic"` |
| 颜色 | `foreground`、`background` | `"#FFFFEED4"`、`"Black"` |
| 边框 | `borderBrush`、`borderThickness` | `"Red"`、`[1, 2, 1, 2]` |
| 内容 | `content` | `"安装"`、`""` 或 `null` |
| Image | `source`、`stretch`、`stretchDirection` | `"images/a.png"`、`"UniformToFill"`、`"Both"` |
| OutlinedTextControl | `fill`、`stroke`、`strokeThickness`、`textAlignment`，以及字体属性 | `"White"`、`"Black"`、`2`、`"Center"` |

Brush 接受 WPF 命名颜色和十六进制颜色。`Thickness` 接受 1、2、4 个数字：一个数字表示四边相同，两个数字表示“水平、垂直”，四个数字依次表示“左、上、右、下”。

属性必须适用于目标控件。例如 `source` 只能用于 Image，`fill` 和 `stroke` 只能用于 `OutlinedTextControl`。类型错误会在弹窗中显示具体 JSON 路径。

`content` 仅适用于 Button、Label 等 `ContentControl`，并且只接受字符串或 `null`。它不能创建控件、设置 Binding、命令、事件或任意对象。切换皮肤时，原始 Content 及其 Binding 会正常恢复。

## 动态图片

`dynamicImages` 只创建静态 Image：

- `name` 在当前皮肤内必须唯一，并且不能与现有元素重名。
- `host` 决定图片加入哪个页面的根 Panel，图片会自然跟随该页面显示或隐藏。
- `source` 为图片路径。
- `properties` 使用上表中的通用属性和 Image 属性，但 `source` 必须写在外层。
- 动态图片默认 `IsHitTestVisible=false`，不能绑定点击、命令、事件或动画。
- 切换或重载皮肤时，旧动态图片会先全部移除，因此不会重复创建。

## 素材路径

- `"images/bg.png"`：相对于当前皮肤目录。
- `"/Picture/Background.png"`：相对于工具箱程序目录。
- `"D:\\Skins\\bg.png"`：绝对路径，不推荐分发皮肤时使用。

配置加载时会检查素材是否存在；实际应用前还会解码图片。路径错误或图片损坏都不会破坏当前界面。

## 应用顺序与恢复

每次切换或重载都使用固定顺序：

1. 恢复 XAML 原始值和原始 Binding。
2. 应用 `default` 皮肤。
3. 应用当前皮肤。

系统按依赖属性保存原始本地值和 Binding，因此上一套皮肤修改的尺寸、颜色、可见性等不会残留。JSON 解析、属性转换、元素定位和素材预加载全部成功后才会替换当前配置；失败时继续显示最后一次成功效果。

## Debug 热重载

仅 `Debug` 构建启用皮肤开发工具：

- 自动监听当前 `skin.json` 和已解析素材，400ms 防抖。
- 支持直接覆盖文件，以及编辑器通过临时文件重命名的原子保存方式。
- 皮肤页提供“重载皮肤”按钮和监听状态。
- `Ctrl+Shift+R` 可手动重载。
- 皮肤页的“导航判定框”可显示导航按钮点击区域（红框）和 Hover 图片区域（绿框），热重载后会自动刷新。
- 工具箱内部保存皮肤音乐开关时不会触发一次无意义重载。

`Release`、`LLC_MOD_Toolbox` 和 `ReleaseLLCMT` 构建不会启动监听器，也不会显示调试入口。

热重载失败时不会自动弹窗，以免打断连续编辑。皮肤页会显示失败状态，并在“重载皮肤”旁启用“失败日志”按钮；其中保留最近 10 条去重记录，包括时间、JSON 路径和具体原因。修复并成功重载后仍可查看历史记录。

## 旧版字段

旧皮肤仍可继续使用：

```json
{
  "images": {
    "MinimizeHover": "images/minimize_hover.png"
  },
  "visibility": {
    "ZALogo": false
  },
  "margins": {
    "TitleBar": "0,13,0,0"
  }
}
```

它们会分别规范化为 `source`、`visibility`、`margin` 属性，然后进入与 v2 相同的恢复和应用流程。
