using System;
using System.Globalization;
using System.Windows.Data;

namespace PharmacyApp.Converters
{
    /// <summary>
    /// Binds a TabItem's IsSelected (or a RadioButton's IsChecked) to an enum property,
    /// comparing the bound enum value against ConverterParameter (per rulebook Rule 8.3).
    /// Used for MainWindowTabEnum navigation and any future radio-button-driven enum choice.
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter != null)
            {
                return parameter;
            }
            return Binding.DoNothing;
        }
    }
}
