namespace LLC_MOD_Toolbox.Services.UI
{
    public class DialogButton
    {
        public DialogButton(string text, bool isDefault = false, bool isCancel = false, Func<string?, bool>? action = null)
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
        public Func<string?, bool>? Action { get; set; }
    }
}
