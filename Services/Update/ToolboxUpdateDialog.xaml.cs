using System.ComponentModel;
using System.Windows;

namespace LLC_MOD_Toolbox.Services.Update
{
    public partial class ToolboxUpdateDialog : Window
    {
        private readonly Func<IProgress<float>, Task> _updateAction;
        private bool _isUpdating;

        public ToolboxUpdateDialog(
            string currentVersion,
            string latestVersion,
            string sourceName,
            Func<IProgress<float>, Task> updateAction)
        {
            InitializeComponent();
            _updateAction = updateAction;

            SummaryText.Text = "下载完成后，工具箱会关闭并自动覆盖更新，然后重新启动。";
            VersionText.Text = $"当前版本：{currentVersion}\n最新版本：{latestVersion}";
            SourceText.Text = $"更新来源：{sourceName}";
            StatusText.Text = "更新前请保存正在进行的操作。";
        }

        public bool UpdateSucceeded { get; private set; }
        public Exception? UpdateError { get; private set; }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            await RunUpdateAsync();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_isUpdating)
            {
                e.Cancel = true;
                return;
            }

            base.OnClosing(e);
        }

        private async Task RunUpdateAsync()
        {
            _isUpdating = true;
            UpdateError = null;
            UpdateButton.IsEnabled = false;
            DownloadProgress.Visibility = Visibility.Visible;
            DownloadProgress.Value = 0;
            StatusText.Text = "正在下载更新包...";

            try
            {
                var progress = new Progress<float>(value =>
                {
                    double clamped = Math.Max(0, Math.Min(100, value));
                    DownloadProgress.Value = clamped;
                    StatusText.Text = $"正在下载更新包... {clamped:0}%";
                });

                await _updateAction(progress);

                UpdateSucceeded = true;
                StatusText.Text = "下载完成，正在启动覆盖更新。";
                _isUpdating = false;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                UpdateError = ex;
                StatusText.Text = "更新准备失败。请稍后重试，或切换节点后再试。";
                UpdateButton.Content = "重试";
                UpdateButton.IsEnabled = true;
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }
}
