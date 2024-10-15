using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LLC_MOD_Toolbox.Models;

public class RootModel
{
    public static RootModel NodeList => new();

    public static readonly JsonSerializerSettings _jsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Include,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy(),
        }
    };
    public List<ApiNodeInfo> DownloadNode { get; set; } = new List<ApiNodeInfo>();
    public List<ApiNodeInfo> ApiNode { get; set; } = new List<ApiNodeInfo>();

    private RootModel()
    {
        NodeList.DownloadNode.Add(new ApiNodeInfo("默认", "https://node.zeroasso.top/d/od/"));
        NodeList.ApiNode.Add(new ApiNodeInfo("默认", "https://api.kr.zeroasso.top/"));
    }

    public RootModel(RootModel rootModel)
    {
        NodeList.DownloadNode.AddRange(rootModel.DownloadNode);
        NodeList.ApiNode.AddRange(rootModel.ApiNode);
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
