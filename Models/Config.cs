using System.IO;
using LLC_MOD_Toolbox.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace LLC_MOD_Toolbox.Models
{
    public class Config(PrimaryNodeList primaryNodeList)
    {
        public string? GamePath { get; set; } = PathHelper.DetectedLimbusCompanyPath;
        public string? Token { get; set; }
        public NodeInformation ApiNode { get; set; } =
            primaryNodeList.ApiNode.First(n => n.IsDefault);

        public NodeInformation DownloadNode { get; set; } =
            primaryNodeList.DownloadNode.First(n => n.IsDefault);

        public static Config Read(string path)
        {
            var jsonPayload = File.ReadAllText(path);
            return JsonHelper.Deserialize<Config>(jsonPayload);
        }

        public void Write(string path) => JsonHelper.Serialize(this, path);

        public static void WriteDefault(string path)
        {
            var defaultConfig = new Config(
                App.Current.Services.GetRequiredService<PrimaryNodeList>()
            )
            {
                GamePath = PathHelper.DetectedLimbusCompanyPath
            };
            defaultConfig.Write(path);
        }
    }
}
