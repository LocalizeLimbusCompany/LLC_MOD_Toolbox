namespace LLC_MOD_Toolbox.Models;
/// <summary>
/// 包含下载与 API 节点的信息
/// </summary>
public class PrimaryNodeList
{
    public List<NodeInformation> DownloadNode { get; set; } = 
        [new NodeInformation("默认 (GitHub)", "https://node.zeroasso.top/d/od/", true)];
    public List<NodeInformation> ApiNode { get; set; } = 
        [new NodeInformation("默认 (GitHub)", "https://api.kr.zeroasso.top/", true)];
}
/// <summary>
/// 节点信息
/// </summary>
public record class NodeInformation(string Name, string Endpoint, bool IsDefault = false);

