using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models;

public class PrimaryNodeList
{
    public static PrimaryNodeList NodeInstance { get; } = new();
    public List<NodeInformation> DownloadNode { get; set; } = [];
    public List<NodeInformation> ApiNode { get; set; } = [];

    private PrimaryNodeList()
    {
        DownloadNode.Add(new NodeInformation("默认", "https://node.zeroasso.top/d/od/"));
        ApiNode.Add(new NodeInformation("默认", "https://api.kr.zeroasso.top/"));
    }
    /// <summary>
    /// 用于反序列化的构造函数，会自添加到静态实例中
    /// </summary>
    /// <param name="downloadNode">反序列化生成的</param>
    /// <param name="apiNode">反序列化生成的</param>
    [JsonConstructor]
    private PrimaryNodeList(List<NodeInformation> downloadNode, List<NodeInformation> apiNode)
    {
        NodeInstance.DownloadNode.AddRange(downloadNode);
        NodeInstance.ApiNode.AddRange(apiNode);
    }
}
/// <summary>
/// 节点信息
/// </summary>
public class NodeInformation(string name, string endpoint, bool isDefault=false)
{
    public string Name { get; init; } = name;
    public string Endpoint { get; init; } = endpoint;
    public bool? IsDefault { get; init; } = isDefault;
}
