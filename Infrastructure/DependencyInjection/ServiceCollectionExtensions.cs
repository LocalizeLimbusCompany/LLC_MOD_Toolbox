using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.Content;
using LLC_MOD_Toolbox.Services.Font;
using LLC_MOD_Toolbox.Services.Gacha;
using LLC_MOD_Toolbox.Services.Greytest;
using LLC_MOD_Toolbox.Services.Installation;
using LLC_MOD_Toolbox.Services.IO;
using LLC_MOD_Toolbox.Services.Network;
using LLC_MOD_Toolbox.Services.Skin;
using LLC_MOD_Toolbox.Services.Telemetry;
using LLC_MOD_Toolbox.Services.UI;
using LLC_MOD_Toolbox.Services.Update;
using LLC_MOD_Toolbox.Services.Version;
using LLC_MOD_Toolbox.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace LLC_MOD_Toolbox.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddToolboxServices(this IServiceCollection services, string baseDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDirectory);

        services.AddSingleton<ApplicationState>();
        services.AddSingleton<IConfigurationService>(_ => new ConfigurationService(Path.Combine(baseDirectory, "config.json")));

        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IHttpService, HttpService>();
        services.AddSingleton<INodeService, NodeService>();
        services.AddSingleton<IMirrorChyanService, MirrorChyanService>();
        services.AddSingleton<IInstallService, InstallService>();
        services.AddSingleton<IUninstallService, UninstallService>();
        services.AddSingleton<IToolboxUpdateService, ToolboxUpdateService>();
        services.AddSingleton<IModVersionService, ModVersionService>();
        services.AddSingleton<IAnnouncementService, AnnouncementService>();
        services.AddSingleton<ILoadingTextService, LoadingTextService>();
        services.AddSingleton<IGachaService, GachaService>();
        services.AddSingleton<IGreytestService, GreytestService>();
        services.AddSingleton<IFontService, FontService>();
        services.AddSingleton<ISkinService, SkinService>();
        services.AddSingleton<ISkinMusicService, SkinMusicService>();
        services.AddSingleton<ITelemetryService, TelemetryService>();

        services.AddSingleton<AnnouncementViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
        return services;
    }
}
