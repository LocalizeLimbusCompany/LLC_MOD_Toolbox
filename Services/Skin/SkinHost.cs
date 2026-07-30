using System.Windows;

namespace LLC_MOD_Toolbox.Services.Skin
{
    public static class SkinHost
    {
        public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached(
            "Name",
            typeof(string),
            typeof(SkinHost),
            new FrameworkPropertyMetadata(string.Empty));

        public static void SetName(DependencyObject element, string value) => element.SetValue(NameProperty, value);

        public static string GetName(DependencyObject element) => (string)element.GetValue(NameProperty);
    }
}
