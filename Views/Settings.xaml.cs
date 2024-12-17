using System.Windows;
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
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current is LLC_MOD_Toolbox.App app)
            {
                DataContext = app.Services.GetRequiredService<SettingsViewModel>();
            }
        }
    }
}
