using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDnaSerial
{
    /// <summary>
    /// Unit for a temperature reading
    /// </summary>
    public enum TemperatureUnit
    {
        /// <summary>
        /// Kelvin
        /// </summary>
        K,

        /// <summary>
        /// Celsius
        /// </summary>
        C,

        /// <summary>
        /// Fahrenheit
        /// </summary>
        F
    }

    /// <summary>
    /// Buttons values are sent/received as a bitmask
    /// </summary>
    [Flags]
    public enum Buttons
    {
        /// <summary>
        /// No buttons pressed
        /// </summary>
        None = 0,

        /// <summary>
        /// Fire button is pressed
        /// </summary>
        Fire = 1,

        /// <summary>
        /// Down button is pressed
        /// </summary>
        Down = 2,

        /// <summary>
        /// Up button is pressed
        /// </summary>
        Up = 4,

        /// <summary>
        /// Select button is pressed (DNA 75C only)
        /// </summary>
        Select = 8,
    }
}
