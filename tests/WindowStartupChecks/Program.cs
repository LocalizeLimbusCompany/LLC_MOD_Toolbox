using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using LLC_MOD_Toolbox;
using LLC_MOD_Toolbox.Infrastructure.DependencyInjection;
using LLC_MOD_Toolbox.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;

internal static class Program
{
    [STAThread]
    private static int Main()
    {
        string directory = Path.Combine(Path.GetTempPath(), "llc-window-checks-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string originalDirectory = Environment.CurrentDirectory;
        Application.ResourceAssembly = typeof(App).Assembly;
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var imageErrors = new List<string>();
        EventManager.RegisterClassHandler(typeof(Image), Image.ImageFailedEvent,
            new EventHandler<ExceptionRoutedEventArgs>((sender, e) =>
                imageErrors.Add($"{(sender as Image)?.Name}: {e.ErrorException}")));

        try
        {
            // Shortcuts may start from any directory. Keep checks offline: construct/layout only,
            // without Show/Run and the main window's ContentRendered initialization.
            Environment.CurrentDirectory = directory;

            // GetResourceStream must resolve an embedded resource, not ContentFilePart.
            // A Content/Resource collision previously redirected this URI to a missing loose file.
            var iconResource = Application.GetResourceStream(
                new Uri("pack://application:,,,/LLC_MOD_Toolbox;component/favicon.ico"))
                ?? throw new InvalidOperationException("The application icon is not an embedded WPF resource.");
            using (var stream = iconResource.Stream)
            {
                var icon = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                if (icon.PixelWidth <= 0 || icon.PixelHeight <= 0)
                    throw new InvalidOperationException("The embedded application icon could not be decoded.");
            }
            if (!File.Exists(Path.Combine(AppContext.BaseDirectory, "favicon.ico")))
                throw new FileNotFoundException("The installer icon was not copied to the build output.");
            Console.WriteLine("PASS Embedded window icon and installer icon are available.");

            var services = new ServiceCollection();
            services.AddToolboxServices(directory);
            using var provider = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });

            Window[] windows =
            [
                new AgreementDialog(),
                new UniversalDialog(),
                new MirrorChyanKeyDialog(),
                new ToolboxUpdateDialog("test", "test", "test", _ => Task.CompletedTask),
                provider.GetRequiredService<MainWindow>()
            ];
            foreach (Window window in windows)
            {
                window.Measure(new Size(960, 600));
                window.Arrange(new Rect(0, 0, 960, 600));
                window.UpdateLayout();
                Console.WriteLine($"PASS Construct and lay out {window.GetType().Name}");
            }

            if (imageErrors.Count > 0)
                throw new InvalidOperationException("Startup images failed to load:\n" + string.Join("\n", imageErrors));
            Console.WriteLine("PASS Startup services, XAML and images load from a different working directory.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
        finally
        {
            app.Shutdown();
            Environment.CurrentDirectory = originalDirectory;
            Directory.Delete(directory, recursive: true);
        }
    }
}
