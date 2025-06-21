using System.IO;
using LLC_MOD_Toolbox.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models
{
    public class Config(PrimaryNodeList primaryNodeList)
    {
        public string? GamePath { get; set; }
        public string? Token { get; set; }
        public required NodeInformation ApiNode { get; set; }
        public required NodeInformation DownloadNode { get; set; }

        [JsonIgnore]
        public PrimaryNodeList PrimaryNodeList { get; set; } = primaryNodeList;

        public static Config ReadFrom(string path, IServiceProvider services)
        {
            services.GetRequiredService<ILogger<Config>>().LogInformation("从 {path} 获取设置", path);
            try
            {
                var jsonPayload = File.ReadAllText(path);
                var config = JsonHelper.Deserialize<Config>(jsonPayload);
                config.PrimaryNodeList = services.GetRequiredService<PrimaryNodeList>();
                return config;
            }
            catch (Exception ex)
            {
                services.GetRequiredService<ILogger<Config>>().LogError(ex, "读取配置文件失败，使用默认配置");
                var primaryNodeList = services.GetRequiredService<PrimaryNodeList>();
                return new Config(primaryNodeList)
                {
                    ApiNode = primaryNodeList.ApiNode.First(n => n.IsDefault),
                    DownloadNode = primaryNodeList.DownloadNode.First(n => n.IsDefault),
                };
            }
        }

        public void WriteTo(string path) => JsonHelper.Serialize(this, path);
    }
}
