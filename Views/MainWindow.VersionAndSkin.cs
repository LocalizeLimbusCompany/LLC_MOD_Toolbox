using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        #region 改主页版本号
        internal async Task<bool> ChangeHomePageVersion()
        {
            bool needUpdate = false;
            string nowVersionText = "";
            string latestVersionText = "";
            int latestVersion = 0;
            int nowVersion = 0;
            if (isMirrorChyanMode)
            {
                latestVersion = await GetLatestLimbusLocalizeVersionWithMirrorChyanWithoutReport();
            }
            else
            {
                latestVersion = await GetLatestLimbusLocalizeVersionWithoutReport();
            }
            if (latestVersion == -100)
            {
                latestVersionText = "最新版本：获取失败";
            }
            else
            {
                latestVersionText = $"最新版本：{latestVersion}";
            }

            string langDir = Path.Combine(limbusCompanyDir, "LimbusCompany_Data/Lang/LLC_zh-CN");
            string versionJsonPath = Path.Combine(langDir, "Info", "version.json");
            if (!File.Exists(versionJsonPath))
            {
                needUpdate = true;
                nowVersionText = "当前版本：未安装";
            }
            else
            {
                try
                {
                    JObject versionObj = JObject.Parse(File.ReadAllText(versionJsonPath));
                    nowVersion = versionObj["version"].Value<int>();
                    nowVersionText = $"当前版本：{nowVersion}";
                }
                catch (Exception ex)
                {
                    nowVersionText = "当前版本：解析失败";
                    Log.logger.Error("解析version.json出问题", ex);
                }
            }
            if (nowVersion < latestVersion && nowVersion != 0)
            {
                nowVersionText = nowVersionText + "（可更新）";
                needUpdate = true;
            }
            await Dispatcher.BeginInvoke(() =>
            {
                ViewModel.CurrentVersionText = nowVersionText;
                ViewModel.LatestVersionText = latestVersionText;
            });
            return needUpdate;
        }

        private async Task<int> GetLatestLimbusLocalizeVersionWithMirrorChyanWithoutReport()
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
            catch (Exception ex)
            {
                Log.logger.Error("获取模组标签失败。", ex);
                return -100;
            }
        }

        private async Task<int> GetLatestLimbusLocalizeVersionWithoutReport()
        {
            try
            {
                Log.logger.Info("获取模组标签。");
                string version;
                string raw = await GetURLText(string.Format(useAPIEndPoint, "v2/resource/get_version"));
                var json = JObject.Parse(raw);
                version = json["version"].Value<string>();
                Log.logger.Info($"汉化模组最后标签为： {version}");
                int parseVersion = int.Parse(version);
                return parseVersion;
            }
            catch (Exception ex)
            {
                Log.logger.Error("获取模组标签失败。", ex);
                return -100;
            }
        }

        private async Task CHangeFkingHomeVersion(string version)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                ViewModel.CurrentVersionText = $"当前版本：{version}";
            });
        }
        #endregion

        #region 皮肤系统
        private void LoadAndApplySkin()
        {
            try
            {
                if (configuation.Settings.skin == null)
                {
                    configuation.Settings.skin = new SkinConfig();
                    configuation.SaveConfig();
                }

                string skinName = configuation.Settings.skin.currentSkin ?? "default";
                Log.logger.Info($"正在加载皮肤: {skinName}");

                var skinManager = SkinManager.Instance;
                bool loaded = skinManager.LoadSkin(skinName);

                if (loaded)
                {
                    skinManager.ApplySkinToWindow(this);
                    Log.logger.Info($"皮肤加载成功: {skinManager.CurrentSkinName}");
                }
                else
                {
                    Log.logger.Warn("皮肤加载失败，使用默认外观");
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error("加载皮肤时出错", ex);
            }
        }

        public bool ChangeSkin(string skinName)
        {
            try
            {
                Log.logger.Info($"正在切换皮肤到: {skinName}");

                var skinManager = SkinManager.Instance;
                bool loaded = skinManager.LoadSkin(skinName);

                if (loaded)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        skinManager.ApplySkinToWindow(this);
                    });

                    configuation.Settings.skin.currentSkin = skinName;
                    configuation.SaveConfig();

                    Log.logger.Info($"皮肤切换成功: {skinManager.CurrentSkinName}");
                    return true;
                }
                else
                {
                    Log.logger.Warn($"皮肤切换失败: {skinName}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error("切换皮肤时出错", ex);
                return false;
            }
        }

        internal async Task<bool> InstallSkinFromServerAsync(string skinName)
        {
            if (string.IsNullOrWhiteSpace(skinName))
            {
                return false;
            }

            string archivePath = Path.Combine(currentDir, $"{skinName}.7z");
            string downloadUrl = $"https://api.zeroasso.top/v2/skin/get_skin/{Uri.EscapeDataString(skinName)}";

            try
            {
                Log.logger.Info($"开始下载皮肤: {skinName}");
                await DownloadFileAsyncWithoutProgress(downloadUrl, archivePath);

                Log.logger.Info($"开始解压皮肤: {skinName}");
                Unarchive(archivePath, currentDir);

                string installedSkinPath = Path.Combine(currentDir, "Skins", skinName, "skin.json");
                bool installed = File.Exists(installedSkinPath);
                Log.logger.Info(installed
                    ? $"皮肤安装完成: {skinName}"
                    : $"皮肤已解压但未找到预期配置: {installedSkinPath}");
                if (installed)
                {
                    UniversalDialog.ShowMessage("皮肤安装完成。");
                }
                else
                {
                    UniversalDialog.ShowMessage("皮肤安装失败。");
                }
                return installed;
            }
            catch (Exception ex)
            {
                Log.logger.Error($"安装皮肤失败: {skinName}", ex);
                return false;
            }
            finally
            {
                try
                {
                    if (File.Exists(archivePath))
                    {
                        File.Delete(archivePath);
                    }
                }
                catch (Exception ex)
                {
                    Log.logger.Warn($"清理皮肤安装包失败: {archivePath}, {ex.Message}");
                }
            }
        }

        public List<string> GetAvailableSkins()
        {
            return SkinManager.Instance.GetAvailableSkins();
        }

        public SkinDefinition GetSkinInfo(string skinName)
        {
            return SkinManager.Instance.GetSkinInfo(skinName);
        }
        #endregion
    }
}
