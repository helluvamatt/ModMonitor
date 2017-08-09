using LibDnaSerial;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DnaDeviceMonitor.Converters
{
    [ValueConversion(typeof(Temperature), typeof(string))]
    class TemperatureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = (Temperature)value;
            return string.Format("{0:0,0.0} °{1}", t.Value, t.Unit);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
