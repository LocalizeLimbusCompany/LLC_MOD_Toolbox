using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LLC_MOD_Toolbox.Helpers;

static class JsonHelper
{
    /// <summary>
    /// JSON 序列化设置，使用驼峰命名法，忽略空值
    /// </summary>
    private static readonly JsonSerializerSettings camelCaseJsonSettings =
        new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            }
        };

    /// <summary>
    /// 反序列化节点列表
    /// </summary>
    /// <param name="jsonPayload">接受的Json文本</param>
    /// <returns>序列化的 <seealso cref="PrimaryNodeList"/></returns>
    public static PrimaryNodeList DeserializePrimaryNodeList(string jsonPayload) =>
        JsonConvert.DeserializeObject<PrimaryNodeList>(jsonPayload, camelCaseJsonSettings)
        ?? new PrimaryNodeList();

    /// <summary>
    /// 反序列化标签名
    /// </summary>
    /// <param name="jsonPayload"></param>
    /// <returns>版本号</returns>
    /// <exception cref="JsonReaderException"></exception>
    public static string DeserializeTagName(string jsonPayload) =>
        JObject.Parse(jsonPayload).GetValue("tag_name")?.ToString()[1..]
        ?? throw new JsonReaderException();

    /// <summary>
    /// 反序列化哈希值
    /// </summary>
    /// <param name="jsonPayload"></param>
    /// <returns>哈希值</returns>
    /// <exception cref="JsonReaderException"></exception>
    public static string DeserializeHash(string jsonPayload) =>
        JObject.Parse(jsonPayload).GetValue("hash")?.ToString() ?? throw new JsonReaderException();
}
