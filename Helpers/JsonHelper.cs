﻿using LLC_MOD_Toolbox.Models;
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

        public static Task<PrimaryNodeList> DeserializePrimaryNodeList(string jsonPayload) 
            => Task.FromResult(
                JsonConvert.DeserializeObject<PrimaryNodeList>(jsonPayload, JsonSettings)
                ?? new PrimaryNodeList());


        public static Task<string> DeserializeTagName(string jsonPayload)
            => Task.FromResult(
                JObject.Parse(jsonPayload).GetValue("tag_name")?.ToString()
                ?? string.Empty);
    }
}