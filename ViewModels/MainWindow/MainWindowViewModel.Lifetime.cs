using System.ComponentModel;
using System.Windows.Threading;

namespace LLC_MOD_Toolbox.ViewModels;

public sealed partial class MainWindowViewModel
{
    private DispatcherTimer? _loadingTextTimer;

    private void OnLoadingTextTimerTick(object? sender, EventArgs e) =>
        LoadingText = _loadingTextService.GetRandomLoadingText();

    private void OnAnnouncementPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AnnouncementViewModel.CanAcknowledge))
            NotifyCommandState();
    }

    public void Dispose()
    {
        _loadingTextTimer?.Stop();
        if (_loadingTextTimer is not null)
            _loadingTextTimer.Tick -= OnLoadingTextTimerTick;
        Announcement.PropertyChanged -= OnAnnouncementPropertyChanged;
        _mirrorChyanService.ModeDisabledByError -= OnMirrorChyanModeDisabled;
        _skinService.SkinReloaded -= OnSkinReloaded;
        _skinService.HotReloadStatusChanged -= OnHotReloadStatusChanged;
    }
}
