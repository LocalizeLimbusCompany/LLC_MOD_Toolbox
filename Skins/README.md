# 皮肤系统使用说明

## 目录结构

```
Skins/
├── default/
│   └── skin.json
├── example/
│   ├── skin.json
│   └── images/
│       ├── custom_minimize_hover.png
│       └── custom_close_hover.png
└── your_skin/
    ├── skin.json
    └── ...
```

## 创建自定义皮肤

### 1. 创建皮肤目录

在 `Skins/` 目录下创建一个新文件夹，例如 `my_skin`。

### 2. 创建配置文件

在皮肤目录中创建 `skin.json` 文件：

```json
{
  "name": "my_skin",
  "displayName": "我的皮肤",
  "author": "你的名字",
  "version": "1.0.0",
  "images": {
    "ImageName1": "path/to/image1.png",
    "ImageName2": "path/to/image2.png"
  }
}
```

### 3. 配置说明

- **name**: 皮肤的唯一标识符（必须与文件夹名称一致）
- **displayName**: 显示给用户的皮肤名称
- **author**: 皮肤作者
- **version**: 皮肤版本号
- **images**: Image组件名称到图片路径的映射

### 4. 图片路径

图片路径可以是：
- **相对路径**: 相对于皮肤目录，例如 `"images/bg.png"` 指向 `Skins/my_skin/images/bg.png`
- **绝对路径**: 完整的文件系统路径（不推荐）

### 5. 查找Image组件名称

要替换某个图片，你需要知道对应Image组件的Name属性。可以通过以下方式查找：

1. 查看 `MainWindow.xaml` 文件
2. 找到 `<Image x:Name="XXX" ...>` 标签
3. 使用 `x:Name` 的值作为配置中的键

例如，在XAML中：
```xml
<Image x:Name="MinimizeHover" Source="/Picture/MinimizeHover.png" />
```

在skin.json中：
```json
{
  "images": {
    "MinimizeHover": "my_minimize_hover.png"
  }
}
```

## 使用皮肤

### 方法1: 通过配置文件

编辑 `config.json`，添加或修改：

```json
{
  "skin": {
    "currentSkin": "my_skin"
  }
}
```

重启应用程序即可应用皮肤。

### 方法2: 运行时切换（需要前端支持）

如果前端实现了皮肤切换UI，可以在运行时动态切换皮肤，无需重启。

## 回退机制

如果指定的皮肤加载失败（目录不存在、配置错误、图片缺失等），系统会自动回退到 `default` 皮肤。

如果某个图片在当前皮肤中不存在，系统会尝试从 `default` 皮肤加载该图片。如果 `default` 皮肤中也没有，则保持原有图片不变。

## 常见问题

### Q: 我的皮肤没有生效？

A: 检查以下几点：
1. 皮肤目录名称是否与 `skin.json` 中的 `name` 字段一致
2. `skin.json` 格式是否正确（可以用JSON验证工具检查）
3. 图片路径是否正确
4. 图片文件是否存在
5. 查看日志文件了解详细错误信息

### Q: 如何知道有哪些Image可以替换？

A: 查看 `MainWindow.xaml` 文件，搜索 `<Image x:Name=` 找到所有命名的Image组件。

### Q: 可以只替换部分图片吗？

A: 可以。你不需要在 `images` 中列出所有Image组件，只需要列出你想替换的即可。未列出的Image将保持原样或使用默认皮肤的图片。

### Q: 支持哪些图片格式？

A: 支持WPF支持的所有图片格式，包括 PNG、JPG、BMP、GIF 等。推荐使用PNG格式以获得最佳质量。

## 示例

参考 `Skins/example/` 目录查看完整的示例皮肤。
