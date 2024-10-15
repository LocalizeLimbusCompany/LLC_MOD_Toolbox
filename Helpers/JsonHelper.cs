using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LLC_MOD_Toolbox.Helpers
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerSettings JsonSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            }
        };

        public static Task<bool> DeserializeRootModel(string jsonPayload)
        {
            var rootModel = JsonConvert.DeserializeObject<PrimaryNodeList>(jsonPayload, JsonSettings);
            PrimaryNodeList.NodeList = rootModel ?? PrimaryNodeList.NodeList;
            return Task.FromResult(rootModel != null);
        }

        public static Task<string> DeserializeTagName(string jsonPayload)
        {
            var output = JObject.Parse(jsonPayload);
            var latestVersionTag = output.GetValue("tag_name")?.ToString() ?? string.Empty;
            return Task.FromResult(latestVersionTag);
        }
    }
}
