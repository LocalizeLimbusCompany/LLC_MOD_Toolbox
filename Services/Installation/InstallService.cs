using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.IO;
using LLC_MOD_Toolbox.Services.Network;
using LLC_MOD_Toolbox.Services.UI;
using Newtonsoft.Json.Linq;
using System.IO;

namespace LLC_MOD_Toolbox.Services.Installation
{
    public sealed class InstallService : IInstallService
    {
        private readonly AppState _appState;
        private readonly IHttpService _httpService;
        private readonly INodeService _nodeService;
        private readonly IMirrorChyanService _mirrorChyanService;
        private readonly IFileService _fileService;
        private readonly IDialogService _dialogService;
        private readonly ConfigurationManager _config;
        private JObject? _hashCacheObject;
        private bool _isStopped;

        public InstallService(
            AppState appState,
            IHttpService httpService,
            INodeService nodeService,
            IMirrorChyanService mirrorChyanService,
            IFileService fileService,
            IDialogService dialogService,
            ConfigurationManager config)
        {
            _appState = appState;
            _httpService = httpService;
            _nodeService = nodeService;
            _mirrorChyanService = mirrorChyanService;
            _fileService = fileService;
            _dialogService = dialogService;
            _config = config;
        }

        public async Task InstallAsync(IProgress<InstallProgress> progress, CancellationToken ct = default)
        {
            _isStopped = false;
            _hashCacheObject = null;

            if (_appState.GreytestStatus)
            {
                await InstallGreytestMod(progress);
                WriteLCBLangConfig("LLC_zh-CN");
                return;
            }

            if (!_appState.IsMirrorChyanMode)
            {
                await CacheHash();
                if (_isStopped) return;
            }

            await InstallFont(progress);
            if (_isStopped) return;

            await InstallMod(progress);
            if (_isStopped) return;

            WriteLCBLangConfig("LLC_zh-CN");
            _hashCacheObject = null;
        }

        public Task StopInstallAsync()
        {
            _isStopped = true;
            string dir = _appState.LimbusCompanyDir;
            _fileService.DeleteFile(Path.Combine(dir, "BepInEx-IL2CPP-x64.7z"));
            _fileService.DeleteFile(Path.Combine(dir, "tmpchinesefont_BIE.7z"));
            _fileService.DeleteFile(Path.Combine(dir, "LimbusLocalize_BIE.7z"));
            _fileService.DeleteFile(Path.Combine(dir, "LimbusLocalize_Dev.7z"));
            _hashCacheObject = null;
            return Task.CompletedTask;
        }

        private async Task InstallFont(IProgress<InstallProgress> progress)
        {
            await Task.Run(async () =>
            {
                Log.logger.Info("正在安装字体文件。");
                progress.Report(new InstallProgress(1, 0));
                string fontDir = Path.Combine(_appState.LimbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context");
                Directory.CreateDirectory(fontDir);
                string fontZIPFile = Path.Combine(_appState.LimbusCompanyDir, "LLCCN-Font.7z");
                string fontChinese = Path.Combine(fontDir, "ChineseFont.ttf");
                string fontBackup = Path.Combine(_appState.LimbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "BackupFont", "ChineseFont.ttf.bak");

                if (File.Exists(fontChinese) || File.Exists(fontBackup))
                {
                    Log.logger.Info("检测到已安装字体文件。");
                    return;
                }

                if (_appState.IsMirrorChyanMode)
                {
                    var (url, sha256) = await _mirrorChyanService.GetFontInfoAsync();
                    if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(sha256))
                    {
                        await StopInstallAsync();
                        return;
                    }
                    await _httpService.DownloadFileAsync(url, fontZIPFile, new Progress<float>(p => progress.Report(new InstallProgress(1, p))));
                    if (_fileService.CalculateSHA256(fontZIPFile) != sha256)
                    {
                        Log.logger.Error("字体哈希校验失败。");
                        _dialogService.ShowMessage("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                        await StopInstallAsync();
                        return;
                    }
                }
                else
                {
                    string downloadUrl = _nodeService.UseGithub
                        ? "https://raw.githubusercontent.com/LocalizeLimbusCompany/LocalizeLimbusCompany/refs/heads/main/Fonts/LLCCN-Font.7z"
                        : _nodeService.ResolveDownloadUrl("LLCCN-Font.7z");
                    await _httpService.DownloadFileAsync(downloadUrl, fontZIPFile, new Progress<float>(p => progress.Report(new InstallProgress(1, p))));
                    if (_fileService.CalculateSHA256(fontZIPFile) != _hashCacheObject!["font_hash"]!.Value<string>())
                    {
                        Log.logger.Error("字体哈希校验失败。");
                        _dialogService.ShowMessage("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                        await StopInstallAsync();
                        return;
                    }
                }

                Log.logger.Info("解压字体包中。");
                _fileService.ExtractArchive(fontZIPFile, _appState.LimbusCompanyDir);
                Log.logger.Info("删除字体包。");
                File.Delete(fontZIPFile);
            });
        }

        private async Task InstallMod(IProgress<InstallProgress> progress)
        {
            await Task.Run(async () =>
            {
                Log.logger.Info("开始安装模组。");
                progress.Report(new InstallProgress(2, 0));
                string langDir = Path.Combine(_appState.LimbusCompanyDir, "LimbusCompany_Data/Lang/LLC_zh-CN");
                string versionJsonPath = Path.Combine(langDir, "Info", "version.json");
                string limbusLocalizeZipPath = Path.Combine(_appState.LimbusCompanyDir, "LimbusLocalize.7z");

                int latestVersion = -1;
                int currentVersion = -1;
                bool needInstall = false;

                if (!File.Exists(versionJsonPath))
                {
                    Log.logger.Info("模组不存在。开始安装。");
                    needInstall = true;
                }

                if (!needInstall)
                {
                    latestVersion = _appState.IsMirrorChyanMode
                        ? await _mirrorChyanService.GetLatestModVersionAsync()
                        : await GetLatestModVersion();
                    if (latestVersion == -100)
                    {
                        await StopInstallAsync();
                        return;
                    }
                    Log.logger.Info("最后模组版本： " + latestVersion);
                    var versionObj = JObject.Parse(File.ReadAllText(versionJsonPath));
                    currentVersion = versionObj["version"]!.Value<int>();
                    Log.logger.Info("当前模组版本： " + currentVersion);
                    if (currentVersion >= latestVersion)
                    {
                        Log.logger.Info("模组无需更新。");
                        return;
                    }
                    needInstall = true;
                    Log.logger.Info("模组需要更新。进行安装。");
                }

                if (!needInstall) return;

                if (_appState.IsMirrorChyanMode)
                {
                    var (version, url, sha256) = await _mirrorChyanService.GetLatestModInfoAsync();
                    latestVersion = version;
                    await _httpService.DownloadFileAsync(url, limbusLocalizeZipPath, new Progress<float>(p => progress.Report(new InstallProgress(2, p))));
                    if (sha256 != _fileService.CalculateSHA256(limbusLocalizeZipPath))
                    {
                        Log.logger.Error("校验Hash失败。");
                        _dialogService.ShowMessage("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                        await StopInstallAsync();
                        return;
                    }
                }
                else if (_nodeService.UseGithub)
                {
                    latestVersion = await GetLatestModVersion();
                    await _httpService.DownloadFileAsync(
                        $"https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/{latestVersion}/LimbusLocalize_{latestVersion}.7z",
                        limbusLocalizeZipPath,
                        new Progress<float>(p => progress.Report(new InstallProgress(2, p))));
                }
                else
                {
                    latestVersion = await GetLatestModVersion();
                    string downloadUrl = _nodeService.ResolveDownloadUrl($"LimbusLocalize_{latestVersion}.7z");
                    await _httpService.DownloadFileAsync(downloadUrl, limbusLocalizeZipPath, new Progress<float>(p => progress.Report(new InstallProgress(2, p))));
                    if (_hashCacheObject!["main_hash"]!.Value<string>() != _fileService.CalculateSHA256(limbusLocalizeZipPath))
                    {
                        Log.logger.Error("校验Hash失败。");
                        _dialogService.ShowMessage("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
                        await StopInstallAsync();
                        return;
                    }
                }

                Log.logger.Info("解压模组本体 zip 中。");
                _fileService.ExtractArchive(limbusLocalizeZipPath, _appState.LimbusCompanyDir);
                Log.logger.Info("删除模组本体 zip 。");
                File.Delete(limbusLocalizeZipPath);
            });
        }

        private async Task InstallGreytestMod(IProgress<InstallProgress> progress)
        {
            await Task.Run(async () =>
            {
                Log.logger.Info("灰度测试模式已开启。开始安装灰度模组。");
                progress.Report(new InstallProgress(2, 0));
                string zipPath = Path.Combine(_appState.LimbusCompanyDir, "LimbusLocalize_Dev.7z");
                await _httpService.DownloadFileAsync(_appState.GreytestUrl, zipPath, new Progress<float>(p => progress.Report(new InstallProgress(2, p))));
                _fileService.ExtractArchive(zipPath, _appState.LimbusCompanyDir);
                File.Delete(zipPath);
                Log.logger.Info("灰度模组安装完成。");
            });
        }

        private async Task<int> GetLatestModVersion()
        {
            try
            {
                Log.logger.Info("获取模组标签。");
                string raw;
                string version;
                if (_nodeService.UseGithub)
                {
                    raw = await _httpService.GetTextAsync("https://api.github.com/repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/latest");
                    var json = JObject.Parse(raw);
                    version = json["tag_name"]!.Value<string>()!;
                }
                else
                {
                    raw = await _httpService.GetTextAsync(_nodeService.ResolveApiUrl("v2/resource/get_version"));
                    var json = JObject.Parse(raw);
                    version = json["version"]!.Value<string>()!;
                }
                Log.logger.Info($"汉化模组最后标签为： {version}");
                return int.Parse(version);
            }
            catch (Exception ex)
            {
                Log.logger.Error("获取模组标签失败。", ex);
                return -100;
            }
        }

        private async Task CacheHash()
        {
            string url = _config.Settings.general.internationalMode
                ? "https://cdn-api.zeroasso.top/v2/hash/get_hash"
                : "https://api.zeroasso.top/v2/hash/get_hash";
            string hash = await _httpService.GetTextAsync(url);
            _hashCacheObject = JObject.Parse(hash);
            if (_hashCacheObject == null)
            {
                Log.logger.Error("获取Hash失败。");
                _dialogService.ShowMessage("获取Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "获取Hash失败");
                await StopInstallAsync();
            }
        }

        private void WriteLCBLangConfig(string value)
        {
            try
            {
                string configPath = Path.Combine(_appState.LimbusCompanyDir, "LimbusCompany_Data", "Lang", "config.json");
                if (File.Exists(configPath))
                {
                    string configJson = File.ReadAllText(configPath);
                    var configObject = JObject.Parse(configJson);
                    configObject["lang"] = value;
                    File.WriteAllText(configPath, configObject.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.logger.Warn("修改LCB lang config失败: " + ex.Message);
            }
        }
    }
}
