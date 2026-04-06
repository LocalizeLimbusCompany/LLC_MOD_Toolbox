using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        private async Task InstallMod()
        {
            if (isMirrorChyanMode)
            {
                await InstallModWithMirrorChyan();
            }
            else
            {
                await InstallModWithoutMirrorChyan();
            }
        }

        private async Task InstallModWithMirrorChyan()
        {
            await Task.Run(async () =>
            {
                Log.logger.Info("开始安装模组，Mirror酱，合体！");
                installPhase = 2;
                string langDir = Path.Combine(limbusCompanyDir, "LimbusCompany_Data/Lang/LLC_zh-CN");
                string versionJsonPath = Path.Combine(langDir, "Info", "version.json");
                string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize.7z");
                int latestVersion = -1;
                int currentVersion = -1;
                bool needInstall = false;
                JObject versionObj;
                if (!File.Exists(versionJsonPath))
                {
                    Log.logger.Info("模组不存在。开始安装。");
                    needInstall = true;
                    isNewestModVersion = false;
                }
                if (!needInstall)
                {
                    latestVersion = await GetLatestLimbusLocalizeVersionWithMirrorChyan();
                    if (latestVersion == -100)
                    {
                        await StopInstall();
                        return;
                    }
                    Log.logger.Info("最后模组版本： " + latestVersion);
                    versionObj = JObject.Parse(File.ReadAllText(versionJsonPath));
                    currentVersion = versionObj["version"].Value<int>();
                    Log.logger.Info("当前模组版本： " + currentVersion);
                    if (currentVersion >= latestVersion)
                    {
                        Log.logger.Info("模组无需更新。");
                        return;
                    }
                    else
                    {
                        needInstall = true;
                        isNewestModVersion = false;
                        Log.logger.Info("模组需要更新。进行安装。");
                    }
                }
                if (needInstall)
                {
                    string url = "";
                    string sha256 = "";
                    (latestVersion, url, sha256) = await GetLatestLimbusLocalizeInfoWithMirrorChyan();
                    await DownloadFileAsync(url, limbusLocalizeZipPath);
                    if (sha256 != CalculateSHA256(limbusLocalizeZipPath))
                    {
                        Log.logger.Error("校验Hash失败。");
                        UniversalDialog.ShowMessage("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败", null, this);
                        await StopInstall();
                        return;
                    }
                    else
                    {
                        Log.logger.Info("校验Hash成功。");
                    }
                    Log.logger.Info("解压模组本体 zip 中。");
                    Unarchive(limbusLocalizeZipPath, limbusCompanyDir);
                    Log.logger.Info("删除模组本体 zip 。");
                    File.Delete(limbusLocalizeZipPath);
                    await CHangeFkingHomeVersion(latestVersion.ToString());
                }
            });
        }

        private async Task<int> GetLatestLimbusLocalizeVersionWithMirrorChyan()
        {
            try
            {
                Log.logger.Info("获取模组标签。");
                string version;
                string raw = await GetURLText("https://mirrorchyan.com/api/resources/LLC/latest?user_agent=LLC_MOD_Toolbox&current_version=&cdk=");
                var json = ParseMirrorChyanJson(raw);
                version = json["data"]["version_name"].Value<string>();
                Log.logger.Info($"汉化模组最后标签为： {version}");
                int parseVersion = int.Parse(version);
                return parseVersion;
            }
            catch (MirrorChyanException ex)
            {
                ErrorReportMirrorChyan(ex, false);
                return -100;
            }
            catch (Exception ex)
            {
                ErrorReport(ex, false);
                return -100;
            }
        }

        private async Task<(int, string, string)> GetLatestLimbusLocalizeInfoWithMirrorChyan()
        {
            try
            {
                Log.logger.Info("获取模组标签。");
                string version;
                string raw = await GetURLText($"https://mirrorchyan.com/api/resources/LLC/latest?user_agent=LLC_MOD_Toolbox&current_version=&cdk={mirrorChyanToken}", parseErrorJson: true);
                var json = ParseMirrorChyanJson(raw);
                version = json["data"]["version_name"].Value<string>();
                Log.logger.Info($"汉化模组最后标签为： {version}");
                int parseVersion = int.Parse(version);
                string url = json["data"]["url"].Value<string>();
                string sha256 = json["data"]["sha256"].Value<string>();
                return (parseVersion, url, sha256);
            }
            catch (MirrorChyanException ex)
            {
                ErrorReportMirrorChyan(ex, false);
                return (-100, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                ErrorReport(ex, false);
                return (-100, string.Empty, string.Empty);
            }
        }

        private async Task InstallModWithoutMirrorChyan()
        {
            await Task.Run(async () =>
            {
                Log.logger.Info("开始安装模组。");
                installPhase = 2;
                string langDir = Path.Combine(limbusCompanyDir, "LimbusCompany_Data/Lang/LLC_zh-CN");
                string versionJsonPath = Path.Combine(langDir, "Info", "version.json");
                string limbusLocalizeZipPath = Path.Combine(limbusCompanyDir, "LimbusLocalize.7z");
                int latestVersion = -1;
                int currentVersion = -1;
                bool needInstall = false;
                JObject versionObj;
                if (!File.Exists(versionJsonPath))
                {
                    Log.logger.Info("模组不存在。开始安装。");
                    needInstall = true;
                    isNewestModVersion = false;
                }
                if (useGithub && !needInstall)
                {
                    latestVersion = await GetLatestLimbusLocalizeVersion(true);
                    Log.logger.Info("最后模组版本： " + latestVersion);
                    versionObj = JObject.Parse(File.ReadAllText(versionJsonPath));
                    currentVersion = versionObj["version"].Value<int>();
                    Log.logger.Info("当前模组版本： " + currentVersion);
                    if (currentVersion >= latestVersion)
                    {
                        Log.logger.Info("模组无需更新。");
                        return;
                    }
                    else
                    {
                        needInstall = true;
                        isNewestModVersion = false;
                        Log.logger.Info("模组需要更新。进行安装。");
                    }
                }
                else if (!useGithub && !needInstall)
                {
                    latestVersion = await GetLatestLimbusLocalizeVersion(false);
                    Log.logger.Info("最后模组版本： " + latestVersion);
                    versionObj = JObject.Parse(File.ReadAllText(versionJsonPath));
                    currentVersion = versionObj["version"].Value<int>();
                    Log.logger.Info("当前模组版本： " + currentVersion);
                    if (currentVersion >= latestVersion)
                    {
                        Log.logger.Info("模组无需更新。");
                        return;
                    }
                    else
                    {
                        needInstall = true;
                        isNewestModVersion = false;
                        Log.logger.Info("模组需要更新。进行安装。");
                    }
                }
                if (useGithub && needInstall)
                {
                    latestVersion = await GetLatestLimbusLocalizeVersion(true);
                    await DownloadFileAsync($"https://github.com/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/download/{latestVersion}/LimbusLocalize_{latestVersion}.7z", limbusLocalizeZipPath);
                    Log.logger.Info("解压模组本体 zip 中。");
                    Unarchive(limbusLocalizeZipPath, limbusCompanyDir);
                    Log.logger.Info("删除模组本体 zip 。");
                    File.Delete(limbusLocalizeZipPath);
                    await CHangeFkingHomeVersion(latestVersion.ToString());
                }
                else if (!useGithub && needInstall)
                {
                    latestVersion = await GetLatestLimbusLocalizeVersion(false);
                    await DownloadFileAutoAsync($"LimbusLocalize_{latestVersion}.7z", limbusLocalizeZipPath);
                    if (hashCacheObject["main_hash"].Value<string>() != CalculateSHA256(limbusLocalizeZipPath))
                    {
                        Log.logger.Error("校验Hash失败。");
                        UniversalDialog.ShowMessage("校验Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "校验失败", null, this);
                        await StopInstall();
                        return;
                    }
                    else
                    {
                        Log.logger.Info("校验Hash成功。");
                    }
                    Log.logger.Info("解压模组本体 zip 中。");
                    Unarchive(limbusLocalizeZipPath, limbusCompanyDir);
                    Log.logger.Info("删除模组本体 zip 。");
                    File.Delete(limbusLocalizeZipPath);
                    await CHangeFkingHomeVersion(latestVersion.ToString());
                }
            });
        }

        private async Task CachedHash()
        {
            string hash = "";
            if (!configuation.Settings.general.internationalMode)
            {
                hash = await GetURLText("https://api.zeroasso.top/v2/hash/get_hash");
            }
            else
            {
                hash = await GetURLText("https://cdn-api.zeroasso.top/v2/hash/get_hash");
            }
            hashCacheObject = JObject.Parse(hash);
            if (hashCacheObject == null)
            {
                Log.logger.Error("获取Hash失败。");
                UniversalDialog.ShowMessage("获取Hash失败。\n请等待数分钟或更换节点。\n如果问题仍然出现，请进行反馈。", "获取Hash失败", null, this);
                await StopInstall();
                return;
            }
        }
    }
}
