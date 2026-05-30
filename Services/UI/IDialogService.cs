namespace LLC_MOD_Toolbox.Services.UI
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "提示");
        bool ShowConfirm(string message, string title = "确认");
        DialogInputResult ShowInput(string message, string title, string inputLabel, InputType inputType, List<DialogButton>? buttons = null);
        string? ShowOpenFileDialog(string title, string filter, string? initialDirectory = null);
    }

    public sealed class DialogInputResult
    {
        public bool IsSuccess { get; init; }
        public bool IsCanceled { get; init; }
        public string? Input { get; init; }
    }
}
