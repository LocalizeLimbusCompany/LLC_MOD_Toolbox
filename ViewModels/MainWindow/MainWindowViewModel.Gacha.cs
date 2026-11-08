namespace LLC_MOD_Toolbox.ViewModels
{
    public sealed partial class MainWindowViewModel
    {
        private async Task HandleGachaNavigationAsync()
        {
            if (!_gachaService.IsInitialized)
            {
                bool confirmed = _dialogService.ShowConfirm("本抽卡模拟器资源来源自维基，可能信息更新不准时。\n本模拟器 不 会 对您的游戏数据造成任何影响。\n若您已知悉，请点击\"确定\"进行初始化。", "提示");
                if (confirmed)
                {
                    SelectInstallSubPage(InstallSubPage.Gacha);
                    IsGlobalOperationsEnabled = false;
                    await _gachaService.InitializeAsync();
                    IsGlobalOperationsEnabled = true;
                    if (_gachaService.InitializationFailed)
                        _dialogService.ShowMessage("初始化失败。请检查网络情况。");
                    else
                        _dialogService.ShowMessage("初始化完成。", "提示");
                }
            }
            else
            {
                SelectInstallSubPage(InstallSubPage.Gacha);
            }
        }

        private Task ExecuteGachaRollAsync()
        {
            if (_gachaService.InitializationFailed)
            {
                _dialogService.ShowMessage("初始化失败，无法进行抽卡操作。", "提示");
                return Task.CompletedTask;
            }
            // Gacha roll UI animation will be handled by MainWindow subscribing to a roll event
            GachaRollExecuted?.Invoke(_gachaService.Roll());
            return Task.CompletedTask;
        }

        public event Action<Services.Gacha.GachaRollResult>? GachaRollExecuted;

        public void ShowGachaMessage(string message, string title = "提示") =>
            _dialogService.ShowMessage(message, title);
    }
}
