using System.Windows.Controls;
using LLC_MOD_Toolbox.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LLC_MOD_Toolbox.Views
{
    /// <summary>
    /// Link.xaml 的交互逻辑
    /// </summary>
    public partial class Link : UserControl
    {
        public Link()
        {
            DataContext = App.Current.Services.GetRequiredService<LinkViewModel>();
            InitializeComponent();
        }
    }
}
