using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Windows;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        #region 读取节点
        private static bool APPChangeAPIUI = false;

        public void InitNode()
        {
            var _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            var json = JsonConvert.DeserializeObject<RootModel>(File.ReadAllText($"NodeList.json"), _jsonSettings);
            nodeList = json.DownloadNode;
            apiList = json.ApiNode;
            List<string> nodeOptions = ["恢复默认"];
            foreach (var Node in nodeList)
            {
                if (Node.IsDefault == true && !configuation.Settings.general.internationalMode)
                {
                    defaultEndPoint = Node.Endpoint;
                }
                if (Node.Endpoint == "https://cdn-download.zeroasso.top/files/{0}" && configuation.Settings.general.internationalMode)
                {
                    Log.logger.Info("获取到国际下载节点。");
                    defaultEndPoint = Node.Endpoint;
                }
                nodeOptions.Add(Node.Name);
            }
            nodeOptions.Add("Github直连");

            List<string> apiOptions = ["恢复默认"];
            foreach (var api in apiList)
            {
                if (api.IsDefault == true && !configuation.Settings.general.internationalMode)
                {
                    defaultAPIEndPoint = api.Endpoint;
                    useAPIEndPoint = defaultAPIEndPoint;
                }
                if (api.Endpoint == "https://cdn-api.zeroasso.top/{0}" && configuation.Settings.general.internationalMode)
                {
                    Log.logger.Info("获取到国际API节点。");
                    defaultAPIEndPoint = api.Endpoint;
                    useAPIEndPoint = defaultAPIEndPoint;
                }
                apiOptions.Add(api.Name);
            }
            ViewModel.SetNodeOptions(nodeOptions);
            ViewModel.SetApiOptions(apiOptions);
            Log.logger.Info("API数量：" + apiList.Count);
            Log.logger.Info("节点数量：" + nodeList.Count);
        }

        private static string FindNodeEndpoint(string Name)
        {
            foreach (var node in nodeList)
            {
                if (node.Name == Name)
                {
                    return node.Endpoint;
                }
            }
            return string.Empty;
        }

        private static string FindAPIEndpoint(string Name)
        {
            foreach (var api in apiList)
            {
                if (api.Name == Name)
                {
                    return api.Endpoint;
                }
            }
            return string.Empty;
        }

        private async Task SetApiSelectionAsync(string text)
        {
            APPChangeAPIUI = true;
            await this.Dispatcher.BeginInvoke(() =>
            {
                ViewModel.UpdateSelectedApiOption(text);
            });
        }

        private async void HandleNodeSelectionChanged(string? nodeComboboxText)
        {
            if (isMirrorChyanMode) return;
            Log.logger.Info("选择节点。");
            if (!string.IsNullOrEmpty(nodeComboboxText))
            {
                if (nodeComboboxText == "恢复默认")
                {
                    useEndPoint = string.Empty;
                    useGithub = false;
                    SetDownloadNodeConfig("");
                    Log.logger.Info("已恢复默认Endpoint。");
                }
                else if (nodeComboboxText == "Github直连")
                {
                    Log.logger.Info("选择Github节点。");
                    if (!isLaunching)
                    {
                        UniversalDialog.ShowMessage("如果您没有使用代理软件（包括Watt Toolkit）\n请不要使用此节点。\nGithub由于不可抗力因素，对国内网络十分不友好。\n如果您是国外用户，才应该使用此选项。", "警告", null, this);
                    }
                    SetDownloadNodeConfig("Github直连");
                    useEndPoint = string.Empty;
                    useGithub = true;
                }
                else
                {
                    useEndPoint = FindNodeEndpoint(nodeComboboxText);
                    useGithub = false;
                    SetDownloadNodeConfig(nodeComboboxText);
                    Log.logger.Info("当前Endpoint：" + useEndPoint);
                    UniversalDialog.ShowMessage("切换成功。", "提示", null, this);
                }
            }
            else
            {
                Log.logger.Info("NodeComboboxText 为 null。");
            }
        }

        private async void HandleApiSelectionChanged(string? apiComboboxText)
        {
            if (isMirrorChyanMode) return;
            if (!useGithub)
            {
                Log.logger.Info("选择API节点。");
                if (!string.IsNullOrEmpty(apiComboboxText))
                {
                    if (apiComboboxText == "恢复默认")
                    {
                        useAPIEndPoint = defaultAPIEndPoint;
                        SetApiNodeConfig("");
                        Log.logger.Info("已恢复默认API Endpoint。");
                    }
                    else
                    {
                        useAPIEndPoint = FindAPIEndpoint(apiComboboxText);
                        SetApiNodeConfig(apiComboboxText);
                        Log.logger.Info("当前API Endpoint：" + useAPIEndPoint);
                        UniversalDialog.ShowMessage("切换成功。", "提示", null, this);
                    }
                }
                else
                {
                    Log.logger.Info("APIComboboxText 为 null。");
                }
            }
            else if (APPChangeAPIUI == false)
            {
                await SetApiSelectionAsync("恢复默认");
                Log.logger.Info("已开启Github。无法切换API。");
                UniversalDialog.ShowMessage("切换失败。\n无法在节点为Github直连的情况下切换API。", "提示", null, this);
            }
            APPChangeAPIUI = false;
        }

        internal void SetDownloadNodeConfig(string node)
        {
            Log.logger.Info("设置下载节点:" + node);
            configuation.Settings.nodeSelect.defaultNode = node;
            configuation.SaveConfig();
        }

        private void InitializeSkinComboBox()
        {
            try
            {
                Log.logger.Info("初始化皮肤选择框。");
                var availableSkins = SkinManager.Instance.GetAvailableSkins();
                List<string> skinOptions = [];

                foreach (var skinName in availableSkins)
                {
                    var skinInfo = SkinManager.Instance.GetSkinInfo(skinName);
                    if (skinInfo != null)
                    {
                        skinOptions.Add(skinInfo.displayName);
                    }
                }
                ViewModel.SetSkinOptions(skinOptions);

                var currentSkin = SkinManager.Instance.CurrentSkinInfo;
                if (currentSkin != null)
                {
                    ViewModel.UpdateSelectedSkinOption(currentSkin.displayName);
                    UpdateSkinDescription(currentSkin);
                }

                Log.logger.Info($"皮肤选择框初始化完成，共 {availableSkins.Count} 个皮肤。");
            }
            catch (Exception ex)
            {
                Log.logger.Error($"初始化皮肤选择框失败: {ex.Message}");
            }
        }

        private void HandleSkinSelectionChanged(string? selectedDisplayName)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedDisplayName)) return;
                Log.logger.Info($"选择皮肤: {selectedDisplayName}");

                var availableSkins = SkinManager.Instance.GetAvailableSkins();
                string selectedSkinName = null;

                foreach (var skinName in availableSkins)
                {
                    var skinInfo = SkinManager.Instance.GetSkinInfo(skinName);
                    if (skinInfo != null && skinInfo.displayName == selectedDisplayName)
                    {
                        selectedSkinName = skinName;
                        break;
                    }
                }

                if (selectedSkinName != null)
                {
                    bool success = SkinManager.Instance.LoadSkin(selectedSkinName);
                    if (success)
                    {
                        SkinManager.Instance.ApplySkinToWindow(this);
                        var skinInfo = SkinManager.Instance.GetSkinInfo(selectedSkinName);
                        UpdateSkinDescription(skinInfo);
                        configuation.Settings.skin.currentSkin = selectedSkinName;
                        configuation.SaveConfig();
                        Log.logger.Info($"皮肤 {selectedDisplayName} 应用成功。");
                    }
                    else
                    {
                        Log.logger.Error($"皮肤 {selectedDisplayName} 应用失败。");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.logger.Error($"切换皮肤时出错: {ex.Message}");
            }
        }

        private void UpdateSkinDescription(SkinDefinition skinInfo)
        {
            if (skinInfo == null) return;

            try
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    string description = skinInfo.desc ?? "暂无描述。";
                    ViewModel.SkinDescription = description.Replace("\\n", "\n");
                });
            }
            catch (Exception ex)
            {
                Log.logger.Error($"更新皮肤描述时出错: {ex.Message}");
            }
        }

        internal void SetApiNodeConfig(string api)
        {
            Log.logger.Info("设置API节点:" + api);
            configuation.Settings.nodeSelect.defaultApiNode = api;
            configuation.SaveConfig();
        }

        internal void ReadConfigNode()
        {
            string defaultNode = configuation.Settings.nodeSelect.defaultNode;
            string defaultApiNode = configuation.Settings.nodeSelect.defaultApiNode;
            if (defaultNode != "")
            {
                if (defaultNode == "Github直连")
                {
                    Log.logger.Info("从配置使用Github节点。");
                    useGithub = true;
                    useEndPoint = string.Empty;
                    ViewModel.UpdateSelectedNodeOption("Github直连");
                    return;
                }
                Log.logger.Info("从配置使用节点：" + defaultNode);
                useEndPoint = FindNodeEndpoint(defaultNode);
                useGithub = false;
                ViewModel.UpdateSelectedNodeOption(defaultNode);
            }
            else
            {
                ViewModel.UpdateSelectedNodeOption("恢复默认");
            }
            if (defaultApiNode != "")
            {
                Log.logger.Info("从配置使用API节点：" + defaultApiNode);
                useAPIEndPoint = FindAPIEndpoint(defaultApiNode);
                ViewModel.UpdateSelectedApiOption(defaultApiNode);
            }
            else
            {
                ViewModel.UpdateSelectedApiOption("恢复默认");
            }
        }

        private void HandleNodeHelpRequested()
        {
            OpenUrl("https://www.zeroasso.top/docs/configuration/nodes");
        }
        #endregion
    }
}
