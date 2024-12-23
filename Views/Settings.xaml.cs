using System.Windows.Controls;
using LLC_MOD_Toolbox.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LLC_MOD_Toolbox.Views
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<SettingsViewModel>();
        }
    }
}
