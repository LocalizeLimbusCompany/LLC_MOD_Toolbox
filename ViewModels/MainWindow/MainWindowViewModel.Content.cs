using System.Windows.Threading;

namespace LLC_MOD_Toolbox.ViewModels
{
    public sealed partial class MainWindowViewModel
    {
        private void PresentAnnouncement(Services.Content.AnnouncementResult result)
        {
            if (!Announcement.Present(result)) return;
            ArePrimaryActionsEnabled = false;
            SelectMainPage(MainPage.Announcement);
        }

        private async Task AcknowledgeAnnouncementAsync()
        {
            SelectMainPage(MainPage.Install, InstallSubPage.Auto);
            ArePrimaryActionsEnabled = true;
            if (_config.Settings.Installation.InstallOnLaunch || _appState.IsLauncherMode)
                await ExecuteAutoInstallAsync();
        }

        private async Task InitializeLoadingTextAsync()
        {
            await _loadingTextService.InitializeAsync();
            LoadingText = _loadingTextService.GetRandomLoadingText();
        }

        private void StartLoadingTextRotation()
        {
            _loadingTextTimer ??= new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            _loadingTextTimer.Tick -= OnLoadingTextTimerTick;
            _loadingTextTimer.Tick += OnLoadingTextTimerTick;
            _loadingTextTimer.Start();
        }
    }
}
