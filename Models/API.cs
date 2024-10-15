using System.Runtime.InteropServices;

namespace LLC_MOD_Toolbox.Models;

public class PrimaryNodeList
{
    public static PrimaryNodeList NodeList { get; set; } = new();

    public List<ApiNodeInfo> DownloadNode { get; set; } = [];
    public List<ApiNodeInfo> ApiNode { get; set; } = [];

    private PrimaryNodeList()
    {
        DownloadNode.Add(new ApiNodeInfo("默认", "https://node.zeroasso.top/d/od/"));
        ApiNode.Add(new ApiNodeInfo("默认", "https://api.kr.zeroasso.top/"));
    }
}
/// <summary>
/// API节点信息
/// </summary>
public class ApiNodeInfo(string name, string endpoint, [Optional] bool isDefault)
{
    public string Name { get; } = name;
    public string Endpoint { get; } = endpoint;
    public bool? IsDefault { get; } = isDefault;
}
