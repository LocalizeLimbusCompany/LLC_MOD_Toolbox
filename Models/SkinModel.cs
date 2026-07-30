using Newtonsoft.Json.Linq;

namespace LLC_MOD_Toolbox.Models
{
    public class SkinDefinition
    {
        public int? schemaVersion { get; set; }
        public string name { get; set; } = "";
        public string displayName { get; set; } = "";
        public string desc { get; set; } = "";
        public string author { get; set; } = "";
        public string version { get; set; } = "1.0.0";
        public Dictionary<string, string> images { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, bool> visibility { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, string> margins { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, JObject> elements { get; set; } = new Dictionary<string, JObject>();
        public List<SkinDynamicImageDefinition> dynamicImages { get; set; } = new List<SkinDynamicImageDefinition>();
        public SkinMusicConfig? music { get; set; }
    }

    public class SkinDynamicImageDefinition
    {
        public string name { get; set; } = "";
        public string host { get; set; } = "";
        public string source { get; set; } = "";
        public JObject properties { get; set; } = new JObject();
    }

    public sealed class SkinApplyResult
    {
        public bool Success { get; init; }
        public string? ErrorPath { get; init; }
        public string? ErrorMessage { get; init; }
        public IReadOnlyList<string> Warnings { get; init; } = Array.Empty<string>();

        public static SkinApplyResult Succeeded(IEnumerable<string>? warnings = null) => new()
        {
            Success = true,
            Warnings = warnings?.ToArray() ?? Array.Empty<string>()
        };

        public static SkinApplyResult Failed(string errorPath, string errorMessage, IEnumerable<string>? warnings = null) => new()
        {
            Success = false,
            ErrorPath = errorPath,
            ErrorMessage = errorMessage,
            Warnings = warnings?.ToArray() ?? Array.Empty<string>()
        };

        public string GetDisplayMessage()
        {
            string location = string.IsNullOrWhiteSpace(ErrorPath) ? "$" : ErrorPath;
            return $"皮肤加载失败，已保留最后一次成功效果。\n\n位置：{location}\n原因：{ErrorMessage}";
        }
    }

    public sealed class SkinReloadedEventArgs : EventArgs
    {
        public SkinReloadedEventArgs(SkinApplyResult result, bool shouldNotifyUser)
        {
            Result = result;
            ShouldNotifyUser = shouldNotifyUser;
        }

        public SkinApplyResult Result { get; }
        public bool ShouldNotifyUser { get; }
    }

    public class SkinMusicConfig
    {
        public bool enableMusic { get; set; }
        public string musicPath { get; set; } = "";
    }

    public class SkinCatalogItem
    {
        public string name { get; set; } = "";
        public string displayName { get; set; } = "";
        public string desc { get; set; } = "";
        public string author { get; set; } = "";
        public string version { get; set; } = "1.0.0";
        public bool isInstalled { get; set; }

        public string DisplayText => isInstalled ? displayName : $"{displayName} [可安装]";
    }
}
