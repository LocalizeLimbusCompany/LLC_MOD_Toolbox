using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Windows;
using LLC_MOD_Toolbox;
using System.IO;
using log4net;
using System.Windows.Threading;

namespace LLC_MOD_Toolbox.ViewModels
{
    class AutoInstall
    {
        private static List<ApiNodeInfo> nodeList = [];
        private static List<ApiNodeInfo> apiList = [];
        private static string defaultEndPoint = "https://node.zeroasso.top/d/od/";
        private static string defaultAPIEndPoint = "https://api.kr.zeroasso.top/";
        private static bool APPChangeAPIUI = false;
        private static string? useEndPoint;
        private static string? useAPIEndPoint;
        private static bool useGithub = false;
        private static bool useMirrorGithub = false;
        private readonly ILog logger;
        private Dispatcher Dispatcher = Application.Current.Dispatcher;

        public AutoInstall()
        {
            logger = LogManager.GetLogger(typeof(AutoInstall));
        }

        public static void InitNode()
        {
            var _jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            var json = JsonConvert.DeserializeObject<RootModel>(File.ReadAllText($"NodeList.json"), _jsonSettings);
            nodeList = json.DownloadNode;
            apiList = json.ApiNode;
            foreach (var Node in nodeList)
            {
                if (Node.IsDefault == true)
                {
                    defaultEndPoint = Node.Endpoint;
                }
            }
            foreach (var api in apiList)
            {
                if (api.IsDefault == true)
                {
                    defaultAPIEndPoint = api.Endpoint;
                    useAPIEndPoint = defaultAPIEndPoint;
                }
            }
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
        public async Task<string> GetNodeComboboxText()
        {
            string combotext = string.Empty;
            await this.Dispatcher.BeginInvoke(() =>
            {
            });
            return combotext;
        }
        public async Task<string> GetAPIComboboxText()
        {
            string combotext = string.Empty;
            await this.Dispatcher.BeginInvoke(() =>
            {
            });
            return combotext;
        }
        public async Task<string> SetAPIComboboxText(string text)
        {
            APPChangeAPIUI = true;
            string combotext = string.Empty;
            await this.Dispatcher.BeginInvoke(() =>
            {
            });
            return combotext;
        }
        private async void NodeComboboxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string nodeComboboxText = await GetNodeComboboxText();
            logger.Info("选择节点。");
            if (nodeComboboxText != string.Empty)
            {
                if (nodeComboboxText == "恢复默认")
                {
                    useEndPoint = string.Empty;
                    useMirrorGithub = false;
                    useGithub = false;
                    logger.Info("已恢复默认Endpoint。");
                }
                else if (nodeComboboxText == "Github直连")
                {
                    logger.Info("选择Github节点。");
                    System.Windows.MessageBox.Show("如果您没有使用代理软件（包括Watt Toolkit）\n请不要使用此节点。\nGithub由于不可抗力因素，对国内网络十分不友好。\n如果您是国外用户，才应该使用此选项。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    useEndPoint = string.Empty;
                    useGithub = true;
                    useMirrorGithub = false;
                }
                else if (nodeComboboxText == "Mirror Github")
                {
                    logger.Info("选择镜像Github节点。");
                    System.Windows.MessageBox.Show("Mirror Github服务由【mirror.ghproxy.com】提供。\n零协会不对其可能造成的任何问题（包括不可使用，安全性相关）负责。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    useMirrorGithub = true;
                    useGithub = false;
                }
                else
                {
                    useEndPoint = FindNodeEndpoint(nodeComboboxText);
                    useMirrorGithub = false;
                    useGithub = false;
                    logger.Info("当前Endpoint：" + useEndPoint);
                    System.Windows.MessageBox.Show("切换成功。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                logger.Info("NodeComboboxText 为 null。");
            }
        }
        private async void APIComboboxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!useGithub)
            {
                string apiComboboxText = await GetAPIComboboxText();
                logger.Info("选择API节点。");
                if (apiComboboxText != string.Empty)
                {
                    if (apiComboboxText == "恢复默认")
                    {
                        useAPIEndPoint = defaultAPIEndPoint;
                        logger.Info("已恢复默认API Endpoint。");
                    }
                    else
                    {
                        useAPIEndPoint = FindAPIEndpoint(apiComboboxText);
                        logger.Info("当前API Endpoint：" + useAPIEndPoint);
                        System.Windows.MessageBox.Show("切换成功。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    logger.Info("APIComboboxText 为 null。");
                }
            }
            else if (APPChangeAPIUI == false)
            {
                await SetAPIComboboxText("恢复默认");
                logger.Info("已开启Github。无法切换API。");
                System.Windows.MessageBox.Show("切换失败。\n无法在节点为Github直连的情况下切换API。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            APPChangeAPIUI = false;
        }
    }

}
