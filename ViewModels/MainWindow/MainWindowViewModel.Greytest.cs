namespace LLC_MOD_Toolbox.ViewModels
{
    public sealed partial class MainWindowViewModel
    {
        private async Task HandleGreytestStartAsync()
        {
            IsGlobalOperationsEnabled = false;
            var result = await _greytestService.ValidateAndActivateAsync(GreytestToken);
            if (!result.IsValid)
            {
                _dialogService.ShowMessage(result.ErrorMessage ?? "验证失败。", "提示");
                IsGlobalOperationsEnabled = true;
                return;
            }
            if (_appState.IsGreytestMode)
            {
                IsGreytestLogoVisible = true;
                _dialogService.ShowMessage($"目前Token有效。\n灰度测试模式已开启。\n备注：{result.Note}\n请在自动安装安装此秘钥对应版本汉化。", "提示");
            }
            IsGlobalOperationsEnabled = true;
        }
    }
}
