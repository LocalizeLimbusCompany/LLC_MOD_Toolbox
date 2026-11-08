namespace LLC_MOD_Toolbox.Services.UI
{
    public class DialogResult
    {
        public DialogResult(DialogButton? button, string? input)
        {
            Button = button;
            Input = input;
        }

        public DialogButton? Button { get; }
        public string? Input { get; }

        public bool IsSuccess => Button?.IsDefault == true;
        public bool IsCanceled => Button?.IsCancel == true;
    }
}
