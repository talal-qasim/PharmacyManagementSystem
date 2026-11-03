using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PharmacyApp.Converters
{
    /// <summary>
    /// Converts bool to Visibility. Pass ConverterParameter="Inverse" to flip the mapping
    /// (e.g. hiding an "empty state" panel when a list has items).
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = value is bool b && b;
            bool inverse = string.Equals(parameter as string, "Inverse", StringComparison.OrdinalIgnoreCase);

            if (inverse) boolValue = !boolValue;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool inverse = string.Equals(parameter as string, "Inverse", StringComparison.OrdinalIgnoreCase);
            bool isVisible = value is Visibility v && v == Visibility.Visible;
            return inverse ? !isVisible : isVisible;
        }
    }
}
