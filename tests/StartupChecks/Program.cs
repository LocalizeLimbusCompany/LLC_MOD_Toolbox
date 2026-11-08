using System.Diagnostics;
using System.Net;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services;
using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.Content;
using LLC_MOD_Toolbox.Services.Network;
using LLC_MOD_Toolbox.Services.UI;

var checks = new (string Name, Func<Task> Run)[]
{
    ("Stalled request is canceled without retrying beyond the budget", async () =>
    {
        using var handler = new TestHandler(async (_, token) =>
        {
            await Task.Delay(Timeout.Infinite, token);
            return Ok("unexpected");
        });
        using var client = new HttpClient(handler);
        var service = NewHttp(client);
        var clock = Stopwatch.StartNew();
        string result = await service.GetTextAsync("https://startup.invalid/slow", false, timeout: TimeSpan.FromMilliseconds(100));
        Check(result == "" && handler.Calls == 1, "Timeout should return an empty result without retrying.");
        Check(handler.LastToken.IsCancellationRequested, "The underlying request must actually be canceled.");
        Check(clock.Elapsed < TimeSpan.FromSeconds(2), "Request exceeded its budget significantly.");
    }),
    ("Retry delay consumes the same total budget", async () =>
    {
        using var handler = new TestHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)));
        using var client = new HttpClient(handler);
        string result = await NewHttp(client).GetTextAsync("https://startup.invalid/retry", false,
            delayMs: 5000, timeout: TimeSpan.FromMilliseconds(100));
        Check(result == "" && handler.Calls == 1, "A retry must not outlive the total budget.");
    }),
    ("Explicit caller cancellation propagates and is not retried", async () =>
    {
        using var handler = new TestHandler(async (_, token) =>
        {
            await Task.Delay(Timeout.Infinite, token);
            return Ok("unexpected");
        });
        using var client = new HttpClient(handler);
        using var cancellation = new CancellationTokenSource(100);
        try
        {
            await NewHttp(client).GetTextAsync("https://startup.invalid/cancel", false, cancellationToken: cancellation.Token);
            throw new InvalidOperationException("Expected cancellation.");
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            Check(handler.Calls == 1, "Caller cancellation was retried.");
        }
    }),
    ("Concurrent requests have independent timeout budgets", async () =>
    {
        using var handler = new TestHandler(async (request, token) =>
        {
            await Task.Delay(request.RequestUri!.AbsolutePath == "/slow" ? 5000 : 200, token);
            return Ok("ok");
        });
        using var client = new HttpClient(handler);
        var service = NewHttp(client);
        Task<string> slow = service.GetTextAsync("https://startup.invalid/slow", false, timeout: TimeSpan.FromMilliseconds(50));
        Task<string> fast = service.GetTextAsync("https://startup.invalid/fast", false, timeout: TimeSpan.FromSeconds(3));
        Check(await slow == "" && await fast == "ok", "Canceling one request affected another.");
    }),
    ("Transient failure can recover within budget", async () =>
    {
        int attempt = 0;
        using var handler = new TestHandler((_, _) => Task.FromResult(++attempt == 1
            ? new HttpResponseMessage(HttpStatusCode.ServiceUnavailable) : Ok("recovered")));
        using var client = new HttpClient(handler);
        string result = await NewHttp(client).GetTextAsync("https://startup.invalid/retry", false,
            delayMs: 1, timeout: TimeSpan.FromSeconds(3));
        Check(result == "recovered" && handler.Calls == 2, "Transient retry failed.");
    }),
    ("MirrorChyan errors keep their existing typed failure behavior", async () =>
    {
        using var handler = new TestHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
        {
            Content = new StringContent("{\"code\":7002}")
        }));
        using var client = new HttpClient(handler);
        try
        {
            await NewHttp(client).GetTextAsync("https://startup.invalid/mirror", false, parseErrorJson: true);
            throw new InvalidOperationException("Expected MirrorChyanException.");
        }
        catch (MirrorChyanException error)
        {
            Check(error.ErrorCode == 7002 && handler.Calls == 1, "Typed error was swallowed or retried.");
        }
    }),
    ("Expired single-entry cache is immediately usable without network", async () =>
    {
        await File.WriteAllTextAsync(CachePath(), "{\"loadingDate\":\"2000-01-01 00:00\",\"loadingTexts\":[\"cached\"]}");
        var http = new FakeHttp();
        var service = NewLoadingText(http);
        await service.InitializeAsync();
        Check(http.Calls == 0, "Initialization should only read local text.");
        for (int i = 0; i < 100; i++)
            Check(service.GetRandomLoadingText() == "cached", "Single-entry cache caused an invalid index.");
    }),
    ("Failed refresh preserves the cached text and file", async () =>
    {
        const string cached = "{\"loadingDate\":\"2000-01-01 00:00\",\"loadingTexts\":[\"cached\"]}";
        await File.WriteAllTextAsync(CachePath(), cached);
        var http = new FakeHttp { Response = "invalid json" };
        var service = NewLoadingText(http);
        await service.InitializeAsync();
        await service.RefreshAsync();
        Check(service.GetRandomLoadingText() == "cached", "Cached text was lost after refresh failure.");
        Check(await File.ReadAllTextAsync(CachePath()) == cached, "Failed refresh overwrote the local cache.");
    }),
    ("Missing cache falls back locally and can recover on refresh", async () =>
    {
        File.Delete(CachePath());
        var http = new FakeHttp { Response = "[\"fresh\"]" };
        var service = NewLoadingText(http);
        await service.InitializeAsync();
        Check(http.Calls == 0 && !string.IsNullOrWhiteSpace(service.GetRandomLoadingText()), "Missing cache blocked startup.");
        await service.RefreshAsync();
        Check(service.GetRandomLoadingText() == "fresh" && File.Exists(CachePath()), "Refresh did not repair the cache.");
        Check(http.Timeout == TimeSpan.FromSeconds(8) && http.Attempts == 2, "Metadata request did not use the bounded policy.");
    }),
    ("Malformed cache cannot fail initialization", async () =>
    {
        await File.WriteAllTextAsync(CachePath(), "broken json");
        var http = new FakeHttp();
        var service = NewLoadingText(http);
        await service.InitializeAsync();
        Check(http.Calls == 0 && !string.IsNullOrWhiteSpace(service.GetRandomLoadingText()), "Malformed cache blocked startup.");
    }),
    ("Fresh cache skips remote refresh", async () =>
    {
        await File.WriteAllTextAsync(CachePath(), "{\"loadingDate\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\",\"loadingTexts\":[\"cached\"]}");
        var http = new FakeHttp();
        var service = NewLoadingText(http);
        await service.InitializeAsync();
        await service.RefreshAsync();
        Check(http.Calls == 0, "Fresh cache triggered an unnecessary request.");
    })
};

int failures = 0;
var allChecks = checks.Concat(ConfigurationChecks.All).Concat(DiagnosticChecks.All).ToArray();
foreach (var (name, run) in allChecks)
{
    try
    {
        await run().WaitAsync(TimeSpan.FromSeconds(10));
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception ex)
    {
        failures++;
        Console.Error.WriteLine($"FAIL {name}: {ex}");
    }
}
Console.WriteLine($"{allChecks.Length - failures}/{allChecks.Length} checks passed.");
return failures == 0 ? 0 : 1;

static string CachePath() => Path.Combine(AppContext.BaseDirectory, "loadingText.json");

static HttpResponseMessage Ok(string content) => new(HttpStatusCode.OK) { Content = new StringContent(content) };
static HttpService NewHttp(HttpClient client) => new(new ApplicationState(), new NoDialogs(), client);
static LoadingTextService NewLoadingText(IHttpService http) => new(new ApplicationState(), http,
    new ConfigurationService(Path.Combine(AppContext.BaseDirectory, "test-config.json")));
static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed class NoDialogs : IDialogService
{
    public void ShowMessage(string message, string title = "提示") =>
        throw new InvalidOperationException($"Unexpected dialog: {message}");
}

sealed class TestHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) : HttpMessageHandler
{
    public int Calls { get; private set; }
    public CancellationToken LastToken { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Calls++;
        LastToken = cancellationToken;
        return send(request, cancellationToken);
    }
}

sealed class FakeHttp : IHttpService
{
    public int Calls { get; private set; }
    public TimeSpan? Timeout { get; private set; }
    public int Attempts { get; private set; }
    public string Response { get; init; } = "";

    public Task<string> GetTextAsync(string url, bool reportError = true, int maxRetries = 3, int delayMs = 300,
        bool parseErrorJson = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Calls++;
        Timeout = timeout;
        Attempts = maxRetries;
        return Task.FromResult(Response);
    }

    public Task DownloadFileAsync(string url, string path, IProgress<float>? progress = null) => throw new NotSupportedException();
    public Task DownloadFileWithoutProgressAsync(string url, string path) => throw new NotSupportedException();
}
