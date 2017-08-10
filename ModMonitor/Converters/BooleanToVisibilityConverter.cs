using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ModMonitor.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class BooleanToVisibilityConverter : IValueConverter
    {
        public bool Negate { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)(value) ^ Negate ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)(value) == Visibility.Visible ^ Negate;
        }
    }
}
