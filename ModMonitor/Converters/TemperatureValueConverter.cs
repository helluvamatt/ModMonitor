using LibDnaSerial;
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
    [ValueConversion(typeof(Temperature), typeof(float))]
    class TemperatureValueConverter : DependencyObject, IValueConverter
    {
        public TemperatureUnit Unit { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Temperature)value).GetValue(Unit);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Temperature { Value = (float)value, Unit = Unit };
        }
    }
}
