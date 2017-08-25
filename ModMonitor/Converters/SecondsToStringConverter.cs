using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModMonitor.Converters
{
    [ValueConversion(typeof(float), typeof(string))]
    class SecondsToStringConverter : IValueConverter
    {
        const float MILLIS_THRESH = 0.1f;
        const float MILLIS = 1000f;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float val = System.Convert.ToSingle(value);
            if (val < MILLIS_THRESH)
            {
                val = val * MILLIS;
                return string.Format("{0:0.0} ms", val);
            }
            return string.Format("{0:0.000} s", val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
