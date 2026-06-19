using System.Diagnostics;
using System.Windows;

namespace LLC_MOD_Toolbox.Views
{
    public partial class AgreementDialog : Window
    {
        private const string UserAgreementUrl = "https://www.zeroasso.top/blog/2026/4/11/";
        private const string PrivacyAgreementUrl = "https://www.zeroasso.top/blog/2026/4/11/2";

        public AgreementDialog()
        {
            InitializeComponent();
        }

        public bool IsAccepted { get; private set; }

        private void AgreementCheckChanged(object sender, RoutedEventArgs e)
        {
            AgreeButton.IsEnabled = UserAgreementCheckBox.IsChecked == true &&
                                    PrivacyAgreementCheckBox.IsChecked == true;
        }

        private void AgreeClick(object sender, RoutedEventArgs e)
        {
            if (!AgreeButton.IsEnabled)
                return;

            IsAccepted = true;
            DialogResult = true;
            Close();
        }

        private void RejectClick(object sender, RoutedEventArgs e)
        {
            IsAccepted = false;
            DialogResult = false;
            Close();
        }

        private void OpenUserAgreementClick(object sender, RoutedEventArgs e)
        {
            OpenUrl(UserAgreementUrl);
        }

        private void OpenPrivacyAgreementClick(object sender, RoutedEventArgs e)
        {
            OpenUrl(PrivacyAgreementUrl);
        }

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Log.logger.Warn($"打开协议链接失败：{url}", ex);
            }
        }
    }
}
