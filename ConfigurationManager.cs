using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Reflection;

namespace LLC_MOD_Toolbox
{
    public sealed class ConfigurationManager
    {
        private readonly object _lock = new();
        private readonly string _configFilePath;

        public AppSettings Settings { get; private set; }

        public ConfigurationManager(string configFile)
        {
            _configFilePath = configFile;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_configFilePath))
                    {
                        // 修复参数错位
                        File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(new AppSettings(), Formatting.Indented));
                    }
                    var json = File.ReadAllText(_configFilePath);
                    var serializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new IgnoreCommentsResolver(),
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Populate,
                    };
                    Settings = JsonConvert.DeserializeObject<AppSettings>(json, serializerSettings);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("主配置文件未找到，使用默认配置。");
                    Settings = new AppSettings();
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException($"JSON解析错误: {ex.Message}", ex);
                }
            }
        }
        public void SaveConfig()
        {
            File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
        }
        public T GetConfigSection<T>(Func<AppSettings, T> selector)
        {
            lock (_lock)
            {
                return selector(Settings);
            }
        }
    }
    public class IgnoreCommentsResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = _ => !property.PropertyName.StartsWith("_");
            return property;
        }
    }
}
