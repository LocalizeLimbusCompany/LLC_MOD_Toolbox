using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace LLC_MOD_Toolbox.Services.Network
{
    public sealed class NodeService : INodeService
    {
        private readonly AppState _appState;
        private readonly ConfigurationManager _config;
        private List<Node> _downloadNodes = [];
        private List<Node> _apiNodes = [];
        private string _defaultEndpoint = "";
        private string _defaultApiEndpoint = "";
        private string _currentEndpoint = "";
        private string _currentApiEndpoint = "";
        private bool _useGithub;

        public NodeService(AppState appState, ConfigurationManager config)
        {
            _appState = appState;
            _config = config;
        }

        public IReadOnlyList<Node> DownloadNodes => _downloadNodes;
        public IReadOnlyList<Node> ApiNodes => _apiNodes;
        public string CurrentEndpoint => string.IsNullOrEmpty(_currentEndpoint) ? _defaultEndpoint : _currentEndpoint;
        public string CurrentApiEndpoint => string.IsNullOrEmpty(_currentApiEndpoint) ? _defaultApiEndpoint : _currentApiEndpoint;
        public bool UseGithub => _useGithub;

        public void Initialize()
        {
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            var json = JsonConvert.DeserializeObject<RootModel>(File.ReadAllText("NodeList.json"), jsonSettings);
            _downloadNodes = json!.DownloadNode;
            _apiNodes = json.ApiNode;

            bool international = _config.Settings.general.internationalMode;

            foreach (var node in _downloadNodes)
            {
                if (node.IsDefault && !international)
                    _defaultEndpoint = node.Endpoint;
                if (node.Endpoint == "https://cdn-download.zeroasso.top/files/{0}" && international)
                {
                    Log.logger.Info("获取到国际下载节点。");
                    _defaultEndpoint = node.Endpoint;
                }
            }

            foreach (var api in _apiNodes)
            {
                if (api.IsDefault && !international)
                {
                    _defaultApiEndpoint = api.Endpoint;
                    _currentApiEndpoint = _defaultApiEndpoint;
                }
                if (api.Endpoint == "https://cdn-api.zeroasso.top/{0}" && international)
                {
                    Log.logger.Info("获取到国际API节点。");
                    _defaultApiEndpoint = api.Endpoint;
                    _currentApiEndpoint = _defaultApiEndpoint;
                }
            }

            Log.logger.Info("API数量：" + _apiNodes.Count);
            Log.logger.Info("节点数量：" + _downloadNodes.Count);
        }

        public List<string> GetNodeOptions()
        {
            List<string> options = ["恢复默认"];
            foreach (var node in _downloadNodes)
                options.Add(node.Name);
            options.Add("Github直连");
            return options;
        }

        public List<string> GetApiOptions()
        {
            List<string> options = ["恢复默认"];
            foreach (var api in _apiNodes)
                options.Add(api.Name);
            return options;
        }

        public void SelectDownloadNode(string? nodeName)
        {
            if (string.IsNullOrEmpty(nodeName) || nodeName == "恢复默认")
            {
                _currentEndpoint = string.Empty;
                _useGithub = false;
                SaveDownloadNodeConfig("");
                Log.logger.Info("已恢复默认Endpoint。");
            }
            else if (nodeName == "Github直连")
            {
                _currentEndpoint = string.Empty;
                _useGithub = true;
                SaveDownloadNodeConfig("Github直连");
                Log.logger.Info("选择Github节点。");
            }
            else
            {
                _currentEndpoint = FindNodeEndpoint(nodeName);
                _useGithub = false;
                SaveDownloadNodeConfig(nodeName);
                Log.logger.Info("当前Endpoint：" + _currentEndpoint);
            }
        }

        public void SelectApiNode(string? nodeName)
        {
            if (string.IsNullOrEmpty(nodeName) || nodeName == "恢复默认")
            {
                _currentApiEndpoint = _defaultApiEndpoint;
                SaveApiNodeConfig("");
                Log.logger.Info("已恢复默认API Endpoint。");
            }
            else
            {
                _currentApiEndpoint = FindApiEndpoint(nodeName);
                SaveApiNodeConfig(nodeName);
                Log.logger.Info("当前API Endpoint：" + _currentApiEndpoint);
            }
        }

        public string ResolveDownloadUrl(string fileName)
        {
            if (_useGithub)
                return fileName;
            string endpoint = string.IsNullOrEmpty(_currentEndpoint) ? _defaultEndpoint : _currentEndpoint;
            return string.Format(endpoint, fileName);
        }

        public string ResolveApiUrl(string path)
        {
            string endpoint = string.IsNullOrEmpty(_currentApiEndpoint) ? _defaultApiEndpoint : _currentApiEndpoint;
            return string.Format(endpoint, path);
        }

        public void ReadConfigNode()
        {
            string defaultNode = _config.Settings.nodeSelect.defaultNode;
            string defaultApiNode = _config.Settings.nodeSelect.defaultApiNode;
            if (!string.IsNullOrEmpty(defaultNode))
            {
                if (defaultNode == "Github直连")
                {
                    Log.logger.Info("从配置使用Github节点。");
                    _useGithub = true;
                    _currentEndpoint = string.Empty;
                }
                else
                {
                    Log.logger.Info("从配置使用节点：" + defaultNode);
                    _currentEndpoint = FindNodeEndpoint(defaultNode);
                    _useGithub = false;
                }
            }
            if (!string.IsNullOrEmpty(defaultApiNode))
            {
                Log.logger.Info("从配置使用API节点：" + defaultApiNode);
                _currentApiEndpoint = FindApiEndpoint(defaultApiNode);
            }
        }

        private string FindNodeEndpoint(string name)
        {
            foreach (var node in _downloadNodes)
                if (node.Name == name)
                    return node.Endpoint;
            return string.Empty;
        }

        private string FindApiEndpoint(string name)
        {
            foreach (var api in _apiNodes)
                if (api.Name == name)
                    return api.Endpoint;
            return string.Empty;
        }

        private void SaveDownloadNodeConfig(string node)
        {
            Log.logger.Info("设置下载节点:" + node);
            _config.Settings.nodeSelect.defaultNode = node;
            _config.SaveConfig();
        }

        private void SaveApiNodeConfig(string api)
        {
            Log.logger.Info("设置API节点:" + api);
            _config.Settings.nodeSelect.defaultApiNode = api;
            _config.SaveConfig();
        }
    }
}
