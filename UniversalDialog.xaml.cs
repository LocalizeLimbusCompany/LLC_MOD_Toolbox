using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LLC_MOD_Toolbox
{
    public partial class UniversalDialog : Window, INotifyPropertyChanged
    {
        private string _dialogTitle = "提示";
        private string _messageText;
        private string _inputText;
        private string _inputLabel;
        private InputType _inputType = InputType.None;
        private List<DialogButton> _buttons = [];

        public UniversalDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region 属性

        public string DialogTitle
        {
            get => _dialogTitle;
            set
            {
                _dialogTitle = value;
                OnPropertyChanged(nameof(DialogTitle));
            }
        }

        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                OnPropertyChanged(nameof(MessageText));
                OnPropertyChanged(nameof(MessageVisibility));
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

        public InputType InputType
        {
            get => _inputType;
            set
            {
                _inputType = value;
                OnPropertyChanged(nameof(InputVisibility));
                OnPropertyChanged(nameof(TextBoxVisibility));
                OnPropertyChanged(nameof(PasswordBoxVisibility));
                OnPropertyChanged(nameof(MultilineTextBoxVisibility));
            }
        }

        public List<DialogButton> Buttons
        {
            get => _buttons;
            set
            {
                _buttons = value ?? [];
                CreateButtons();
            }
        }

        // 可见性属性
        public Visibility MessageVisibility =>
            string.IsNullOrEmpty(MessageText) ? Visibility.Collapsed : Visibility.Visible;

        public Visibility InputVisibility =>
            InputType == InputType.None ? Visibility.Collapsed : Visibility.Visible;

        public Visibility InputLabelVisibility =>
            string.IsNullOrEmpty(InputLabel) ? Visibility.Collapsed : Visibility.Visible;

        public Visibility TextBoxVisibility =>
            InputType == InputType.Text ? Visibility.Visible : Visibility.Collapsed;

        public Visibility PasswordBoxVisibility =>
            InputType == InputType.Password ? Visibility.Visible : Visibility.Collapsed;

        public Visibility MultilineTextBoxVisibility =>
            InputType == InputType.Multiline ? Visibility.Visible : Visibility.Collapsed;

        #endregion

        #region 结果属性

        public string InputResult { get; private set; }
        public DialogButton ClickedButton { get; private set; }

        #endregion

        #region 事件处理

        private void InputPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // 密码框内容变化处理
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var dialogButton = button?.Tag as DialogButton;

            if (dialogButton != null)
            {
                ClickedButton = dialogButton;

                // 获取输入结果
                switch (InputType)
                {
                    case InputType.Text:
                    case InputType.Multiline:
                        InputResult = InputText;
                        break;
                    case InputType.Password:
                        InputResult = InputPasswordBox.Password;
                        break;
                    default:
                        InputResult = null;
                        break;
                }

                // 执行按钮的自定义操作
                if (dialogButton.Action != null)
                {
                    var continueClose = dialogButton.Action.Invoke(InputResult);
                    if (!continueClose)
                        return; // 不关闭对话框
                }

                DialogResult = dialogButton.IsDefault;
                Close();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // 设置焦点到输入框
            switch (InputType)
            {
                case InputType.Text:
                    InputTextBox.Focus();
                    break;
                case InputType.Password:
                    InputPasswordBox.Focus();
                    break;
                case InputType.Multiline:
                    MultilineTextBox.Focus();
                    break;
            }
        }

        #endregion

        #region 私有方法

        private void CreateButtons()
        {
            ButtonPanel.Children.Clear();

            if (!Buttons.Any())
            {
                // 默认按钮
                Buttons =
                [
                    new DialogButton("确定", true, true),
                    new DialogButton("取消", false, false)
                ];
            }

            for (int i = 0; i < Buttons.Count; i++)
            {
                var dialogButton = Buttons[i];
                var button = new Button
                {
                    Content = dialogButton.Text,
                    Width = 75,
                    Height = 25,
                    Margin = new Thickness(i == 0 ? 0 : 10, 0, 0, 0),
                    IsDefault = dialogButton.IsDefault,
                    IsCancel = dialogButton.IsCancel,
                    Tag = dialogButton
                };

                button.Click += Button_Click;
                ButtonPanel.Children.Add(button);
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// 显示消息对话框
        /// </summary>
        public static DialogResult ShowMessage(string message, string title = "提示",
            List<DialogButton>? buttons = null, Window owner = null)
        {
            if (buttons == null)
            {
                buttons =
                [
                    new DialogButton("确认", true, false)
                ];
            }
            var dialog = new UniversalDialog
            {
                DialogTitle = title,
                MessageText = message,
                InputType = InputType.None,
                Buttons = buttons,
                Owner = owner
            };

            dialog.ShowDialog();
            return new DialogResult(dialog.ClickedButton, dialog.InputResult);
        }

        /// <summary>
        /// 显示输入对话框
        /// </summary>
        public static DialogResult ShowInput(string message, string title = "输入",
            string inputLabel = null, InputType inputType = InputType.Text,
            List<DialogButton> buttons = null, Window owner = null)
        {
            var dialog = new UniversalDialog
            {
                DialogTitle = title,
                MessageText = message,
                InputLabel = inputLabel,
                InputType = inputType,
                Buttons = buttons,
                Owner = owner
            };

            dialog.ShowDialog();
            return new DialogResult(dialog.ClickedButton, dialog.InputResult);
        }

        /// <summary>
        /// 显示确认对话框
        /// </summary>
        public static bool ShowConfirm(string message, string title = "确认", Window owner = null)
        {
            var buttons = new List<DialogButton>
            {
                new DialogButton("是", true, false),
                new DialogButton("否", false, true)
            };

            var result = ShowMessage(message, title, buttons, owner);
            return result.Button?.IsDefault == true;
        }

        /// <summary>
        /// 显示是/否/取消对话框
        /// </summary>
        public static DialogResult ShowYesNoCancel(string message, string title = "选择", Window owner = null)
        {
            var buttons = new List<DialogButton>
            {
                new DialogButton("是", true, false),
                new DialogButton("否", false, false),
                new DialogButton("取消", false, true)
            };

            return ShowMessage(message, title, buttons, owner);
        }

        #endregion
    }

    #region 辅助类

    /// <summary>
    /// 输入框类型
    /// </summary>
    public enum InputType
    {
        None,       // 无输入框
        Text,       // 单行文本
        Password,   // 密码
        Multiline   // 多行文本
    }

    /// <summary>
    /// 对话框按钮
    /// </summary>
    public class DialogButton
    {
        public DialogButton(string text, bool isDefault = false, bool isCancel = false, Func<string, bool> action = null)
        {
            Text = text;
            IsDefault = isDefault;
            IsCancel = isCancel;
            Action = action;
        }

        public string Text { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCancel { get; set; }

        /// <summary>
        /// 按钮点击时的自定义操作，返回true继续关闭对话框，返回false阻止关闭
        /// </summary>
        public Func<string, bool> Action { get; set; }
    }

    /// <summary>
    /// 对话框结果
    /// </summary>
    public class DialogResult
    {
        public DialogResult(DialogButton button, string input)
        {
            Button = button;
            Input = input;
        }

        public DialogButton Button { get; }
        public string Input { get; }

        public bool IsSuccess => Button?.IsDefault == true;
        public bool IsCanceled => Button?.IsCancel == true;
    }

    #endregion
}
