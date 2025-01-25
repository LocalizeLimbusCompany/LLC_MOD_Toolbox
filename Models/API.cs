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
    public List<NodeInformation> DownloadNode { get; init; } =
        [new("默认", (new("https://node.zeroasso.top/d/od/")), true)];

    /// <summary>
    /// API 节点, 默认长度为 1
    /// </summary>
    public List<NodeInformation> ApiNode { get; init; } =
        [new("默认", new("https://api.kr.zeroasso.top/"), true)];

    public static PrimaryNodeList Create(string url)
    {
        var jsonPayload = File.ReadAllText(url);
        return JsonHelper.DeserializePrimaryNodeList(jsonPayload);
    }
}

/// <summary>
/// 节点信息
/// </summary>
public record class NodeInformation(string Name, Uri Endpoint, bool IsDefault = false);
