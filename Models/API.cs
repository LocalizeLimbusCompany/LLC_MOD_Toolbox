using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Models;

public class PrimaryNodeList
{
    public static PrimaryNodeList NodeInstance { get; } = new();
    public List<NodeInformation> DownloadNode { get; set; } = [];
    public List<NodeInformation> ApiNode { get; set; } = [];

    /// <summary>
    /// 我怀疑这么写很邪门
    /// </summary>
    private PrimaryNodeList()
    {
        DownloadNode.Add(new NodeInformation("默认 (GitHub)", "https://node.zeroasso.top/d/od/", true));
        ApiNode.Add(new NodeInformation("默认 (GitHub)", "https://api.kr.zeroasso.top/", true));
    }
    /// <summary>
    /// 反序列化时会调用这个构造函数，所以这个构造函数会自动添加到静态实例中
    /// </summary>
    /// <param name="downloadNode">反序列化生成的</param>
    /// <param name="apiNode">反序列化生成的</param>
    [JsonConstructor]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:删除未使用的私有成员", Justification = "<挂起>")]
    private PrimaryNodeList(List<NodeInformation> downloadNode, List<NodeInformation> apiNode)
    {
        NodeInstance.DownloadNode.AddRange(downloadNode);
        NodeInstance.ApiNode.AddRange(apiNode);
    }
}
/// <summary>
/// 节点信息
/// </summary>
public record class NodeInformation(string Name, string Endpoint, bool IsDefault = false);

