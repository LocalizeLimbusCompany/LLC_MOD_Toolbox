using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LLC_MOD_Toolbox.Infrastructure;
using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services;
using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.Content;
using LLC_MOD_Toolbox.Services.Font;
using LLC_MOD_Toolbox.Services.Gacha;
using LLC_MOD_Toolbox.Services.Greytest;
using LLC_MOD_Toolbox.Services.Installation;
using LLC_MOD_Toolbox.Services.Network;
using LLC_MOD_Toolbox.Services.Skin;
using LLC_MOD_Toolbox.Services.UI;
using LLC_MOD_Toolbox.Services.Update;
using LLC_MOD_Toolbox.Services.Version;

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

    public sealed partial class MainWindowViewModel : ObservableObject
    {
        private readonly IInstallService _installService;
        private readonly IUninstallService _uninstallService;
        private readonly IGachaService _gachaService;
        private readonly IGreytestService _greytestService;
        private readonly INodeService _nodeService;
        private readonly IMirrorChyanService _mirrorChyanService;
        private readonly IToolboxUpdateService _updateService;
        private readonly IAnnouncementService _announcementService;
        private readonly ILoadingTextService _loadingTextService;
        private readonly IVersionService _versionService;
        private readonly IFontService _fontService;
        private readonly ISkinService _skinService;
        private readonly ISkinMusicService _skinMusicService;
        private readonly IDialogService _dialogService;
        private readonly IHttpService _httpService;
        private readonly AppState _appState;
        private readonly ConfigurationManager _config;

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
        private SkinCatalogItem? _selectedSkinOption;
        private bool _suppressNodeSelectionNotification;
        private bool _suppressApiSelectionNotification;
        private bool _suppressSkinSelectionNotification;
        private bool _isSkinMusicAvailable;
        private bool _isSkinMusicEnabled;
        private string _skinMusicButtonText = "音乐开关";
        private bool _isMirrorChyanLogoVisible;
        private bool _isGreytestLogoVisible;
        private float _progressPercentage;
        private bool _hasNewAnno;
        private DispatcherTimer? _annoTimer;
        private int _annoLastTime;

        public MainWindowViewModel(
            IInstallService installService,
            IUninstallService uninstallService,
            IGachaService gachaService,
            IGreytestService greytestService,
            INodeService nodeService,
            IMirrorChyanService mirrorChyanService,
            IToolboxUpdateService updateService,
            IAnnouncementService announcementService,
            ILoadingTextService loadingTextService,
            IVersionService versionService,
            IFontService fontService,
            ISkinService skinService,
            ISkinMusicService skinMusicService,
            IDialogService dialogService,
            IHttpService httpService,
            AppState appState,
            ConfigurationManager config)
        {
            _installService = installService;
            _uninstallService = uninstallService;
            _gachaService = gachaService;
            _greytestService = greytestService;
            _nodeService = nodeService;
            _mirrorChyanService = mirrorChyanService;
            _updateService = updateService;
            _announcementService = announcementService;
            _loadingTextService = loadingTextService;
            _versionService = versionService;
            _fontService = fontService;
            _skinService = skinService;
            _skinMusicService = skinMusicService;
            _dialogService = dialogService;
            _httpService = httpService;
            _appState = appState;
            _config = config;

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
            ShowGachaCommand = new AsyncRelayCommand(HandleGachaNavigationAsync);
            StartInstallCommand = new AsyncRelayCommand(ExecuteAutoInstallAsync, () => IsAutoInstallStartVisible);
            StartGachaRollCommand = new AsyncRelayCommand(ExecuteGachaRollAsync);
            AcknowledgeAnnouncementCommand = new AsyncRelayCommand(AcknowledgeAnnouncementAsync, () => IsAnnouncementButtonEnabled);
            UninstallCommand = new AsyncRelayCommand(ExecuteUninstallAsync);
            CreateLauncherShortcutCommand = new RelayCommand(HandleLauncherShortcut);
            ShowLauncherHelpCommand = new RelayCommand(() => OpenUrl("https://www.zeroasso.top/docs/install/hotupdate"));
            ShowNodeHelpCommand = new RelayCommand(() => OpenUrl("https://www.zeroasso.top/docs/configuration/nodes"));
            ShowHotUpdateHelpCommand = new RelayCommand(HandleHotUpdateHelp);
            ConfigureMirrorChyanCommand = new RelayCommand(HandleMirrorChyanConfig);
            ExploreFontCommand = new RelayCommand(HandleExploreFont);
            PreviewFontCommand = new AsyncRelayCommand(HandlePreviewFontAsync);
            ApplyFontCommand = new RelayCommand(HandleApplyFont);
            RestoreFontCommand = new RelayCommand(HandleRestoreFont);
            StartGreytestCommand = new AsyncRelayCommand(HandleGreytestStartAsync);
            ShowGreytestInfoCommand = new RelayCommand(() => OpenUrl("https://www.zeroasso.top/docs/community/llcdev"));
            ToggleSkinMusicCommand = new RelayCommand(ToggleSkinMusic);

            _mirrorChyanService.ModeDisabledByError += OnMirrorChyanModeDisabled;
        }

        public event Action? CloseRequested;
        public event Action? MinimizeRequested;
        public event Action? ApplySkinRequested;

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
        public ICommand ToggleSkinMusicCommand { get; }

        public ObservableCollection<string> NodeOptions { get; } = [];
        public ObservableCollection<string> ApiOptions { get; } = [];
        public ObservableCollection<SkinCatalogItem> SkinOptions { get; } = [];
    }
}
