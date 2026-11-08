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

模块职责、依赖关系与命名约定见 [架构与命名](docs/architecture.md)。启动流程、性能瓶颈与验证方法见 [性能与架构排查](docs/performance-review.md)。

```txt
LLC_MOD_Toolbox/
├── App.xaml.cs             # 应用生命周期、协议与窗口启动
├── MainWindow.xaml         # 主窗口及控件宿主
├── Infrastructure/         # MVVM 基础类型、诊断
│   └── DependencyInjection/# 集中的服务注册入口
├── Models/                 # 运行状态、节点、皮肤等数据
│   └── Configuration/      # 配置模型及旧 JSON 字段映射
├── ViewModels/
│   ├── AnnouncementViewModel.cs # 公告状态与阅读倒计时
│   ├── MainWindow/         # 主窗口编排，按功能组织 partial
│   └── Navigation/         # 页面与安装子页枚举
├── Views/
│   ├── Controls/           # 页面控件
│   └── Dialogs/            # 协议、通用输入、Mirror 酱、更新对话框
├── Services/               # 按功能分组，接口与实现各自独立文件
│   ├── Configuration/      # IConfigurationService / ConfigurationService
│   ├── Content/            # 公告、加载文案
│   ├── Installation/       # 模组安装与卸载
│   ├── Network/            # HTTP、节点、Mirror 酱、DNS 检查
│   ├── Skin/               # 换肤、热重载、背景音乐
│   ├── Update/             # 工具箱自身更新
│   ├── Version/            # 模组版本检查（ModVersionService）
│   └── ...                 # 字体、抽卡、灰度、文件、遥测等服务
├── tests/StartupChecks/    # 跨平台的服务行为回归检查
├── docs/                   # 架构与验证说明
├── Skins/                  # 皮肤配置
├── Fonts/                  # 字体资源及子集
└── Picture/                # UI 贴图与背景素材
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
