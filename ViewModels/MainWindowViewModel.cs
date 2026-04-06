using System.Windows.Input;
using LLC_MOD_Toolbox.Infrastructure;
using System.Collections.ObjectModel;

namespace LLC_MOD_Toolbox.ViewModels
{
    public enum MainPage
    {
        Install,
        Link,
        Greytest,
        Settings,
        About,
        EasterEgg,
        Announcement
    }

    public enum InstallSubPage
    {
        Auto,
        Font,
        Skin,
        Gacha
    }

    public sealed class MainWindowViewModel : ObservableObject
    {
        private MainPage _currentPage = MainPage.Install;
        private InstallSubPage _currentInstallPage = InstallSubPage.Auto;
        private bool _isInstalling;
        private bool _isEasterEggUnlocked;
        private bool _isGlobalOperationsEnabled = true;
        private bool _arePrimaryActionsEnabled = true;
        private bool _isAnnouncementButtonEnabled;
        private bool _showAnnouncementTip = true;
        private string _announcementText = string.Empty;
        private string _announcementTip = string.Empty;
        private string _loadingText = string.Empty;
        private string _currentVersionText = "当前版本：获取中";
        private string _latestVersionText = "最新版本：获取中";
        private string _mirrorChyanButtonText = "填写秘钥";
        private string _skinDescription = "默认皮肤。";
        private string _fontReplacePath = "输入字体路径";
        private string _fontSizeText = "16";
        private string _greytestToken = "请输入秘钥";
        private string? _selectedNodeOption;
        private string? _selectedApiOption;
        private string? _selectedSkinOption;
        private bool _suppressNodeSelectionNotification;
        private bool _suppressApiSelectionNotification;
        private bool _suppressSkinSelectionNotification;

        public MainWindowViewModel()
        {
            MinimizeCommand = new RelayCommand(() => MinimizeRequested?.Invoke());
            CloseCommand = new RelayCommand(() => CloseRequested?.Invoke());
            OpenLinkCommand = new RelayCommand<string>(OpenLink, CanOpenLink);
            ShowInstallCommand = new RelayCommand(() => SelectMainPage(MainPage.Install, InstallSubPage.Auto));
            ShowLinkCommand = new RelayCommand(() => SelectMainPage(MainPage.Link));
            ShowGreytestCommand = new RelayCommand(() => SelectMainPage(MainPage.Greytest));
            ShowSettingsCommand = new RelayCommand(() => SelectMainPage(MainPage.Settings));
            ShowAboutCommand = new RelayCommand(() => SelectMainPage(MainPage.About));
            ShowEasterEggCommand = new RelayCommand(() => SelectMainPage(MainPage.EasterEgg), () => IsEasterEggUnlocked);
            ShowAutoInstallCommand = new RelayCommand(() => SelectInstallSubPage(InstallSubPage.Auto));
            ShowFontReplaceCommand = new RelayCommand(() => SelectInstallSubPage(InstallSubPage.Font));
            ShowSkinCommand = new RelayCommand(() => SelectInstallSubPage(InstallSubPage.Skin));
            ShowGachaCommand = new AsyncRelayCommand(ShowGachaAsync);
            StartInstallCommand = new AsyncRelayCommand(RequestAutoInstallAsync, () => IsAutoInstallStartVisible);
            StartGachaRollCommand = new AsyncRelayCommand(RequestGachaRollAsync);
            AcknowledgeAnnouncementCommand = new AsyncRelayCommand(AcknowledgeAnnouncementAsync, () => IsAnnouncementButtonEnabled);
            UninstallCommand = new AsyncRelayCommand(RequestUninstallAsync);
            CreateLauncherShortcutCommand = new RelayCommand(() => LauncherShortcutRequested?.Invoke());
            ShowLauncherHelpCommand = new RelayCommand(() => LauncherHelpRequested?.Invoke());
            ShowNodeHelpCommand = new RelayCommand(() => NodeHelpRequested?.Invoke());
            ShowHotUpdateHelpCommand = new RelayCommand(() => HotUpdateHelpRequested?.Invoke());
            ConfigureMirrorChyanCommand = new RelayCommand(() => MirrorChyanConfigRequested?.Invoke());
            ExploreFontCommand = new RelayCommand(() => ExploreFontRequested?.Invoke());
            PreviewFontCommand = new AsyncRelayCommand(RequestPreviewFontAsync);
            ApplyFontCommand = new RelayCommand(() => ApplyFontRequested?.Invoke());
            RestoreFontCommand = new RelayCommand(() => RestoreFontRequested?.Invoke());
            StartGreytestCommand = new AsyncRelayCommand(RequestGreytestStartAsync);
            ShowGreytestInfoCommand = new RelayCommand(() => GreytestInfoRequested?.Invoke());
        }

        public event Action? CloseRequested;
        public event Action? MinimizeRequested;
        public event Action<string>? LinkRequested;
        public event Func<Task>? GachaRequested;
        public event Func<Task>? AutoInstallRequested;
        public event Func<Task>? GachaRollRequested;
        public event Func<Task>? AnnouncementAcknowledged;
        public event Func<Task>? UninstallRequested;
        public event Action? LauncherShortcutRequested;
        public event Action? LauncherHelpRequested;
        public event Action? NodeHelpRequested;
        public event Action? HotUpdateHelpRequested;
        public event Action? MirrorChyanConfigRequested;
        public event Action<string?>? NodeSelectionChanged;
        public event Action<string?>? ApiSelectionChanged;
        public event Action<string?>? SkinSelectionChanged;
        public event Action? ExploreFontRequested;
        public event Func<Task>? PreviewFontRequested;
        public event Action? ApplyFontRequested;
        public event Action? RestoreFontRequested;
        public event Func<Task>? GreytestStartRequested;
        public event Action? GreytestInfoRequested;

        public ICommand MinimizeCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand OpenLinkCommand { get; }
        public ICommand ShowInstallCommand { get; }
        public ICommand ShowLinkCommand { get; }
        public ICommand ShowGreytestCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowAboutCommand { get; }
        public ICommand ShowEasterEggCommand { get; }
        public ICommand ShowAutoInstallCommand { get; }
        public ICommand ShowFontReplaceCommand { get; }
        public ICommand ShowSkinCommand { get; }
        public ICommand ShowGachaCommand { get; }
        public ICommand StartInstallCommand { get; }
        public ICommand StartGachaRollCommand { get; }
        public ICommand AcknowledgeAnnouncementCommand { get; }
        public ICommand UninstallCommand { get; }
        public ICommand CreateLauncherShortcutCommand { get; }
        public ICommand ShowLauncherHelpCommand { get; }
        public ICommand ShowNodeHelpCommand { get; }
        public ICommand ShowHotUpdateHelpCommand { get; }
        public ICommand ConfigureMirrorChyanCommand { get; }
        public ICommand ExploreFontCommand { get; }
        public ICommand PreviewFontCommand { get; }
        public ICommand ApplyFontCommand { get; }
        public ICommand RestoreFontCommand { get; }
        public ICommand StartGreytestCommand { get; }
        public ICommand ShowGreytestInfoCommand { get; }

        public ObservableCollection<string> NodeOptions { get; } = [];
        public ObservableCollection<string> ApiOptions { get; } = [];
        public ObservableCollection<string> SkinOptions { get; } = [];

        public MainPage CurrentPage
        {
            get => _currentPage;
            private set
            {
                if (!SetProperty(ref _currentPage, value))
                {
                    return;
                }

                NotifyNavigationStateChanged();
            }
        }

        public InstallSubPage CurrentInstallPage
        {
            get => _currentInstallPage;
            private set
            {
                if (!SetProperty(ref _currentInstallPage, value))
                {
                    return;
                }

                NotifyNavigationStateChanged();
            }
        }

        public bool IsInstalling
        {
            get => _isInstalling;
            set
            {
                if (!SetProperty(ref _isInstalling, value))
                {
                    return;
                }

                OnPropertyChanged(nameof(IsAutoInstallStartVisible));
                OnPropertyChanged(nameof(IsAutoInstallBusyVisible));
                NotifyCommandState();
            }
        }

        public bool IsEasterEggUnlocked
        {
            get => _isEasterEggUnlocked;
            set
            {
                if (!SetProperty(ref _isEasterEggUnlocked, value))
                {
                    return;
                }

                OnPropertyChanged(nameof(IsEasterEggVisible));
                NotifyCommandState();
            }
        }

        public bool IsGlobalOperationsEnabled
        {
            get => _isGlobalOperationsEnabled;
            set
            {
                if (!SetProperty(ref _isGlobalOperationsEnabled, value))
                {
                    return;
                }

                OnPropertyChanged(nameof(IsOverlayVisible));
            }
        }

        public bool ArePrimaryActionsEnabled
        {
            get => _arePrimaryActionsEnabled;
            set => SetProperty(ref _arePrimaryActionsEnabled, value);
        }

        public string AnnouncementText
        {
            get => _announcementText;
            set => SetProperty(ref _announcementText, value);
        }

        public string AnnouncementTip
        {
            get => _announcementTip;
            set => SetProperty(ref _announcementTip, value);
        }

        public bool IsAnnouncementButtonEnabled
        {
            get => _isAnnouncementButtonEnabled;
            set
            {
                if (!SetProperty(ref _isAnnouncementButtonEnabled, value))
                {
                    return;
                }

                NotifyCommandState();
            }
        }

        public bool ShowAnnouncementTip
        {
            get => _showAnnouncementTip;
            set
            {
                if (!SetProperty(ref _showAnnouncementTip, value))
                {
                    return;
                }

                OnPropertyChanged(nameof(IsAnnouncementTipVisible));
            }
        }

        public string LoadingText
        {
            get => _loadingText;
            set => SetProperty(ref _loadingText, value);
        }

        public string CurrentVersionText
        {
            get => _currentVersionText;
            set => SetProperty(ref _currentVersionText, value);
        }

        public string LatestVersionText
        {
            get => _latestVersionText;
            set => SetProperty(ref _latestVersionText, value);
        }

        public string MirrorChyanButtonText
        {
            get => _mirrorChyanButtonText;
            set => SetProperty(ref _mirrorChyanButtonText, value);
        }

        public string SkinDescription
        {
            get => _skinDescription;
            set => SetProperty(ref _skinDescription, value);
        }

        public string? SelectedNodeOption
        {
            get => _selectedNodeOption;
            set
            {
                if (!SetProperty(ref _selectedNodeOption, value) || _suppressNodeSelectionNotification)
                {
                    return;
                }

                NodeSelectionChanged?.Invoke(value);
            }
        }

        public string? SelectedApiOption
        {
            get => _selectedApiOption;
            set
            {
                if (!SetProperty(ref _selectedApiOption, value) || _suppressApiSelectionNotification)
                {
                    return;
                }

                ApiSelectionChanged?.Invoke(value);
            }
        }

        public string? SelectedSkinOption
        {
            get => _selectedSkinOption;
            set
            {
                if (!SetProperty(ref _selectedSkinOption, value) || _suppressSkinSelectionNotification)
                {
                    return;
                }

                SkinSelectionChanged?.Invoke(value);
            }
        }

        public string FontReplacePath
        {
            get => _fontReplacePath;
            set => SetProperty(ref _fontReplacePath, value);
        }

        public string FontSizeText
        {
            get => _fontSizeText;
            set => SetProperty(ref _fontSizeText, value);
        }

        public string GreytestToken
        {
            get => _greytestToken;
            set => SetProperty(ref _greytestToken, value);
        }

        public bool IsInstallPageVisible => CurrentPage == MainPage.Install && CurrentInstallPage == InstallSubPage.Auto;
        public bool IsFontPageVisible => CurrentPage == MainPage.Install && CurrentInstallPage == InstallSubPage.Font;
        public bool IsSkinPageVisible => CurrentPage == MainPage.Install && CurrentInstallPage == InstallSubPage.Skin;
        public bool IsGachaPageVisible => CurrentPage == MainPage.Install && CurrentInstallPage == InstallSubPage.Gacha;
        public bool IsLinkPageVisible => CurrentPage == MainPage.Link;
        public bool IsGreytestPageVisible => CurrentPage == MainPage.Greytest;
        public bool IsSettingsPageVisible => CurrentPage == MainPage.Settings;
        public bool IsAboutPageVisible => CurrentPage == MainPage.About;
        public bool IsAnnouncementPageVisible => CurrentPage == MainPage.Announcement;
        public bool IsEasterEggPageVisible => CurrentPage == MainPage.EasterEgg;
        public bool IsInstallTabSelected => CurrentPage == MainPage.Install;
        public bool IsInstallTabsVisible => CurrentPage == MainPage.Install;
        public bool IsInstallTabsDisabledOverlayVisible => CurrentPage != MainPage.Install;
        public bool IsAutoInstallStartVisible => !IsInstalling;
        public bool IsAutoInstallBusyVisible => IsInstalling;
        public bool IsOverlayVisible => !IsGlobalOperationsEnabled;
        public bool IsAnnouncementTipVisible => ShowAnnouncementTip;
        public bool IsEasterEggVisible => IsEasterEggUnlocked;

        public void SelectMainPage(MainPage page, InstallSubPage? installSubPage = null)
        {
            CurrentPage = page;
            if (installSubPage.HasValue)
            {
                CurrentInstallPage = installSubPage.Value;
            }
        }

        public void SelectInstallSubPage(InstallSubPage page)
        {
            CurrentPage = MainPage.Install;
            CurrentInstallPage = page;
        }

        public void ShowAnnouncement(string text, bool buttonEnabled, bool showTip)
        {
            AnnouncementText = text;
            IsAnnouncementButtonEnabled = buttonEnabled;
            ShowAnnouncementTip = showTip;
            CurrentPage = MainPage.Announcement;
        }

        public void SetNodeOptions(IEnumerable<string> options)
        {
            ReplaceCollection(NodeOptions, options);
        }

        public void SetApiOptions(IEnumerable<string> options)
        {
            ReplaceCollection(ApiOptions, options);
        }

        public void SetSkinOptions(IEnumerable<string> options)
        {
            ReplaceCollection(SkinOptions, options);
        }

        public void UpdateSelectedNodeOption(string? option)
        {
            SetSelection(ref _suppressNodeSelectionNotification, () => SelectedNodeOption = option);
        }

        public void UpdateSelectedApiOption(string? option)
        {
            SetSelection(ref _suppressApiSelectionNotification, () => SelectedApiOption = option);
        }

        public void UpdateSelectedSkinOption(string? option)
        {
            SetSelection(ref _suppressSkinSelectionNotification, () => SelectedSkinOption = option);
        }

        private async Task ShowGachaAsync()
        {
            if (GachaRequested != null)
            {
                await GachaRequested.Invoke();
            }
        }

        private async Task RequestAutoInstallAsync()
        {
            if (AutoInstallRequested != null)
            {
                await AutoInstallRequested.Invoke();
            }
        }

        private async Task RequestGachaRollAsync()
        {
            if (GachaRollRequested != null)
            {
                await GachaRollRequested.Invoke();
            }
        }

        private async Task AcknowledgeAnnouncementAsync()
        {
            if (AnnouncementAcknowledged != null)
            {
                await AnnouncementAcknowledged.Invoke();
            }
        }

        private async Task RequestUninstallAsync()
        {
            if (UninstallRequested != null)
            {
                await UninstallRequested.Invoke();
            }
        }

        private async Task RequestPreviewFontAsync()
        {
            if (PreviewFontRequested != null)
            {
                await PreviewFontRequested.Invoke();
            }
        }

        private async Task RequestGreytestStartAsync()
        {
            if (GreytestStartRequested != null)
            {
                await GreytestStartRequested.Invoke();
            }
        }

        private void OpenLink(string? url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                LinkRequested?.Invoke(url);
            }
        }

        private static bool CanOpenLink(string? url)
        {
            return !string.IsNullOrWhiteSpace(url);
        }

        private void NotifyNavigationStateChanged()
        {
            OnPropertyChanged(nameof(IsInstallPageVisible));
            OnPropertyChanged(nameof(IsFontPageVisible));
            OnPropertyChanged(nameof(IsSkinPageVisible));
            OnPropertyChanged(nameof(IsGachaPageVisible));
            OnPropertyChanged(nameof(IsLinkPageVisible));
            OnPropertyChanged(nameof(IsGreytestPageVisible));
            OnPropertyChanged(nameof(IsSettingsPageVisible));
            OnPropertyChanged(nameof(IsAboutPageVisible));
            OnPropertyChanged(nameof(IsAnnouncementPageVisible));
            OnPropertyChanged(nameof(IsEasterEggPageVisible));
            OnPropertyChanged(nameof(IsInstallTabSelected));
            OnPropertyChanged(nameof(IsInstallTabsVisible));
            OnPropertyChanged(nameof(IsInstallTabsDisabledOverlayVisible));
        }

        private void NotifyCommandState()
        {
            if (ShowEasterEggCommand is RelayCommand showEasterEggCommand)
            {
                showEasterEggCommand.NotifyCanExecuteChanged();
            }

            if (AcknowledgeAnnouncementCommand is AsyncRelayCommand acknowledgeAnnouncementCommand)
            {
                acknowledgeAnnouncementCommand.NotifyCanExecuteChanged();
            }

            if (StartInstallCommand is AsyncRelayCommand startInstallCommand)
            {
                startInstallCommand.NotifyCanExecuteChanged();
            }
        }

        private static void ReplaceCollection(ObservableCollection<string> target, IEnumerable<string> values)
        {
            target.Clear();
            foreach (var value in values)
            {
                target.Add(value);
            }
        }

        private static void SetSelection(ref bool suppressFlag, Action updateSelection)
        {
            suppressFlag = true;
            try
            {
                updateSelection();
            }
            finally
            {
                suppressFlag = false;
            }
        }
    }
}
