using LibDnaSerial;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModMonitor.Converters
{
    [ValueConversion(typeof(Temperature), typeof(string))]
    class TemperatureToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is Temperature)
            {
                var t = (Temperature)value;
                return string.Format("{0:0,0.0} °{1}", t.Value, t.Unit);
            }
            else
            {
                return "N/A";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
