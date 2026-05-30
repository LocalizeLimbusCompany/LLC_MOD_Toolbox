using LLC_MOD_Toolbox.Infrastructure;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services.Adaptation;
using LLC_MOD_Toolbox.Services.Installation;
using LLC_MOD_Toolbox.Services.Security;
using log4net.Config;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using static LLC_MOD_Toolbox.Services.Network.SimpleDnsChecker;

namespace LLC_MOD_Toolbox.ViewModels
{
    public sealed partial class MainWindowViewModel
    {
        public async Task InitializeAsync()
        {
            var startupStopwatch = Stopwatch.StartNew();
            XmlConfigurator.Configure();
            IsGlobalOperationsEnabled = false;
            Log.logger.Info("—————新日志分割线—————");
            Log.logger.Info("工具箱已进入加载流程。");
            Log.logger.Info("We have a lift off.");
            Log.logger.Info($"WPF架构工具箱 版本：{_appState.Version} 。");

            LoadConfiguredSkin();
            ApplySkinRequested?.Invoke();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            _mirrorChyanService.Initialize();
            IsMirrorChyanLogoVisible = _appState.IsMirrorChyanMode;
            MirrorChyanButtonText = _appState.IsMirrorChyanMode ? "禁用" : "填写秘钥";
            _nodeService.Initialize();

            if (_appState.IsMirrorChyanMode)
            {
                SetNodeOptions(["已使用Mirror酱"]);
                UpdateSelectedNodeOption("已使用Mirror酱");
                SetApiOptions(["已使用Mirror酱"]);
                UpdateSelectedApiOption("已使用Mirror酱");
            }
            else
            {
                SetNodeOptions(_nodeService.GetNodeOptions());
                SetApiOptions(_nodeService.GetApiOptions());
                _nodeService.ReadConfigNode();
                string savedNode = _config.Settings.nodeSelect.defaultNode;
                string savedApi = _config.Settings.nodeSelect.defaultApiNode;
                UpdateSelectedNodeOption(string.IsNullOrEmpty(savedNode) ? "恢复默认" : savedNode);
                UpdateSelectedApiOption(string.IsNullOrEmpty(savedApi) ? "恢复默认" : savedApi);
            }

            Task loadingTextTask = InitLoadingTextAsync();
            Task skinComboBoxTask = InitializeSkinComboBoxAsync();
            Task<Services.Update.ToolboxUpdateCheckResult> toolboxUpdateTask = _updateService.CheckForUpdateAsync();
            Task<Services.Content.AnnouncementResult> announcementTask = _announcementService.CheckForAnnouncementAsync();
            Task<bool> dnsCheckTask = _appState.IsLauncherMode ? Task.FromResult(false) : CheckForSuspiciousIpAsync("www.zeroasso.top");

            var updateResult = await toolboxUpdateTask;
            if (updateResult.HasUpdate)
            {
                bool updated = await _updateService.PerformUpdateAsync(updateResult);
                if (updated)
                {
                    Application.Current.Shutdown();
                    return;
                }
            }

            CheckLimbusCompanyPath();
            SevenZip.SevenZipBase.SetLibraryPath(Path.Combine(_appState.CurrentDir, "7z.dll"));

            Task<Services.Version.VersionCheckResult> versionTask = _versionService.CheckVersionsAsync();

            var annoResult = await announcementTask;
            await HandleAnnouncementAsync(annoResult);

            var versionResult = await versionTask;
            CurrentVersionText = versionResult.CurrentText;
            LatestVersionText = versionResult.LatestText;
            bool needUpdate = versionResult.NeedsUpdate;

            LegacyStructureAdapter.CheckAdapt(_appState.LimbusCompanyDir);

            if (!_appState.IsLauncherMode)
            {
                StartLoadingTextRotation();
                EasterEggImageRequested?.Invoke(
                    _config.Settings.general.internationalMode
                        ? "https://cdn-api.zeroasso.top/v2/eepic/get_image"
                        : "https://api.zeroasso.top/v2/eepic/get_image");
                await CheckModInstalled();
                bool hasSuspiciousIp = await dnsCheckTask;
                if (hasSuspiciousIp)
                    _dialogService.ShowMessage("警告！\n检测到您的DNS解析结果存在问题。\n您大概率无法使用工具箱。\n建议您更换DNS服务器后再使用工具箱。\n如果您不知道使用什么DNS服务器，请使用阿里云DNS。", "警告");
            }

            await Task.WhenAll(loadingTextTask, skinComboBoxTask);

            if (_appState.IsLauncherMode && !_hasNewAnno && !needUpdate)
            {
                try { OpenUrl("steam://rungameid/1973530"); } catch { }
                Environment.Exit(0);
            }

            if ((_config.Settings.install.installWhenLaunch || _appState.IsLauncherMode) && !_hasNewAnno && needUpdate)
                await ExecuteAutoInstallAsync();

            IsGlobalOperationsEnabled = true;
            Log.logger.Info($"启动总耗时: {startupStopwatch.ElapsedMilliseconds}ms");
            Log.logger.Info("加载流程完成。");
        }

        private async Task ExecuteAutoInstallAsync()
        {
            IsInstalling = true;
            Log.logger.Info("开始安装。");

            if (File.Exists(Path.Combine(_appState.LimbusCompanyDir, "version.dll")))
            {
                if (!_dialogService.ShowConfirm("检测到MelonLoader框架！\nMelonLoader框架已过时，且其可能导致您的账号遭到封禁，导致您无法进行游戏！\n建议您进行一次卸载后继续安装模组。\n若您**及其确定这是个误判**，请点击是，否则请点击否返回。", "警告"))
                {
                    await StopInstall();
                    return;
                }
            }
            if (File.Exists(Path.Combine(_appState.LimbusCompanyDir, "winhttp.dll")))
            {
                if (!_dialogService.ShowConfirm("检测到BepInEx框架（旧版本模组）！\n使用旧版本汉化模组可能遭到月亮计划的封禁！\n建议您进行一次卸载后继续安装模组。\n若您**及其确定这是个误判**，请点击\"是\"。", "警告"))
                {
                    await StopInstall();
                    return;
                }
            }

            Process[] limbusProcess = Process.GetProcessesByName("LimbusCompany");
            if (limbusProcess.Length > 0)
            {
                if (!_dialogService.ShowConfirm("检测到 Limbus Company 仍然处于开启状态！\n建议您关闭游戏后继续安装模组。\n若您已经关闭了 Limbus Company，请点击是。", "警告"))
                {
                    await StopInstall();
                    return;
                }
            }

            try
            {
                var progress = new Progress<InstallProgress>(p =>
                {
                    ProgressPercentage = (p.Phase - 1) * 50 + p.Percentage * 0.5f;
                });
                await _installService.InstallAsync(progress);
            }
            catch (Exception ex)
            {
                Log.logger.Error("安装出错。", ex);
                _dialogService.ShowMessage($"安装出错。\n您可以尝试在设置中切换节点。\n错误：{Services.Network.HttpService.GetExceptionText(ex)}", "错误");
                ProgressPercentage = 0;
                IsInstalling = false;
                return;
            }

            Log.logger.Info("安装完成。");

            try
            {
                var refreshedVersion = await _versionService.CheckVersionsAsync();
                CurrentVersionText = refreshedVersion.CurrentText;
                LatestVersionText = refreshedVersion.LatestText;
            }
            catch (Exception ex)
            {
                Log.logger.Error("安装后刷新版本信息失败。", ex);
            }

            if (_config.Settings.install.afterInstallClose || _appState.IsLauncherMode)
            {
                OpenUrl("steam://rungameid/1973530");
                Application.Current.Shutdown();
                return;
            }

            bool runResult = _dialogService.ShowConfirm("安装已完成！\n点击\"是\"立刻运行边狱公司。\n点击\"否\"关闭弹窗。", "提示");
            if (runResult)
            {
                try { OpenUrl("steam://rungameid/1973530"); }
                catch (Exception ex) { _dialogService.ShowMessage("出现了问题。\n" + ex.ToString(), "提示"); }
            }

            IsInstalling = false;
            ProgressPercentage = 0;
        }

        private async Task StopInstall()
        {
            await _installService.StopInstallAsync();
            IsInstalling = false;
            ProgressPercentage = 0;
        }

        private async Task ExecuteUninstallAsync()
        {
            if (!_dialogService.ShowConfirm("删除后你需要重新安装汉化补丁。\n确定继续吗？", "警告"))
                return;

            Log.logger.Info("确定删除模组。");
            try
            {
                IsGlobalOperationsEnabled = false;
                _uninstallService.UninstallAll();
                IsGlobalOperationsEnabled = true;
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage("删除过程中出现了一些问题： " + ex.ToString(), "警告");
            }
            _dialogService.ShowMessage("删除完成。", "提示");
            CurrentVersionText = "当前版本：未安装";
            Log.logger.Info("删除完成。");
        }

        private async Task HandleGachaNavigationAsync()
        {
            if (!_gachaService.IsInitialized)
            {
                bool confirmed = _dialogService.ShowConfirm("本抽卡模拟器资源来源自维基，可能信息更新不准时。\n本模拟器 不 会 对您的游戏数据造成任何影响。\n若您已知悉，请点击\"确定\"进行初始化。", "提示");
                if (confirmed)
                {
                    SelectInstallSubPage(InstallSubPage.Gacha);
                    IsGlobalOperationsEnabled = false;
                    await _gachaService.InitializeAsync();
                    IsGlobalOperationsEnabled = true;
                    if (_gachaService.InitializationFailed)
                        _dialogService.ShowMessage("初始化失败。请检查网络情况。");
                    else
                        _dialogService.ShowMessage("初始化完成。", "提示");
                }
            }
            else
            {
                SelectInstallSubPage(InstallSubPage.Gacha);
            }
        }

        private Task ExecuteGachaRollAsync()
        {
            if (_gachaService.InitializationFailed)
            {
                _dialogService.ShowMessage("初始化失败，无法进行抽卡操作。", "提示");
                return Task.CompletedTask;
            }
            // Gacha roll UI animation will be handled by MainWindow subscribing to a roll event
            GachaRollExecuted?.Invoke(_gachaService.Roll());
            return Task.CompletedTask;
        }

        public event Action<Services.Gacha.GachaRollResult>? GachaRollExecuted;

        public void ShowGachaMessage(string message, string title = "提示") =>
            _dialogService.ShowMessage(message, title);

        public event Action<string>? EasterEggImageRequested;

        private void LoadConfiguredSkin()
        {
            string skinName = _config.Settings.skin.currentSkin;
            if (string.IsNullOrWhiteSpace(skinName))
                skinName = "default";

            if (!_skinService.LoadSkin(skinName))
                Log.logger.Warn($"启动时加载皮肤失败: {skinName}");
        }

        private async Task HandleGreytestStartAsync()
        {
            IsGlobalOperationsEnabled = false;
            var result = await _greytestService.ValidateAndActivateAsync(GreytestToken);
            if (!result.IsValid)
            {
                _dialogService.ShowMessage(result.ErrorMessage ?? "验证失败。", "提示");
                IsGlobalOperationsEnabled = true;
                return;
            }
            if (_appState.GreytestStatus)
            {
                IsGreytestLogoVisible = true;
                _dialogService.ShowMessage($"目前Token有效。\n灰度测试模式已开启。\n备注：{result.Note}\n请在自动安装安装此秘钥对应版本汉化。", "提示");
            }
            IsGlobalOperationsEnabled = true;
        }

        private void HandleNodeSelectionChanged(string? nodeName)
        {
            if (_appState.IsMirrorChyanMode) return;
            Log.logger.Info("选择节点。");
            if (string.IsNullOrEmpty(nodeName)) return;

            if (nodeName == "Github直连")
            {
                _dialogService.ShowMessage("如果您没有使用代理软件（包括Watt Toolkit）\n请不要使用此节点。", "警告");
            }
            _nodeService.SelectDownloadNode(nodeName);
            if (nodeName != "恢复默认" && nodeName != "Github直连")
                _dialogService.ShowMessage("切换成功。", "提示");
        }

        private void HandleApiSelectionChanged(string? apiName)
        {
            if (_appState.IsMirrorChyanMode) return;
            if (_nodeService.UseGithub)
            {
                UpdateSelectedApiOption("恢复默认");
                _dialogService.ShowMessage("切换失败。\n无法在节点为Github直连的情况下切换API。", "提示");
                return;
            }
            Log.logger.Info("选择API节点。");
            if (string.IsNullOrEmpty(apiName)) return;
            _nodeService.SelectApiNode(apiName);
            if (apiName != "恢复默认")
                _dialogService.ShowMessage("切换成功。", "提示");
        }

        private async void HandleSkinSelectionChanged(SkinCatalogItem? selectedSkin)
        {
            try
            {
                if (selectedSkin == null || string.IsNullOrWhiteSpace(selectedSkin.name)) return;
                Log.logger.Info($"选择皮肤: {selectedSkin.DisplayText}");

                if (!selectedSkin.isInstalled)
                {
                    bool installed = await _skinService.InstallSkinFromServerAsync(selectedSkin.name);
                    if (!installed)
                    {
                        _dialogService.ShowMessage("皮肤安装失败。");
                        return;
                    }
                    _dialogService.ShowMessage("皮肤安装完成。");
                    await InitializeSkinComboBoxAsync(selectedSkin.name);
                    return;
                }

                bool success = _skinService.LoadSkin(selectedSkin.name);
                if (success)
                {
                    ApplySkinRequested?.Invoke();
                    var skinInfo = _skinService.CurrentSkinInfo ?? _skinService.GetSkinInfo(selectedSkin.name);
                    if (skinInfo != null)
                        SkinDescription = (skinInfo.desc ?? "暂无描述。").Replace("\\n", "\n");
                    _config.Settings.skin.currentSkin = selectedSkin.name;
                    _config.SaveConfig();
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error($"切换皮肤时出错: {ex.Message}");
            }
        }

        private async Task HandleAnnouncementAsync(Services.Content.AnnouncementResult result)
        {
            if (result.Error != null || !result.HasNew) return;

            ArePrimaryActionsEnabled = false;
            AnnouncementText = result.Content;
            _announcementService.MarkAsRead(result.Version);
            _isInAnno = true;
            _hasNewAnno = true;
            IsAnnouncementButtonEnabled = false;
            ShowAnnouncementTip = true;
            SelectMainPage(MainPage.Announcement);

            if (result.Level == "normal")
            {
                IsAnnouncementButtonEnabled = true;
                ShowAnnouncementTip = false;
                return;
            }
            _annoLastTime = result.Level switch
            {
                "normal" => 0,
                "important" => 5,
                "special" => 15,
                _ => 0,
            };
            AnnouncementTip = "由于本次公告较为重要，您需要继续阅读" + _annoLastTime + "秒。";
            _annoTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _annoTimer.Tick += AnnoTimerTick;
            _annoTimer.Start();
        }

        private void AnnoTimerTick(object? sender, EventArgs e)
        {
            if (_annoLastTime > 0)
            {
                _annoLastTime--;
                AnnouncementTip = "由于本次公告较为重要，您需要继续阅读" + _annoLastTime + "秒。";
            }
            else
            {
                _isInAnno = false;
                IsAnnouncementButtonEnabled = true;
                ShowAnnouncementTip = false;
                _annoTimer?.Stop();
            }
        }

        private async Task AcknowledgeAnnouncementAsync()
        {
            SelectMainPage(MainPage.Install, InstallSubPage.Auto);
            ArePrimaryActionsEnabled = true;
            if (_config.Settings.install.installWhenLaunch || _appState.IsLauncherMode)
                await ExecuteAutoInstallAsync();
        }

        private void HandleMirrorChyanConfig()
        {
            if (_appState.IsMirrorChyanMode)
            {
                if (!_dialogService.ShowConfirm("确定要禁用Mirror酱吗？\n关闭后，你可以在设置重新开启Mirror酱的服务。", "提示"))
                    return;
                SecureStringStorage.DeleteSecretFile();
                _mirrorChyanService.DisableMode();
                _dialogService.ShowMessage("已禁用Mirror酱并删除你的Mirror酱CDK。\n为了处理，软件将关闭，再次启动后效果生效。", "提示");
                Application.Current.Shutdown();
            }
            else
            {
                var result = _dialogService.ShowInput("请输入你的 Mirror 酱 CDK。\n你可以在 Mirror 酱官网购买。", "输入秘钥", "Mirror 酱 CDK", InputType.Password,
                    [new DialogButton("确定", true, false), new DialogButton("取消", false, true)]);
                if (result.IsCanceled) return;
                if (result.IsSuccess && !string.IsNullOrEmpty(result.Input))
                {
                    _mirrorChyanService.SetupMode(result.Input);
                    _dialogService.ShowMessage("Mirror酱秘钥设置成功。\n为了处理，软件将关闭，再次启动后效果生效。", "提示");
                    Application.Current.Shutdown();
                }
                else
                {
                    _dialogService.ShowMessage("设置失败。", "提示");
                }
            }
        }

        private void OnMirrorChyanModeDisabled()
        {
            IsMirrorChyanLogoVisible = false;
            MirrorChyanButtonText = "填写秘钥";
            _nodeService.Initialize();
            SetNodeOptions(_nodeService.GetNodeOptions());
            SetApiOptions(_nodeService.GetApiOptions());
            _nodeService.ReadConfigNode();
            string savedNode = _config.Settings.nodeSelect.defaultNode;
            string savedApi = _config.Settings.nodeSelect.defaultApiNode;
            UpdateSelectedNodeOption(string.IsNullOrEmpty(savedNode) ? "恢复默认" : savedNode);
            UpdateSelectedApiOption(string.IsNullOrEmpty(savedApi) ? "恢复默认" : savedApi);
        }

        private void HandleLauncherShortcut()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcutPath = Path.Combine(desktopPath, "LimbusCompany with LLC.lnk");
            IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(shortcutPath);
            shortcut.TargetPath = Path.Combine(_appState.CurrentDir, "LLC_MOD_Toolbox.exe");
            shortcut.Arguments = "-launcher";
            shortcut.WorkingDirectory = _appState.CurrentDir;
            shortcut.Description = "启动边狱公司并检查汉化更新";
            shortcut.IconLocation = Path.Combine(_appState.CurrentDir, "PublicResource", "favicon.ico");
            shortcut.Save();
            _dialogService.ShowMessage("快捷方式已创建。\n可在桌面上找到\"LimbusCompany with LLC\"启动。", "提示");
        }

        private void HandleHotUpdateHelp()
        {
            _dialogService.ShowMessage("你想要知道怎么用热更新？那你可找对地方了兄弟！\n你现在有两种方式，随你便：\n1. 从快捷方式启动\n2. 从Steam启动\n详细请参阅官网文档。", "热更新教程");
        }

        private void HandleExploreFont()
        {
            string? path = _dialogService.ShowOpenFileDialog("请选择你的字体", "字体文件 (*.ttf;*.otf)|*.ttf;*.otf|所有文件 (*.*)|*.*");
            if (path != null)
                FontReplacePath = Path.GetFullPath(path);
        }

        private Task HandlePreviewFontAsync()
        {
            if (!double.TryParse(FontSizeText, out _))
            {
                _dialogService.ShowMessage("请输入正确的字体大小。", "提示");
                return Task.CompletedTask;
            }
            if (!_fontService.IsValidFontFile(FontReplacePath))
            {
                _dialogService.ShowMessage("请选择正确的字体文件。", "提示");
                return Task.CompletedTask;
            }
            // Preview font requires Window access - signal via event
            FontPreviewRequested?.Invoke(FontReplacePath, double.Parse(FontSizeText));
            _dialogService.ShowMessage("已将预览文本切换为自定义字体。\n如果出现部分字显示为默认字体，可能影响游戏内显示。", "提示");
            return Task.CompletedTask;
        }

        public event Action<string, double>? FontPreviewRequested;

        private void HandleApplyFont()
        {
            if (!_fontService.IsValidFontFile(FontReplacePath))
            {
                _dialogService.ShowMessage("字体文件无效。", "提示");
                return;
            }
            try
            {
                _fontService.ApplyFontReplacement(FontReplacePath, _appState.LimbusCompanyDir);
                _dialogService.ShowMessage("字体替换成功。\n启动游戏以应用更改。", "提示");
            }
            catch (InvalidOperationException ex)
            {
                _dialogService.ShowMessage(ex.Message, "提示");
            }
        }

        private void HandleRestoreFont()
        {
            try
            {
                _fontService.RestoreOriginalFont(_appState.LimbusCompanyDir);
                _dialogService.ShowMessage("字体还原成功。\n启动游戏以应用更改。", "提示");
            }
            catch (FileNotFoundException)
            {
                _dialogService.ShowMessage("没有找到备份字体文件。", "提示");
            }
        }

        private async Task InitLoadingTextAsync()
        {
            await _loadingTextService.InitializeAsync();
            LoadingText = _loadingTextService.GetRandomLoadingText();
        }

        private void StartLoadingTextRotation()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            timer.Tick += (_, _) => LoadingText = _loadingTextService.GetRandomLoadingText();
            timer.Start();
        }

        private async Task InitializeSkinComboBoxAsync(string? preferredSkinName = null)
        {
            try
            {
                List<SkinDefinition> remoteSkins = [];
                try { remoteSkins = await _skinService.GetRemoteSkinDefinitionsAsync(); }
                catch (Exception ex) { Log.logger.Warn($"获取远端皮肤列表失败: {ex.Message}"); }

                var localSkinNames = _skinService.GetAvailableSkins();
                var skinOptions = _skinService.BuildSkinCatalog(remoteSkins, localSkinNames);
                SetSkinOptions(skinOptions);

                SkinCatalogItem? selectedOption = null;
                if (!string.IsNullOrWhiteSpace(preferredSkinName))
                    selectedOption = SkinOptions.FirstOrDefault(s => s.name == preferredSkinName);
                selectedOption ??= SkinOptions.FirstOrDefault(s => s.name == _skinService.CurrentSkinInfo?.name);
                selectedOption ??= SkinOptions.FirstOrDefault(s => s.name == _config.Settings.skin.currentSkin);
                selectedOption ??= SkinOptions.FirstOrDefault(s => s.name == "default");

                if (selectedOption != null)
                {
                    UpdateSelectedSkinOption(selectedOption);
                    SkinDescription = (string.IsNullOrEmpty(selectedOption.desc) ? "暂无描述。" : selectedOption.desc).Replace("\\n", "\n");
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error($"初始化皮肤选择框失败: {ex.Message}");
            }
        }

        private async Task CheckModInstalled()
        {
            try
            {
                string fontDir = Path.Combine(_appState.LimbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font");
                if (Directory.Exists(fontDir))
                    Log.logger.Info("模组已安装。");
                else
                    Log.logger.Info("模组未安装。");
            }
            catch (Exception ex)
            {
                Log.logger.Error("出现问题。" + ex.ToString());
            }
        }

        private void CheckLimbusCompanyPath()
        {
            string? configPath = _config.Settings.general.LCBPath;
            string limbusDir = string.Empty;

            if (!string.IsNullOrWhiteSpace(configPath))
                limbusDir = configPath;
            else
            {
                limbusDir = Microsoft.Win32.Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1973530",
                    "InstallLocation", null) as string ?? string.Empty;
            }

            if (!IsValidLimbusDir(limbusDir))
            {
                try
                {
                    limbusDir = SteamLocator.FindLimbusCompanyPath("1973530", "LimbusCompany.exe");
                }
                catch { limbusDir = string.Empty; }
            }

            if (IsValidLimbusDir(limbusDir))
            {
                bool confirmed = _config.Settings.general.skipLCBPathCheck ||
                    _dialogService.ShowConfirm($"这是您的边狱公司地址吗？\n{limbusDir}", "检查路径");
                if (confirmed)
                {
                    _appState.LimbusCompanyDir = limbusDir;
                    _appState.LimbusCompanyGameDir = Path.Combine(limbusDir, "LimbusCompany.exe");
                    _config.Settings.general.LCBPath = limbusDir;
                    _config.Settings.general.skipLCBPathCheck = true;
                    _config.SaveConfig();
                    Log.logger.Info("边狱公司路径：" + limbusDir);
                    return;
                }
            }

            string? selected = _dialogService.ShowOpenFileDialog("请选择你的边狱公司游戏文件", "LimbusCompany.exe|LimbusCompany.exe", limbusDir);
            if (selected != null)
            {
                limbusDir = Path.GetDirectoryName(selected) ?? string.Empty;
            }

            if (!IsValidLimbusDir(limbusDir))
            {
                _dialogService.ShowMessage("选择目录有误，没有在当前目录找到游戏。", "错误");
                Application.Current.Shutdown();
                return;
            }

            _appState.LimbusCompanyDir = limbusDir;
            _appState.LimbusCompanyGameDir = Path.Combine(limbusDir, "LimbusCompany.exe");
            _config.Settings.general.LCBPath = limbusDir;
            _config.Settings.general.skipLCBPathCheck = true;
            _config.SaveConfig();
            Log.logger.Info("边狱公司路径：" + limbusDir);
        }

        private static bool IsValidLimbusDir(string? directory)
        {
            if (string.IsNullOrWhiteSpace(directory)) return false;
            return File.Exists(Path.Combine(directory, "LimbusCompany.exe")) &&
                   File.Exists(Path.Combine(directory, "LimbusCompany_Data", "resources.assets"));
        }
    }
}
