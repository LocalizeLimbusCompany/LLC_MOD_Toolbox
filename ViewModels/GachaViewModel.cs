using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.ViewModels
{
    public partial class GachaViewModel : ObservableObject
    {
        [ObservableProperty]
        private static List<PersonalInfo> personalInfos = [];

        [ObservableProperty]
        private static List<PersonalInfo> selectedPersonalInfos = [];

        [RelayCommand]
        public static void Gacha()
        {
            selectedPersonalInfos.Clear();
            throw new NotImplementedException();
        }

        public GachaViewModel()
        {
            // 不是很必要，暂时用来解决警告
            personalInfos.Clear();
        }
    }
}
