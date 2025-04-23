using System.Windows;

namespace LLC_MOD_Toolbox.Services;

public class DialogDisplayService : IDialogDisplayService
{
    public void ShowError(string message)
    {
        MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public bool Confirm(string message)
    {
        return MessageBox.Show(message, "确认", MessageBoxButton.YesNo, MessageBoxImage.Question)
            == MessageBoxResult.Yes;
    }
}
