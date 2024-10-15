using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LLC_MOD_Toolbox.Models;

public class RootModel
{
    private static readonly RootModel _instance = new();
    public static RootModel NodeList => _instance;


    public static readonly JsonSerializerSettings JsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Include,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy(),
        }
    };
    public List<ApiNodeInfo> DownloadNode { get; set; } = [];
    public List<ApiNodeInfo> ApiNode { get; set; } = [];

    private RootModel()
    {

        DownloadNode.Add(new ApiNodeInfo("默认", "https://node.zeroasso.top/d/od/"));
        ApiNode.Add(new ApiNodeInfo("默认", "https://api.kr.zeroasso.top/"));
    }

    public void AddNodes(RootModel rootModel)
    {
        DownloadNode.AddRange(rootModel.DownloadNode);
        ApiNode.AddRange(rootModel.ApiNode);
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
