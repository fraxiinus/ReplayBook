using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Rofl.UI.Main.Converters
{
    public class FormatKbSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = System.Convert.ToInt64(value, culture);
            var sb = new StringBuilder(32);
            NativeMethods.StrFormatByteSizeW(number, sb, sb.Capacity);
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
