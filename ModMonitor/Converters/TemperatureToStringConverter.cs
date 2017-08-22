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
    [ValueConversion(typeof(Temperature), typeof(string))]
    class TemperatureToStringConverter : Freezable, IValueConverter
    {
        public TemperatureUnit Unit
        {
            get
            {
                return (TemperatureUnit)GetValue(UnitProperty);
            }
            set
            {
                SetValue(UnitProperty, value);
            }
        }

        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(TemperatureUnit), typeof(TemperatureToStringConverter));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Temperature t = value as Temperature;
            if (t != null)
            {
                if (t.Unit != Unit)
                {
                    var newT = new Temperature { Unit = Unit, Value = t.GetValue(Unit) };
                    return newT.ToString();
                }
                else
                {
                    return t.ToString();
                }
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

        protected override Freezable CreateInstanceCore()
        {
            return new TemperatureToStringConverter();
        }
    }
}
