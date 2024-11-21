using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LLC_MOD_Toolbox.Helpers
{
    public static class JsonHelper
    {
        /// <summary>
        /// JSON 序列化设置，使用驼峰命名法，忽略空值
        /// </summary>
        private static readonly JsonSerializerSettings camelCaseJsonSettings = new()
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
        /// <param name="jsonPayload"></param>
        /// <returns></returns>
        public static PrimaryNodeList DeserializePrimaryNodeList(string jsonPayload)
            => JsonConvert.DeserializeObject<PrimaryNodeList>(jsonPayload, camelCaseJsonSettings)
                ?? new PrimaryNodeList();


        public static Task<string> DeserializeTagName(string jsonPayload)
            => Task.FromResult(
                JObject.Parse(jsonPayload).GetValue("tag_name")?.ToString()[1..]
                ?? string.Empty);

        public static Task<string> DeserializeHash(string jsonPayload)
            => Task.FromResult(
                JObject.Parse(jsonPayload).GetValue("hash")?.ToString()
                ?? string.Empty);
    }
}
