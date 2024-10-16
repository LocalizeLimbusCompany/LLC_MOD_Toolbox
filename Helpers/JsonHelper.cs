using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LLC_MOD_Toolbox.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            }
        };

        public static async Task DeserializePrimaryNodeList(string jsonPayload)
        {
            var _ = JsonConvert.DeserializeObject<PrimaryNodeList>(jsonPayload, JsonSettings);
            await Task.CompletedTask;
        }

        public static Task<string> DeserializeTagName(string jsonPayload)
        {
            var output = JObject.Parse(jsonPayload);
            if (output.TryGetValue("tag_name", out var latestVersionTag))
            {
                return Task.FromResult(latestVersionTag.ToString());
            }
            return Task.FromResult(string.Empty);
        }
    }
}
