using LibDnaSerial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModMonitor.Models
{
    /// <summary>
    /// Represents a temperature where the value follows the unit, meaning that the input temperature is fixed, but if the unit changes, the value changes to reflect the same temperature in the new unit
    /// 
    /// Conversion is only done if the desired format is different from the input format
    /// </summary>
    class ImmutableTemperature : DependencyObject
    {
        private Temperature wrapped;

        public ImmutableTemperature(Temperature wrapped)
        {
            this.wrapped = wrapped;
            Unit = wrapped.Unit;
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(float), typeof(ImmutableTemperature));

        public float Value
        {
            get
            {
                return (float)GetValue(ValueProperty);
            }
            private set
            {
                SetValue(ValueProperty, value);
            }
        }

        private TemperatureUnit _Unit;
        public TemperatureUnit Unit
        {
            get
            {
                return _Unit;
            }
            set
            {
                _Unit = value;
                Value = wrapped.GetValue(_Unit);
            }
        }
    }
}
