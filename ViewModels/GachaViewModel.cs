using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.ViewModels
{
    public partial class GachaViewModel : ObservableObject
    {
        private List<PersonalInfo> personalInfos = [];

        [ObservableProperty]
        private ObservableCollection<PersonalInfo> selectedPersonalInfos = [];

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isInitialized;

        private readonly Random random = new();

        private bool CanGacha => !IsLoading;

        [RelayCommand(CanExecute = nameof(CanGacha))]
        public async Task GachaAsync()
        {
            SelectedPersonalInfos.Clear();
            if (!IsInitialized)
            {
                IsLoading = true;

                try
                {
                    // 模拟网络请求
                    await Task.Delay(1000);

                    // 实际网络请求示例
                    // personalInfos = await networkService.GetPersonalInfosAsync();

                    // 临时测试数据
                    personalInfos = [new("123", 0),];

                    IsInitialized = true;
                }
                catch (Exception ex)
                {
                    // 处理异常
                    System.Diagnostics.Debug.WriteLine($"初始化抽卡数据失败: {ex.Message}");
                }
                finally
                {
                    IsLoading = false;
                }
            }

            if (personalInfos.Count > 0)
            {
                SelectedPersonalInfos = [];
                int index = random.Next(personalInfos.Count);
                SelectedPersonalInfos.Add(personalInfos[index]);
            }
        }

        public GachaViewModel()
        {
            // 构造函数中可以注入网络服务依赖
            SelectedPersonalInfos = [];
        }
    }
}
