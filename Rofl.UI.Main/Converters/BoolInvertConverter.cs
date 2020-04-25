using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Rofl.UI.Main.Converters
{
    // Credit to Qwertie (https://stackoverflow.com/questions/361209/how-to-open-a-wpf-popup-when-another-control-is-clicked-using-xaml-markup-only/8946055#8946055)
    public class BoolInvertConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return !(bool)value;
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
