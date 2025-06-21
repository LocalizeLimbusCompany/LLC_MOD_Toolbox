using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LLC_MOD_Toolbox.ViewModels;

internal partial class GrayTestViewModel : ObservableObject
{
    [ObservableProperty]
    bool isGrayTestValid;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CheckGrayTestCommand))]
    string? token;

    [RelayCommand(CanExecute = nameof(CanExecuteCheckGrayTest))]
    private void CheckGrayTest()
    {
        IsGrayTestValid = true;
    }

    private bool CanExecuteCheckGrayTest() => !string.IsNullOrWhiteSpace(Token);
}
