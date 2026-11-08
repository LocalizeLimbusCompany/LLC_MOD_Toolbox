using LLC_MOD_Toolbox.Infrastructure;
using LLC_MOD_Toolbox.Infrastructure.DependencyInjection;
using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.Compatibility;
using LLC_MOD_Toolbox.Services.Telemetry;
using LLC_MOD_Toolbox.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "App.config", ConfigFileExtension = "config", Watch = true)]
namespace LLC_MOD_Toolbox
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; } = null!;
        private static Mutex? _mutex;
        private bool _submitTelemetryAfterStartup;
        private CancellationTokenSource? _telemetryCancellation;
        private Task? _telemetryTask;

        protected override void OnStartup(StartupEventArgs e)
        {
            StartupDiagnostics.Start();
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            ApplyWpfCompatibilityWorkaround();

            bool createdNew;
            _mutex = new Mutex(true, "LLC_MOD_TOOLBOX", out createdNew);
            if (!createdNew)
            {
                UniversalDialog.ShowMessage("已有工具箱在运行中！", "提示", null, null);
                Current.Shutdown();
                return;
            }

            var services = new ServiceCollection();
            services.AddToolboxServices(AppContext.BaseDirectory);
            Services = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
            StartupDiagnostics.Mark("Services configured");

            ShutdownMode previousShutdownMode = ShutdownMode;
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            if (!EnsureAgreementAccepted())
            {
                Current.Shutdown();
                return;
            }
            StartupDiagnostics.Mark("Agreement accepted");

            var mainWindow = Services.GetRequiredService<MainWindow>();
            StartupDiagnostics.Mark("Main window constructed");
            MainWindow = mainWindow;
            if (_submitTelemetryAfterStartup)
                mainWindow.ContentRendered += (_, _) => StartTelemetry();
            mainWindow.Show();
            ShutdownMode = previousShutdownMode;

            base.OnStartup(e);

        }

        private static void ApplyWpfCompatibilityWorkaround()
        {
            if (!WineDetector.IsWine)
                return;

            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            Log.logger.Warn($"检测到 Wine/CrossOver 环境，已启用 WPF 软件渲染。{WineDetector.Description}");
        }

        private bool EnsureAgreementAccepted()
        {
            var config = Services.GetRequiredService<IConfigurationService>();
            if (!config.Settings.Telemetry.HasAcceptedAgreement)
            {
                var dialog = new AgreementDialog();
                bool accepted = dialog.ShowDialog() == true && dialog.IsAccepted;
                if (!accepted)
                    return false;

                config.Settings.Telemetry.HasAcceptedAgreement = true;
                config.Save();
                _submitTelemetryAfterStartup = true;
            }
            else
            {
                _submitTelemetryAfterStartup = true;
            }

            return true;
        }

        private void StartTelemetry()
        {
            if (_telemetryCancellation is not null)
                return;

            var cancellation = new CancellationTokenSource();
            _telemetryCancellation = cancellation;
            var telemetryService = Services.GetRequiredService<ITelemetryService>();
            _telemetryTask = Task.Run(async () =>
            {
                try
                {
                    await telemetryService.RunAsync(cancellation.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
                {
                }
                catch (Exception ex)
                {
                    Log.logger.Warn("遥测后台任务失败。", ex);
                }
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var cancellation = _telemetryCancellation;
            _telemetryCancellation = null;
            if (cancellation is not null)
            {
                cancellation.Cancel();
                if (_telemetryTask is not null)
                    _ = _telemetryTask.ContinueWith(_ => cancellation.Dispose(), TaskScheduler.Default);
                else
                    cancellation.Dispose();
            }
            (Services as IDisposable)?.Dispose();
            base.OnExit(e);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Log.logger.Error("出现了问题：\n", ex);
            UniversalDialog.ShowMessage($"运行中出现了未经处理的严重问题，且在这个错误发生后，工具箱将关闭。\n若要反馈，请带上链接或日志。\n反馈请勿！请勿截图此页面！\n错误分析原因：\n{ex.Message}", "错误", null, null);
            e.Handled = true;
            Current.Shutdown();
        }
    }
}
