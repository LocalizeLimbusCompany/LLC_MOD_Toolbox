using System.Windows.Controls;
using LLC_MOD_Toolbox.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LLC_MOD_Toolbox.Views
{
    /// <summary>
    /// AutoInstaller.xaml 的交互逻辑
    /// </summary>
    public partial class AutoInstaller : UserControl
    {
        public AutoInstaller()
        {
            InitializeComponent();
            App.Current.Services.GetService<AutoInstallerViewModel>();
        }
    }
}
