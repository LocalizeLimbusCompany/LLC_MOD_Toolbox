using System.IO;
using LLC_MOD_Toolbox.Helpers;

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

        public static Config Create(string url)
        {
            string jsonPayload = File.ReadAllText(url);
            return JsonHelper.Deserialize<Config>(jsonPayload);
        }
    }
}
