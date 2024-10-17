using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace LLC_MOD_Toolbox.Helpers;

public static class HttpHelper
{
    private static readonly HttpClient httpClient = new();
    static HttpHelper()
    {
        httpClient.DefaultRequestHeaders.Add("User-Agent", "LLC_MOD_Toolbox");
    }

    /// <summary>
    /// Get an http response from sending request to a specific url
    /// 获取从发送请求到特定url的http响应
    /// </summary>
    /// <param name="url"></param>
    /// <param name="method">Http method being used (default: <see cref="HttpMethod.Get"/>)</param>
    /// <exception cref="HttpRequestException">当网络连接不良时直接断言</exception>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> GetResponseAsync(string url, [Optional] HttpMethod method)
    {
        using var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = method ?? HttpMethod.Get
        };
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return response;
    }

    /// <summary>
    /// 获取网页内容，经常用于获取json
    /// </summary>
    /// <exception cref="HttpRequestException">当网络连接不良时直接断言</exception>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<string> GetStringAsync(string url)
    {
        var response = await GetResponseAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<BitmapImage> GetImageAsync(string url)
    {
        using var response = await GetResponseAsync(url);
        response.EnsureSuccessStatusCode();
        using var stream = await response.Content.ReadAsStreamAsync();
        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = stream;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        return image;
    }

    /// <summary>
    /// 老实说或许它不应该在这个类里……
    /// </summary>
    /// <param name="url">要打开的文件</param>
    public static void LaunchUrl(string url)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
        {
            UseShellExecute = true
        });
    }
}
