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
        // 模拟灰机测试逻辑
        IsGrayTestValid = true; // 假设测试通过
        // 实际逻辑可以根据需要进行修改
        // 例如调用某个服务或执行某个操作来验证灰机状态
    }

    private bool CanExecuteCheckGrayTest() => !string.IsNullOrWhiteSpace(Token);
}
