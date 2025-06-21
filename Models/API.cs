using System.IO;
using LLC_MOD_Toolbox.Helpers;

namespace LLC_MOD_Toolbox.Models;

/// <summary>
/// 包含下载与 API 节点的信息
/// </summary>
public class PrimaryNodeList
{
    /// <summary>
    /// 下载节点, 默认长度为 1
    /// </summary>
    public List<NodeInformation> DownloadNode { get; } =
        [new("默认", "https://node.zeroasso.top/d/od/{0}", true)];

    /// <summary>
    /// API 节点, 默认长度为 1
    /// </summary>
    public List<NodeInformation> ApiNode { get; } =
        [new("默认", "https://api.zeroasso.top/v2/get_api/get/{0}", true)];

    public static PrimaryNodeList ReadFrom(string url)
    {
        string jsonPayload = File.ReadAllText(url);
        return JsonHelper.DeserializePrimaryNodeList(jsonPayload);
    }
}

/// <summary>
/// 节点信息
/// </summary>
public record NodeInformation(
    string Name,
    string Endpoint,
    bool IsDefault = false,
    ApiType ApiType = ApiType.Custom
);

/// <summary>
/// API 类型
/// </summary>
public enum ApiType
{
    GitHub,
    Custom,
}
