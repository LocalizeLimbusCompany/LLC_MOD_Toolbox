using System.Windows.Threading;
using LLC_MOD_Toolbox.Infrastructure;
using LLC_MOD_Toolbox.Services.Content;

namespace LLC_MOD_Toolbox.ViewModels;

/// <summary>Owns announcement presentation and its reading countdown.</summary>
public sealed class AnnouncementViewModel : ObservableObject, IDisposable
{
    private readonly IAnnouncementService _announcementService;
    private readonly DispatcherTimer _readingTimer = new() { Interval = TimeSpan.FromSeconds(1) };
    private string _content = string.Empty;
    private string _tip = string.Empty;
    private bool _canAcknowledge;
    private bool _showTip;
    private int _remainingReadingSeconds;

    public AnnouncementViewModel(IAnnouncementService announcementService)
    {
        _announcementService = announcementService;
        _readingTimer.Tick += OnReadingTimerTick;
    }

    public string Content
    {
        get => _content;
        private set => SetProperty(ref _content, value);
    }

    public string Tip
    {
        get => _tip;
        private set => SetProperty(ref _tip, value);
    }

    public bool CanAcknowledge
    {
        get => _canAcknowledge;
        private set => SetProperty(ref _canAcknowledge, value);
    }

    public bool ShowTip
    {
        get => _showTip;
        private set => SetProperty(ref _showTip, value);
    }

    public bool HasNewAnnouncement { get; private set; }

    public Task<AnnouncementResult> CheckAsync() => _announcementService.CheckForAnnouncementAsync();

    public bool Present(AnnouncementResult result)
    {
        if (result.Error is not null || !result.HasNew)
            return false;

        _readingTimer.Stop();
        Content = result.Content;
        _announcementService.MarkAsRead(result.Version);
        HasNewAnnouncement = true;
        _remainingReadingSeconds = result.Level switch
        {
            "important" => 5,
            "special" => 15,
            _ => 0
        };
        CanAcknowledge = result.Level == "normal";
        ShowTip = !CanAcknowledge;
        UpdateReadingTip();
        if (!CanAcknowledge)
            _readingTimer.Start();
        return true;
    }

    private void OnReadingTimerTick(object? sender, EventArgs e)
    {
        if (_remainingReadingSeconds > 0)
        {
            _remainingReadingSeconds--;
            UpdateReadingTip();
        }
        else
        {
            CanAcknowledge = true;
            ShowTip = false;
            _readingTimer.Stop();
        }
    }

    private void UpdateReadingTip() =>
        Tip = $"由于本次公告较为重要，您需要继续阅读{_remainingReadingSeconds}秒。";

    public void Dispose()
    {
        _readingTimer.Stop();
        _readingTimer.Tick -= OnReadingTimerTick;
    }
}
