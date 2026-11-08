using System.IO;

namespace LLC_MOD_Toolbox.ViewModels
{
    public sealed partial class MainWindowViewModel
    {
        private void HandleExploreFont()
        {
            string? path = _dialogService.ShowOpenFileDialog("请选择你的字体", "字体文件 (*.ttf;*.otf)|*.ttf;*.otf|所有文件 (*.*)|*.*");
            if (path != null)
                FontReplacePath = Path.GetFullPath(path);
        }

        private Task HandlePreviewFontAsync()
        {
            if (!double.TryParse(FontSizeText, out _))
            {
                _dialogService.ShowMessage("请输入正确的字体大小。", "提示");
                return Task.CompletedTask;
            }
            if (!_fontService.IsValidFontFile(FontReplacePath))
            {
                _dialogService.ShowMessage("请选择正确的字体文件。", "提示");
                return Task.CompletedTask;
            }
            // Preview font requires Window access - signal via event
            FontPreviewRequested?.Invoke(FontReplacePath, double.Parse(FontSizeText));
            _dialogService.ShowMessage("已将预览文本切换为自定义字体。\n如果出现部分字显示为默认字体，可能影响游戏内显示。", "提示");
            return Task.CompletedTask;
        }

        public event Action<string, double>? FontPreviewRequested;

        private void HandleApplyFont()
        {
            if (!_fontService.IsValidFontFile(FontReplacePath))
            {
                _dialogService.ShowMessage("字体文件无效。", "提示");
                return;
            }
            try
            {
                _fontService.ApplyFontReplacement(FontReplacePath, _appState.GameDirectory);
                _dialogService.ShowMessage("字体替换成功。\n启动游戏以应用更改。", "提示");
            }
            catch (InvalidOperationException ex)
            {
                _dialogService.ShowMessage(ex.Message, "提示");
            }
        }

        private void HandleRestoreFont()
        {
            try
            {
                _fontService.RestoreOriginalFont(_appState.GameDirectory);
                _dialogService.ShowMessage("字体还原成功。\n启动游戏以应用更改。", "提示");
            }
            catch (FileNotFoundException)
            {
                _dialogService.ShowMessage("没有找到备份字体文件。", "提示");
            }
        }
    }
}
