using System.Windows.Controls;
using System.Windows.Media;

namespace LLC_MOD_Toolbox.Views.Controls
{
    public partial class EasterEggPageControl : UserControl
    {
        public EasterEggPageControl()
        {
            InitializeComponent();
        }

        public ImageSource? PageImageSource
        {
            get => EEPageImage.Source;
            set => EEPageImage.Source = value;
        }
    }
}
