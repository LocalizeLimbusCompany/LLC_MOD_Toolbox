using System.Windows;

namespace LLC_MOD_Toolbox.Services.UI
{
    public sealed class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "提示")
        {
            UniversalDialog.ShowMessage(message, title, null, null);
        }

        public bool ShowConfirm(string message, string title = "确认")
        {
            return UniversalDialog.ShowConfirm(message, title);
        }

        public DialogInputResult ShowInput(string message, string title, string inputLabel, InputType inputType, List<DialogButton>? buttons = null)
        {
            var result = UniversalDialog.ShowInput(message, title, inputLabel, inputType, buttons, null);
            return new DialogInputResult
            {
                IsSuccess = result.IsSuccess,
                IsCanceled = result.IsCanceled,
                Input = result.Input
            };
        }

        public string? ShowOpenFileDialog(string title, string filter, string? initialDirectory = null)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = title,
                Filter = filter,
                Multiselect = false,
                InitialDirectory = initialDirectory ?? string.Empty
            };
            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            return null;
        }
    }
}
