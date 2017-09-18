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
    [ValueConversion(typeof(float), typeof(string))]
    internal class NumberFormatConverter : Freezable, IValueConverter
    {
        #region NumberFormat property

        public string NumberFormat
        {
            get
            {
                return (string)GetValue(NumberFormatProperty);
            }
            set
            {
                SetValue(NumberFormatProperty, value);
            }
        }

        public static readonly DependencyProperty NumberFormatProperty = DependencyProperty.Register("NumberFormat", typeof(string), typeof(NumberFormatConverter));

        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format(NumberFormat, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new NumberFormatConverter();
        }
    }
}
