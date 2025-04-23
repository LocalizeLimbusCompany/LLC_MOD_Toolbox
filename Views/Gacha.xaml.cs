using System.Windows.Controls;
using LLC_MOD_Toolbox.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LLC_MOD_Toolbox.Views
{
    /// <summary>
    /// Gacha.xaml 的交互逻辑
    /// </summary>
    public partial class Gacha : UserControl
    {
        public Gacha()
        {
            DataContext = App.Current.Services.GetRequiredService<GachaViewModel>();
            InitializeComponent();
        }
    }
}
