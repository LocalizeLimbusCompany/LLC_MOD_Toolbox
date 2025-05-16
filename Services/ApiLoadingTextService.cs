using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.Services
{
    internal class ApiLoadingTextService(Config config, IHttpClientFactory httpClientFactory)
        : ILoadingTextService
    {
        [Experimental("Api")]
        public async Task<string> GetLoadingTextAsync()
        {
            // TODO 通过 API 获取随机文本
            // 有两种预想的实现方式:
            // 从 API 获取全部文本, 然后随机选择一个（这种方式可能会导致 API 过载）
            // 以参数的形式获取随机文本
            // 要求 API 自行实现随机文本的获取
            var url = string.Format(config.ApiNode.Endpoint, "111");
            var client = httpClientFactory.CreateClient("ApiLoadingTextService");
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var loadingText = await response.Content.ReadAsStringAsync();
            return loadingText;
        }
    }
}
