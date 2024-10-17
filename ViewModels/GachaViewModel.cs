using CommunityToolkit.Mvvm.ComponentModel;
using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.ViewModels
{
    partial class GachaViewModel : ObservableObject
    {
        [ObservableProperty]
        static List<PersonalInfo> personalInfos = [];

        GachaViewModel()
        {
            // 不是很必要，暂时用来解决警告
            personalInfos.Clear();
        }
    }
}
