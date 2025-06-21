using System.IO;
using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LLC_MOD_Toolbox.Helpers;

internal static class JsonHelper
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
    /// 根据传入的键反序列化Json对应的值
    /// </summary>
    /// <param name="jsonPayload"></param>
    /// <returns>键对应的值</returns>
    /// <exception cref="JsonReaderException">如果不存在对应键值对</exception>
    public static string DeserializeValue(string key, string jsonPayload) =>
        JObject.Parse(jsonPayload).GetValue(key)?.ToString() ?? throw new JsonReaderException();

    public static T Deserialize<T>(string jsonPayload) =>
        JsonConvert.DeserializeObject<T>(jsonPayload, camelCaseJsonSettings)
        ?? throw new JsonReaderException();

    public static void Serialize<T>(T obj, string path)
    {
        string jsonPayload = JsonConvert.SerializeObject(
            obj,
            Formatting.Indented,
            camelCaseJsonSettings
        );
        File.WriteAllText(path, jsonPayload);
    }
}
