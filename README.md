# 都市零协会工具箱 (LLC_MOD_Toolbox)

[![LLC_MOD_Toolbox](https://socialify.git.ci/LocalizeLimbusCompany/LLC_MOD_Toolbox/image?description=1&descriptionEditable=%E9%83%BD%E5%B8%82%E9%9B%B6%E5%8D%8F%E4%BC%9A%E5%B7%A5%E5%85%B7%E7%AE%B1&font=Inter&forks=1&issues=1&language=1&logo=https%3A%2F%2Fwww.zeroasso.top%2Fimg%2Flogo.png&name=1&owner=1&pattern=Circuit%20Board&pulls=1&stargazers=1&theme=Light)](https://github.com/LocalizeLimbusCompany/LLC_MOD_Toolbox)

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/LocalizeLimbusCompany/LLC_MOD_Toolbox)

**都市零协会工具箱 (LLC_MOD_Toolbox)** 是专为《边狱公司》（Limbus Company）零协会汉化模组（LocalizeLimbusCompany）设计的一款自动化安装与辅助工具。

本工具旨在为玩家提供便捷的一键式汉化安装、模组更新、皮肤切换、字体替换及游戏辅助功能，彻底解决手动安装 BepInEx 框架及汉化补丁的繁琐流程。

---

## 🌟 核心功能

*   **一键自动化安装**：自动检测 Steam 中的《边狱公司》游戏路径，一键部署 BepInEx 框架、汉化模组、汉化字体及所需依赖。
*   **多节点智能下载**：内置国内外多个高速下载节点，支持自动测速与最优节点选择，解决大文件（如中文字体包）下载缓慢的问题。
*   **个性化皮肤切换**：支持工具箱换肤功能（如默认皮肤、凯尔希皮肤、第七赛季主题皮肤等），玩家可随心定制工具箱外观。
*   **灰度/内测通道 (Greytest)**：为测试人员和抢先体验用户提供汉化模组灰度更新与切换通道。
*   **字体自定义替换**：支持一键将游戏内的默认汉化字体替换为第三方字体，满足玩家的个性化排版需求。
*   **Mirror酱镜像服务**：针对部分无法直接连接 GitHub/Steam 的海外或特定网络环境用户，提供专属的 Mirror 酱代理及镜像下载服务。
*   **内置小游戏 (抽卡模拟器)**：内置极具《边狱公司》特色的狂气抽卡模拟器与趣味彩蛋页面，让等待安装的过程不再枯燥。

---

## 💻 系统要求与依赖

*   **操作系统**：Windows 7 (SP1) / 8 / 10 / 11 (x64)
*   **运行环境**：[.NET 8.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0) 或更高版本
*   **游戏平台**：Steam 官方版《边狱公司》(Limbus Company)
*   **额外依赖**：工具箱自带 `7z.dll` 用于高效解压，请勿随意删除。

---

## 🚀 快速开始

### 1. 下载与运行
前往 [Releases 页面](https://github.com/LocalizeLimbusCompany/LLC_MOD_Toolbox/releases) 下载最新版本的工具箱压缩包，解压后双击运行 `LLC_MOD_Toolbox.exe`。

### 2. 自动定位游戏
工具箱在启动时会通过注册表或 Steam 默认路径自动检测您的游戏安装位置。如检测失败，您可以在**设置**中手动选择包含 `LimbusCompany.exe` 的文件夹。

### 3. 一键安装
在主界面点击**“安装”**按钮，工具箱将依次执行：
1. 下载并安装字体包（如果本地不存在）。
2. 下载并安装最新版 BepInEx 框架。
3. 下载并覆盖最新的零协会汉化模组文件。
4. 自动写入 `LLC_zh-CN` 语言配置。

---

## 🛠️ 项目结构

```txt
LLC_MOD_Toolbox/
├── Models/                 # 数据模型 (配置、节点、皮肤等)
├── ViewModels/             # WPF MVVM 设计模式的视图模型
├── Views/
│   └── Controls/           # 页面控件 (包含抽卡、设置、皮肤、灰度测试等)
├── Services/               # 核心业务逻辑
│   ├── Adaptation/         # 历史结构兼容适配
│   ├── Configuration/      # config.json 读写与配置管理
│   ├── Content/            # 公告与加载文本服务
│   ├── Font/               # 字体生成与处理服务
│   ├── Gacha/              # 抽卡模拟器逻辑
│   ├── Greytest/           # 灰度/测试通道逻辑
│   ├── Installation/       # 模组安装 (InstallService) 与卸载服务
│   ├── IO/                 # 文件读取与删除包装
│   ├── Network/            # HTTP 请求、测速与节点检测
│   ├── Skin/               # 皮肤资源管理与应用
│   └── System/             # 注册表检测与 Steam 路径寻找
├── Skins/                  # 预设皮肤配置文件夹
├── Fonts/                  # 预设字体资源及子集化字库
└── Pictures/               # UI 贴图与背景素材
```

---

## 🤝 贡献与感谢

都市零协会汉化模组及工具箱的顺利开发离不开以下团队成员及社区伙伴的大力支持：

### 🛠️ 工具箱开发组
*   **代码实现**：曾小皮、明（[ProjektMing](https://github.com/ProjektMing)）、10291029
*   **美术设计**：记得保存、玉佩（抽卡模拟器美术）、小迪（部分美术资源）
*   **服务器运维与开发**：曾小皮

### 📖 LLC 翻译组与汉化模组
*   **翻译文本**：零协会翻译组全体译者
*   **汉化模组代码**：奈芙 (Nephthys)
*   **汉化模组字体**：茜、北岚

### 💖 特别鸣谢
*   **零协会广播局** 全体创作者
*   **Paratranz** 提供的汉化翻译协作平台支持
*   **简幻欢** 提供的服务器赞助支持

> *人无语言则茫然无依。*

---

## 📄 开源协议与第三方库

本项目采用 **MIT License** 开源。
本软件引用的第三方组件与库的授权信息可在 [零协会官网](https://www.zeroasso.top) 进行查看，主要使用的 NuGet 包包括：
*   **Downloader** (高速多线程下载支持)
*   **Newtonsoft.Json** (配置文件序列化/反序列化)
*   **Squid-Box.SevenZipSharp** (基于 7z.dll 的高性能解包)
*   **log4net** (程序运行日志记录)

---

## 🌐 关联链接
*   **零协会官方网站**：[https://www.zeroasso.top](https://www.zeroasso.top)
*   **GitHub 组织仓库**：[https://github.com/LocalizeLimbusCompany](https://github.com/LocalizeLimbusCompany)
