using LLC_MOD_Toolbox.Infrastructure;
using LLC_MOD_Toolbox.Models;
using System.Diagnostics;

namespace LLC_MOD_Toolbox.ViewModels
{
    public sealed partial class MainWindowViewModel
    {
        public MainPage CurrentPage
        {
            get => _currentPage;
            private set { if (SetProperty(ref _currentPage, value)) NotifyNavigationStateChanged(); }
        }

        public InstallSubPage CurrentInstallPage
        {
            get => _currentInstallPage;
            private set { if (SetProperty(ref _currentInstallPage, value)) NotifyNavigationStateChanged(); }
        }

        public bool IsInstalling
        {
            get => _isInstalling;
            set
            {
                if (!SetProperty(ref _isInstalling, value)) return;
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
                if (!SetProperty(ref _isEasterEggUnlocked, value)) return;
                OnPropertyChanged(nameof(IsEasterEggVisible));
                NotifyCommandState();
            }
        }

        public bool IsGlobalOperationsEnabled
        {
            get => _isGlobalOperationsEnabled;
            set { if (SetProperty(ref _isGlobalOperationsEnabled, value)) OnPropertyChanged(nameof(IsOverlayVisible)); }
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
            set { if (SetProperty(ref _isAnnouncementButtonEnabled, value)) NotifyCommandState(); }
        }

        public bool ShowAnnouncementTip
        {
            get => _showAnnouncementTip;
            set { if (SetProperty(ref _showAnnouncementTip, value)) OnPropertyChanged(nameof(IsAnnouncementTipVisible)); }
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
                if (!SetProperty(ref _selectedNodeOption, value) || _suppressNodeSelectionNotification) return;
                HandleNodeSelectionChanged(value);
            }
        }

        public string? SelectedApiOption
        {
            get => _selectedApiOption;
            set
            {
                if (!SetProperty(ref _selectedApiOption, value) || _suppressApiSelectionNotification) return;
                HandleApiSelectionChanged(value);
            }
        }

        public SkinCatalogItem? SelectedSkinOption
        {
            get => _selectedSkinOption;
            set
            {
                if (!SetProperty(ref _selectedSkinOption, value) || _suppressSkinSelectionNotification) return;
                HandleSkinSelectionChanged(value);
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

        public bool IsMirrorChyanLogoVisible
        {
            get => _isMirrorChyanLogoVisible;
            set => SetProperty(ref _isMirrorChyanLogoVisible, value);
        }

        public bool IsGreytestLogoVisible
        {
            get => _isGreytestLogoVisible;
            set => SetProperty(ref _isGreytestLogoVisible, value);
        }

        public float ProgressPercentage
        {
            get => _progressPercentage;
            set => SetProperty(ref _progressPercentage, value);
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
                CurrentInstallPage = installSubPage.Value;
        }

        public void SelectInstallSubPage(InstallSubPage page)
        {
            CurrentPage = MainPage.Install;
            CurrentInstallPage = page;
        }

        public void UpdateSelectedNodeOption(string? option)
        {
            _suppressNodeSelectionNotification = true;
            try { SelectedNodeOption = option; }
            finally { _suppressNodeSelectionNotification = false; }
        }

        public void UpdateSelectedApiOption(string? option)
        {
            _suppressApiSelectionNotification = true;
            try { SelectedApiOption = option; }
            finally { _suppressApiSelectionNotification = false; }
        }

        public void UpdateSelectedSkinOption(SkinCatalogItem? option)
        {
            _suppressSkinSelectionNotification = true;
            try { SelectedSkinOption = option; }
            finally { _suppressSkinSelectionNotification = false; }
        }

        public void SetNodeOptions(IEnumerable<string> options) => ReplaceCollection(NodeOptions, options);
        public void SetApiOptions(IEnumerable<string> options) => ReplaceCollection(ApiOptions, options);
        public void SetSkinOptions(IEnumerable<SkinCatalogItem> options) => ReplaceCollection(SkinOptions, options);

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
            (ShowEasterEggCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (AcknowledgeAnnouncementCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
            (StartInstallCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }

        private static void ReplaceCollection<T>(System.Collections.ObjectModel.ObservableCollection<T> target, IEnumerable<T> values)
        {
            target.Clear();
            foreach (var v in values) target.Add(v);
        }

        // 1. https://www.bilibili.com/video/BV1c49DBjEzq 《明日方舟》EP - Innocence
        // 2. https://www.bilibili.com/video/BV1GJ411x7h7 【官方 MV】Never Gonna Give You Up - Rick Astley
        private static readonly string[] randomLinks = ["https://www.bilibili.com/video/BV1c49DBjEzq", "https://www.bilibili.com/video/BV1GJ411x7h7"];

        private static void OpenUrl(string url)
        {
            string finalUrl = url;
            if (url == "RandomLink")
            {
                Random random = new();
                finalUrl = randomLinks[random.Next(randomLinks.Length)];
            }
            Log.logger.Info("打开了网址：" + finalUrl);
            Process.Start(new ProcessStartInfo(finalUrl) { UseShellExecute = true });
        }

        private void OpenLink(string? url)
        {
            if (!string.IsNullOrWhiteSpace(url)) { 
                OpenUrl(url); 
            }
        }

        private static bool CanOpenLink(string? url) => !string.IsNullOrWhiteSpace(url);
    }
}
