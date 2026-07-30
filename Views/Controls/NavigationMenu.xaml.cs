using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LLC_MOD_Toolbox.Views.Controls
{
    public partial class NavigationMenu : UserControl
    {
#if DEBUG
        private bool _areDebugBoundsVisible;
#endif

        public NavigationMenu()
        {
            InitializeComponent();
        }

        public void SetDebugBoundsVisible(bool visible)
        {
#if DEBUG
            _areDebugBoundsVisible = visible;
            DebugBoundsCanvas.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            if (visible)
                _ = Dispatcher.BeginInvoke(RefreshDebugBounds, DispatcherPriority.Loaded);
            else
                DebugBoundsCanvas.Children.Clear();
#endif
        }

        public void RefreshDebugBounds()
        {
#if DEBUG
            if (!_areDebugBoundsVisible || DebugBoundsCanvas.Visibility != Visibility.Visible)
                return;

            UpdateLayout();
            DebugBoundsCanvas.Children.Clear();

            FrameworkElement[] buttons =
            [
                InstallOption, LinkOption, GreytestOption,
                SettingOption, AboutOption, EEOption
            ];
            FrameworkElement[] hovers =
            [
                InstallHover, LinkHover, GreytestHover,
                SettingHover, AboutHover, EEHover
            ];

            foreach (FrameworkElement button in buttons)
                AddDebugBounds(button, Brushes.Red, dashed: false);
            foreach (FrameworkElement hover in hovers)
                AddDebugBounds(hover, Brushes.Lime, dashed: true);
#endif
        }

#if DEBUG
        private void AddDebugBounds(FrameworkElement target, Brush brush, bool dashed)
        {
            if (target.Visibility != Visibility.Visible || target.ActualWidth <= 0 || target.ActualHeight <= 0)
                return;

            Point position = target.TranslatePoint(new Point(0, 0), NavigationRoot);
            var rectangle = new Rectangle
            {
                Width = target.ActualWidth,
                Height = target.ActualHeight,
                Stroke = brush,
                StrokeThickness = 1.5,
                Fill = Brushes.Transparent,
                StrokeDashArray = dashed ? new DoubleCollection([4, 2]) : null
            };
            Canvas.SetLeft(rectangle, position.X);
            Canvas.SetTop(rectangle, position.Y);
            DebugBoundsCanvas.Children.Add(rectangle);

            var label = new TextBlock
            {
                Text = target.Name,
                FontSize = 9,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(190, 20, 20, 20)),
                Padding = new Thickness(2, 0, 2, 0)
            };
            Canvas.SetLeft(label, position.X);
            Canvas.SetTop(label, Math.Max(0, position.Y - 13));
            DebugBoundsCanvas.Children.Add(label);
        }
#endif
    }
}
