using System.Windows.Controls;
using System.Windows.Media;

namespace LLC_MOD_Toolbox.Views.Controls
{
    public partial class AutoInstallPageControl : UserControl
    {
        public AutoInstallPageControl()
        {
            InitializeComponent();
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

        public Geometry? ProgressClip
        {
            get => FullProgress.Clip;
            set => FullProgress.Clip = value;
        }

        public void ResetHoverOpacity()
        {
            AutoInstallBTHover.Opacity = 0;
        }
    }
}
