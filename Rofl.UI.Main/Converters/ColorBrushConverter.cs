using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Rofl.UI.Main.Converters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class ColorBrushConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value is Brush)
            {
                return value;
            }
            else if (value is Color)
            {
                return new SolidColorBrush((Color)value);
            }
            else
            {
                throw new ArgumentException("Cannot convert non-color object", nameof(value));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value is Color)
            {
                return value;
            }
            else if (value is Brush)
            {
                return ((SolidColorBrush)value).Color;
            }
            else
            {
                throw new ArgumentException("Cannot convert non-brush object", nameof(value));
            }
        }
    }
}
