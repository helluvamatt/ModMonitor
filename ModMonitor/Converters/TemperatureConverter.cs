using LibDnaSerial;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModMonitor.Converters
{
    [ValueConversion(typeof(Temperature), typeof(double))]
    class TemperatureConverter : DependencyObject, IValueConverter
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

        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(TemperatureUnit), typeof(TemperatureConverter));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is Temperature)
            {
                var t = (Temperature)value;
                return t.GetValue(Unit);
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Temperature { Unit = Unit, Value = (float)value }; // Not used
        }
    }
}
