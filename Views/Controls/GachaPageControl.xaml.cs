using System.Windows;
using System.Windows.Controls;

namespace LLC_MOD_Toolbox.Views.Controls
{
    public partial class GachaPageControl : UserControl
    {
        private readonly Label[] _resultLabels;

        public GachaPageControl()
        {
            InitializeComponent();
            _resultLabels =
            [
                GachaText1, GachaText2, GachaText3, GachaText4, GachaText5,
                GachaText6, GachaText7, GachaText8, GachaText9, GachaText10
            ];
        }

        public void SetButtonHitTestVisible(bool isEnabled)
        {
            InGachaButton.IsHitTestVisible = isEnabled;
        }

        public Label GetResultLabel(int index)
        {
            return _resultLabels[index];
        }

        public void CollapseAllResults()
        {
            foreach (var label in _resultLabels)
            {
                label.Visibility = Visibility.Collapsed;
            }
        }
    }
}
