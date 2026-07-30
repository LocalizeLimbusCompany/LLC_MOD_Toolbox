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
        private const float DownloadProgressCeiling = 99.8f;
        private const float FinalItemCompletionProgress = 99.99f;

        private sealed record InstallPlan(bool InstallFont, bool InstallMod, int LatestModVersion)
        {
            public int ItemCount => (InstallFont ? 1 : 0) + (InstallMod ? 1 : 0);
        }

        private sealed class InlineProgress<T>(Action<T> report) : IProgress<T>
        {
            public void Report(T value) => report(value);
        }

        private readonly AppState _appState;
        private readonly IHttpService _httpService;
        private readonly INodeService _nodeService;
        private readonly IMirrorChyanService _mirrorChyanService;
        private readonly IFileService _fileService;
        private readonly IDialogService _dialogService;
        private readonly ConfigurationManager _config;
        private JObject? _hashCacheObject;
        private volatile bool _isStopped;

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

        public async Task<InstallResult> InstallAsync(IProgress<InstallProgress> progress, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(progress);

            _isStopped = false;
            _hashCacheObject = null;
            progress.Report(new InstallProgress(0));

            try
            {
                ct.ThrowIfCancellationRequested();

                if (_appState.GreytestStatus)
                {
                    IProgress<float> greytestProgress = CreateDownloadProgress(progress, 0, 1);
                    if (!await InstallGreytestMod(greytestProgress, ct) || _isStopped)
                        return InstallResult.Aborted;

                    ReportItemCompleted(progress, 0, 1);
                    WriteLCBLangConfig("LLC_zh-CN");
                    ct.ThrowIfCancellationRequested();
                    if (_isStopped)
                        return InstallResult.Aborted;

                    progress.Report(new InstallProgress(100));
                    return InstallResult.Succeeded;
                }

                InstallPlan? plan = await CreateInstallPlanAsync(ct);
                if (plan == null || _isStopped)
                    return InstallResult.Aborted;

                if (RequiresHashCache(plan) && !await CacheHash(plan))
                    return InstallResult.Aborted;

                int itemIndex = 0;
                if (plan.InstallFont)
                {
                    IProgress<float> fontProgress = CreateDownloadProgress(progress, itemIndex, plan.ItemCount);
                    if (!await InstallFont(fontProgress, ct) || _isStopped)
                        return InstallResult.Aborted;

                    ReportItemCompleted(progress, itemIndex, plan.ItemCount);
                    itemIndex++;
                }

                if (plan.InstallMod)
                {
                    IProgress<float> modProgress = CreateDownloadProgress(progress, itemIndex, plan.ItemCount);
                    if (!await InstallMod(plan.LatestModVersion, modProgress, ct) || _isStopped)
                        return InstallResult.Aborted;

                    ReportItemCompleted(progress, itemIndex, plan.ItemCount);
                }

                WriteLCBLangConfig("LLC_zh-CN");
                ct.ThrowIfCancellationRequested();
                if (_isStopped)
                    return InstallResult.Aborted;

                progress.Report(new InstallProgress(100));
                return InstallResult.Succeeded;
            }
            finally
            {
                _hashCacheObject = null;
            }
        }

        public Task StopInstallAsync()
        {
            _isStopped = true;
            string dir = _appState.LimbusCompanyDir;
            _fileService.DeleteFile(Path.Combine(dir, "BepInEx-IL2CPP-x64.7z"));
            _fileService.DeleteFile(Path.Combine(dir, "tmpchinesefont_BIE.7z"));
            _fileService.DeleteFile(Path.Combine(dir, "LLCCN-Font.7z"));
            _fileService.DeleteFile(Path.Combine(dir, "LimbusLocalize.7z"));
            _fileService.DeleteFile(Path.Combine(dir, "LimbusLocalize_BIE.7z"));
            _fileService.DeleteFile(Path.Combine(dir, "LimbusLocalize_Dev.7z"));
            _hashCacheObject = null;
            return Task.CompletedTask;
        }

        private async Task<InstallPlan?> CreateInstallPlanAsync(CancellationToken ct)
        {
            string fontDir = Path.Combine(
                _appState.LimbusCompanyDir,
                "LimbusCompany_Data",
                "Lang",
                "LLC_zh-CN",
                "Font",
                "Context");
            string fontChinese = Path.Combine(fontDir, "ChineseFont.ttf");
            string fontBackup = Path.Combine(
                _appState.LimbusCompanyDir,
                "LimbusCompany_Data",
                "Lang",
                "LLC_zh-CN",
                "BackupFont",
                "ChineseFont.ttf.bak");
            bool installFont = !File.Exists(fontChinese) && !File.Exists(fontBackup);

            string versionJsonPath = Path.Combine(
                _appState.LimbusCompanyDir,
                "LimbusCompany_Data",
                "Lang",
                "LLC_zh-CN",
                "Info",
                "version.json");

            ct.ThrowIfCancellationRequested();
            int latestVersion = _appState.IsMirrorChyanMode
                ? await _mirrorChyanService.GetLatestModVersionAsync()
                : await GetLatestModVersion();
            if (latestVersion == -100)
            {
                await StopInstallAsync();
                return null;
            }

            bool installMod;
            if (!File.Exists(versionJsonPath))
            {
                Log.logger.Info("模组不存在。开始安装。");
                installMod = true;
            }
            else
            {
                var versionObj = JObject.Parse(File.ReadAllText(versionJsonPath));
                int currentVersion = versionObj["version"]!.Value<int>();
                Log.logger.Info("最后模组版本： " + latestVersion);
                Log.logger.Info("当前模组版本： " + currentVersion);
                installMod = currentVersion < latestVersion;
                Log.logger.Info(installMod ? "模组需要更新。进行安装。" : "模组无需更新。");
            }

            Log.logger.Info($"本次安装预检完成：字体={(installFont ? "需要下载" : "无需下载")}，模组={(installMod ? "需要下载" : "无需下载")}。");
            return new InstallPlan(installFont, installMod, latestVersion);
        }

        private async Task<bool> InstallFont(IProgress<float> downloadProgress, CancellationToken ct)
        {
            return await Task.Run(async () =>
            {
                Log.logger.Info("正在安装字体文件。");
                string fontDir = Path.Combine(_appState.LimbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN", "Font", "Context");
                Directory.CreateDirectory(fontDir);
                string fontZIPFile = Path.Combine(_appState.LimbusCompanyDir, "LLCCN-Font.7z");

                ct.ThrowIfCancellationRequested();
                if (_appState.IsMirrorChyanMode)
                {
                    var (url, sha256) = await _mirrorChyanService.GetFontInfoAsync();
                    if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(sha256))
                    {
                        await StopInstallAsync();
                        return false;
                    }

                    await _httpService.DownloadFileAsync(url, fontZIPFile, downloadProgress);
                    ct.ThrowIfCancellationRequested();
                    if (_isStopped)
                        return false;
                    if (!HashMatches(fontZIPFile, sha256))
                        return await AbortForHashFailure("字体哈希校验失败。");
                }
                else
                {
                    string downloadUrl = _nodeService.UseGithub
                        ? "https://raw.githubusercontent.com/LocalizeLimbusCompany/LocalizeLimbusCompany/refs/heads/main/Fonts/LLCCN-Font.7z"
                        : _nodeService.ResolveDownloadUrl("LLCCN-Font.7z");
                    await _httpService.DownloadFileAsync(downloadUrl, fontZIPFile, downloadProgress);
                    ct.ThrowIfCancellationRequested();
                    if (_isStopped)
                        return false;

                    string? expectedHash = _hashCacheObject?["font_hash"]?.Value<string>();
                    if (!HashMatches(fontZIPFile, expectedHash))
                        return await AbortForHashFailure("字体哈希校验失败。");
                }

                Log.logger.Info("解压字体包中。");
                _fileService.ExtractArchive(fontZIPFile, _appState.LimbusCompanyDir);
                ct.ThrowIfCancellationRequested();
                Log.logger.Info("删除字体包。");
                File.Delete(fontZIPFile);
                return true;
            }, ct);
        }

        private async Task<bool> InstallMod(int latestVersion, IProgress<float> downloadProgress, CancellationToken ct)
        {
            return await Task.Run(async () =>
            {
                Log.logger.Info("开始安装模组。");
                string limbusLocalizeZipPath = Path.Combine(_appState.LimbusCompanyDir, "LimbusLocalize.7z");

                ct.ThrowIfCancellationRequested();
                if (_appState.IsMirrorChyanMode)
                {
                    var (version, url, sha256) = await _mirrorChyanService.GetLatestModInfoAsync();
                    if (version == -100 || string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(sha256))
                    {
                        await StopInstallAsync();
                        return false;
                    }

                    await _httpService.DownloadFileAsync(url, limbusLocalizeZipPath, downloadProgress);
                    ct.ThrowIfCancellationRequested();
                    if (_isStopped)
                        return false;
                    if (!HashMatches(limbusLocalizeZipPath, sha256))
                        return await AbortForHashFailure("模组哈希校验失败。");
                }
                else if (_nodeService.UseGithub)
                {
                    await _httpService.DownloadFileAsync(
                        $"https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/{latestVersion}/LimbusLocalize_{latestVersion}.7z",
                        limbusLocalizeZipPath,
                        downloadProgress);
                    ct.ThrowIfCancellationRequested();
                    if (_isStopped)
                        return false;
                }
                else
                {
                    string downloadUrl = _nodeService.ResolveDownloadUrl($"LimbusLocalize_{latestVersion}.7z");
                    await _httpService.DownloadFileAsync(downloadUrl, limbusLocalizeZipPath, downloadProgress);
                    ct.ThrowIfCancellationRequested();
                    if (_isStopped)
                        return false;

                    string? expectedHash = _hashCacheObject?["main_hash"]?.Value<string>();
                    if (!HashMatches(limbusLocalizeZipPath, expectedHash))
                        return await AbortForHashFailure("模组哈希校验失败。");
                }

                Log.logger.Info("解压模组本体 zip 中。");
                _fileService.ExtractArchive(limbusLocalizeZipPath, _appState.LimbusCompanyDir);
                ct.ThrowIfCancellationRequested();
                Log.logger.Info("删除模组本体 zip 。");
                File.Delete(limbusLocalizeZipPath);
                return true;
            }, ct);
        }

        private async Task<bool> InstallGreytestMod(IProgress<float> downloadProgress, CancellationToken ct)
        {
            return await Task.Run(async () =>
            {
                Log.logger.Info("灰度测试模式已开启。开始安装灰度模组。");
                if (string.IsNullOrWhiteSpace(_appState.GreytestUrl))
                {
                    Log.logger.Error("灰度模组下载地址为空。");
                    await StopInstallAsync();
                    return false;
                }

                string zipPath = Path.Combine(_appState.LimbusCompanyDir, "LimbusLocalize_Dev.7z");
                await _httpService.DownloadFileAsync(_appState.GreytestUrl, zipPath, downloadProgress);
                ct.ThrowIfCancellationRequested();
                if (_isStopped)
                    return false;

                _fileService.ExtractArchive(zipPath, _appState.LimbusCompanyDir);
                ct.ThrowIfCancellationRequested();
                File.Delete(zipPath);
                Log.logger.Info("灰度模组安装完成。");
                return true;
            }, ct);
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

        private async Task<bool> CacheHash(InstallPlan plan)
        {
            try
            {
                string url = _config.Settings.general.internationalMode
                    ? "https://cdn-api.zeroasso.top/v2/hash/get_hash"
                    : "https://api.zeroasso.top/v2/hash/get_hash";
                string hash = await _httpService.GetTextAsync(url, reportError: false);
                if (string.IsNullOrWhiteSpace(hash))
                    throw new InvalidDataException("Hash 接口返回为空。");

                var hashObject = JObject.Parse(hash);
                if (plan.InstallFont && string.IsNullOrWhiteSpace(hashObject["font_hash"]?.Value<string>()))
                    throw new InvalidDataException("Hash 响应缺少 font_hash。");
                if (plan.InstallMod && !_nodeService.UseGithub && string.IsNullOrWhiteSpace(hashObject["main_hash"]?.Value<string>()))
                    throw new InvalidDataException("Hash 响应缺少 main_hash。");

                _hashCacheObject = hashObject;
                return true;
            }
            catch (Exception ex)
            {
                Log.logger.Error("获取Hash失败。", ex);
                await StopInstallAsync();
                _dialogService.ShowMessage("获取Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "获取Hash失败");
                return false;
            }
        }

        private bool RequiresHashCache(InstallPlan plan)
        {
            return !_appState.IsMirrorChyanMode &&
                (plan.InstallFont || (plan.InstallMod && !_nodeService.UseGithub));
        }

        private async Task<bool> AbortForHashFailure(string logMessage)
        {
            Log.logger.Error(logMessage);
            await StopInstallAsync();
            _dialogService.ShowMessage("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败");
            return false;
        }

        private bool HashMatches(string filePath, string? expectedHash)
        {
            if (string.IsNullOrWhiteSpace(expectedHash))
                return false;
            string actualHash = _fileService.CalculateSHA256(filePath);
            return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }

        private static IProgress<float> CreateDownloadProgress(
            IProgress<InstallProgress> overallProgress,
            int itemIndex,
            int itemCount)
        {
            float start = itemIndex * 100f / itemCount;
            float end = (itemIndex + 1) * 100f / itemCount;
            float highestPercentage = 0;
            object progressLock = new();

            return new InlineProgress<float>(percentage =>
            {
                float normalized = float.IsFinite(percentage)
                    ? Math.Clamp(percentage, 0, DownloadProgressCeiling)
                    : 0;

                lock (progressLock)
                {
                    if (normalized < highestPercentage)
                        return;

                    highestPercentage = normalized;
                    float overall = start + ((end - start) * normalized / 100f);
                    overallProgress.Report(new InstallProgress(overall));
                }
            });
        }

        private static void ReportItemCompleted(
            IProgress<InstallProgress> progress,
            int itemIndex,
            int itemCount)
        {
            float itemBoundary = (itemIndex + 1) * 100f / itemCount;
            progress.Report(new InstallProgress(itemBoundary >= 100 ? FinalItemCompletionProgress : itemBoundary));
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
                Log.logger.Error("修改LCB lang config失败。", ex);
                throw;
            }
        }
    }
}
