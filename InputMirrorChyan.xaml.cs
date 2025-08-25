using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LLC_MOD_Toolbox
{
    public partial class InputMirrorChyan : Window, INotifyPropertyChanged
    {
        private string _promptText;
        private string _inputText;
        private string _inputLabel;
        private bool _isPasswordMode;

        public InputMirrorChyan()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string PromptText
        {
            get => _promptText;
            set
            {
                _promptText = value;
                OnPropertyChanged(nameof(PromptText));
            }
        }

        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        public bool IsPasswordMode
        {
            get => _isPasswordMode;
            set
            {
                _isPasswordMode = value;
                if (value)
                {
                    InputTextBox.Visibility = Visibility.Collapsed;
                    InputPasswordBox.Visibility = Visibility.Visible;
                    InputPasswordBox.Focus();
                }
                else
                {
                    InputTextBox.Visibility = Visibility.Visible;
                    InputPasswordBox.Visibility = Visibility.Collapsed;
                    InputTextBox.Focus();
                }
            }
        }

        public string Result { get; private set; }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = IsPasswordMode ? InputPasswordBox.Password : InputText;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = null;
            DialogResult = false;
            Close();
        }
        private void InformationButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://mirrorchyan.com/");
        }

        public static void OpenUrl(string Url)
        {
            Log.logger.Info("打开了网址：" + Url);
            ProcessStartInfo psi = new(Url)
            {
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        public string InputLabel
        {
            get => _inputLabel;
            set
            {
                _inputLabel = value;
                OnPropertyChanged(nameof(InputLabel));
                OnPropertyChanged(nameof(InputLabelVisibility));
            }
        }

        // 新增：标签可见性
        public Visibility InputLabelVisibility =>
            string.IsNullOrEmpty(InputLabel) ? Visibility.Collapsed : Visibility.Visible;

        protected override void OnSourceInitialized(System.EventArgs e)
        {
            base.OnSourceInitialized(e);

            // 设置焦点到输入框
            if (IsPasswordMode)
                InputPasswordBox.Focus();
            else
                InputTextBox.Focus();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static string ShowDialog(string prompt, string title = "等下，你有Mirror酱秘钥吗？",
            string inputLabel = null, bool isPassword = false, Window owner = null)
        {
            var dialog = new InputMirrorChyan
            {
                Title = title,
                PromptText = prompt,
                InputLabel = inputLabel,
                IsPasswordMode = isPassword,
                Owner = owner
            };

            return dialog.ShowDialog() == true ? dialog.Result : null;
        }
    }
}
