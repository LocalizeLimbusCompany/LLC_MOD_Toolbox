using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LLC_MOD_Toolbox.Views.Controls
{
    public partial class AutoInstallPageControl : UserControl
    {
        private const double KaltsitArtworkHeight = 222;
        private const double KaltsitProgressLineOffset = 10;
        private const double KaltsitProgressVerticalOffset = -40;
        private const double KaltsitProgressLeftInset = -28;
        private const double KaltsitProgressRightInset = -28;
        private const double KaltsitControlBoardContentOffset = -8;
        private const double KaltsitProgressTextGap = 6;

        public static readonly DependencyProperty ProgressPercentageProperty = DependencyProperty.Register(
            nameof(ProgressPercentage),
            typeof(double),
            typeof(AutoInstallPageControl),
            new FrameworkPropertyMetadata(
                0d,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnProgressPercentageChanged,
                CoerceProgressPercentage));

        public AutoInstallPageControl()
        {
            InitializeComponent();
            Loaded += (_, _) => UpdateProgressVisuals();
            FullProgress.SizeChanged += (_, _) => UpdateStandardProgressClip();
            KaltsitInstallProgressHost.SizeChanged += (_, _) => UpdateKaltsitProgress();
        }

        public double ProgressPercentage
        {
            get => (double)GetValue(ProgressPercentageProperty);
            set => SetValue(ProgressPercentageProperty, value);
        }

        public ImageSource? ButtonImageSource
        {
            get => AutoInstallStartButtonIMG.Source;
            set => AutoInstallStartButtonIMG.Source = value;
        }

        public ImageSource? ButtonHoverImageSource
        {
            get => AutoInstallBTHover.Source;
            set => AutoInstallBTHover.Source = value;
        }

        public void ResetHoverOpacity()
        {
            AutoInstallBTHover.Opacity = 0;
        }

        private static object CoerceProgressPercentage(DependencyObject dependencyObject, object baseValue)
        {
            double value = (double)baseValue;
            if (double.IsNaN(value) || value <= 0)
                return 0d;
            if (double.IsPositiveInfinity(value) || value >= 100)
                return 100d;
            return value;
        }

        private static void OnProgressPercentageChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ((AutoInstallPageControl)dependencyObject).UpdateProgressVisuals();
        }

        private void UpdateProgressVisuals()
        {
            UpdateStandardProgressClip();
            UpdateKaltsitProgress();
        }

        private void UpdateStandardProgressClip()
        {
            double width = FullProgress.ActualWidth > 0 ? FullProgress.ActualWidth : FullProgress.Width;
            double height = FullProgress.ActualHeight > 0 ? FullProgress.ActualHeight : FullProgress.Height;
            FullProgress.Clip = new RectangleGeometry(
                new Rect(0, 0, width * ProgressPercentage / 100d, height));
        }

        private void UpdateKaltsitProgress()
        {
            double hostWidth = KaltsitInstallProgressHost.ActualWidth;
            double hostHeight = KaltsitInstallProgressHost.ActualHeight;
            if (hostWidth <= 0 || hostHeight <= 0)
                return;

            double availableWidth = Math.Max(0, hostWidth - KaltsitProgressLeftInset - KaltsitProgressRightInset);
            double lineLength = availableWidth * ProgressPercentage / 200d;
            double leftInnerEdge = KaltsitProgressLeftInset + KaltsitControlBoardContentOffset + lineLength;
            double rightInnerEdge = hostWidth - KaltsitProgressRightInset + KaltsitControlBoardContentOffset - lineLength;
            double lineTop = ((hostHeight - KaltsitArtworkHeight) / 2d)
                + KaltsitArtworkHeight
                + KaltsitProgressLineOffset
                + KaltsitProgressVerticalOffset;

            KaltsitLeftProgressLine.Width = lineLength;
            Canvas.SetLeft(KaltsitLeftProgressLine, KaltsitProgressLeftInset + KaltsitControlBoardContentOffset);
            Canvas.SetTop(KaltsitLeftProgressLine, lineTop);

            KaltsitRightProgressLine.Width = lineLength;
            Canvas.SetLeft(KaltsitRightProgressLine, rightInnerEdge);
            Canvas.SetTop(KaltsitRightProgressLine, lineTop);

            bool isComplete = ProgressPercentage >= 100d;
            int displayPercentage = isComplete ? 100 : (int)Math.Floor(ProgressPercentage);
            string progressText = displayPercentage.ToString(CultureInfo.InvariantCulture) + "%";

            KaltsitLeftProgressText.Visibility = isComplete ? Visibility.Collapsed : Visibility.Visible;
            KaltsitRightProgressText.Visibility = isComplete ? Visibility.Collapsed : Visibility.Visible;
            KaltsitCenterProgressText.Visibility = isComplete ? Visibility.Visible : Visibility.Collapsed;

            if (isComplete)
            {
                KaltsitCenterProgressText.Text = progressText;
                PositionCenteredTextAboveLine(
                    KaltsitCenterProgressText,
                    (hostWidth / 2d) + KaltsitControlBoardContentOffset,
                    lineTop);
                return;
            }

            KaltsitLeftProgressText.Text = progressText;
            KaltsitRightProgressText.Text = progressText;
            PositionLeftEndpointTextAboveLine(KaltsitLeftProgressText, leftInnerEdge, hostWidth, lineTop);
            PositionRightEndpointTextAboveLine(KaltsitRightProgressText, rightInnerEdge, hostWidth, lineTop);
        }

        private static void PositionCenteredTextAboveLine(TextBlock textBlock, double centerX, double lineTop)
        {
            Size textSize = MeasureText(textBlock);
            Canvas.SetLeft(textBlock, centerX - (textSize.Width / 2d));
            Canvas.SetTop(textBlock, GetTextTop(textSize.Height, lineTop));
        }

        private static void PositionLeftEndpointTextAboveLine(TextBlock textBlock, double endpoint, double hostWidth, double lineTop)
        {
            Size textSize = MeasureText(textBlock);
            double left = Math.Clamp(
                endpoint - textSize.Width - KaltsitProgressTextGap,
                0,
                Math.Max(0, hostWidth - textSize.Width));
            Canvas.SetLeft(textBlock, left);
            Canvas.SetTop(textBlock, GetTextTop(textSize.Height, lineTop));
        }

        private static void PositionRightEndpointTextAboveLine(TextBlock textBlock, double endpoint, double hostWidth, double lineTop)
        {
            Size textSize = MeasureText(textBlock);
            double left = Math.Clamp(
                endpoint + KaltsitProgressTextGap,
                0,
                Math.Max(0, hostWidth - textSize.Width));
            Canvas.SetLeft(textBlock, left);
            Canvas.SetTop(textBlock, GetTextTop(textSize.Height, lineTop));
        }

        private static double GetTextTop(double textHeight, double lineTop)
        {
            return Math.Max(0, lineTop - textHeight - KaltsitProgressTextGap);
        }

        private static Size MeasureText(TextBlock textBlock)
        {
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return textBlock.DesiredSize;
        }
    }
}
