using System.Globalization;
using System.Windows.Data;

namespace LLC_MOD_Toolbox.Converters
{
    public class InstallerMultiValueConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            if (values.Length != 2 || values[0] == null || values[1] == null)
                return Binding.DoNothing;

            var nodeInformation = values[0] as Models.NodeInformation;

            if (nodeInformation == null || values[1] is not string path)
                return Binding.DoNothing;

            return (nodeInformation, path);
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
