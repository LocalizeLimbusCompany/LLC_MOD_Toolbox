using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json.Linq;

namespace LLC_MOD_Toolbox.Services.Network
{
    public interface INodeService
    {
        IReadOnlyList<Node> DownloadNodes { get; }
        IReadOnlyList<Node> ApiNodes { get; }
        string CurrentEndpoint { get; }
        string CurrentApiEndpoint { get; }
        bool UseGithub { get; }
        void Initialize();
        void SelectDownloadNode(string? nodeName);
        void SelectApiNode(string? nodeName);
        string ResolveDownloadUrl(string fileName);
        string ResolveApiUrl(string path);
        void ReadConfigNode();
        List<string> GetNodeOptions();
        List<string> GetApiOptions();
    }
}
