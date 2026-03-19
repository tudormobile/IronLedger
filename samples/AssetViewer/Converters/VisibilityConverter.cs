using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AssetViewer.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visible = value != null;
            if (value is string stringValue)
            {
                visible = !string.IsNullOrWhiteSpace(stringValue);
            }
            return visible
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
