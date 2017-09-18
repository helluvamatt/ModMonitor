using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ModMonitor.Converters
{
    [ValueConversion(typeof(Brush), typeof(Brush))]
    internal class ColorDarkenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var original = value as Brush;
            var clone = original.Clone();
            clone.Opacity = 0.15;
            return clone;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not used
            return value;
        }
    }
}
