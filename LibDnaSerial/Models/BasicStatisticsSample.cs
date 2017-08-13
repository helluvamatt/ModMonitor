using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDnaSerial.Models
{
    /// <summary>
    /// Sample of statistics values for the last puff, puff counts, and device reset count
    /// </summary>
    public class BasicStatisticsSample : LastPuffStatisticsSample
    {
        /// <summary>
        /// Number of device resets over the lifetime of the device
        /// </summary>
        public int Resets { get; set; }

        /// <summary>
        /// Total puffs since last reset
        /// </summary>
        public int Puffs { get; set; }

        /// <summary>
        /// Total temperature protected puffs since last reset
        /// </summary>
        public int TemperatureProtectedPuffs { get; set; }

        /// <summary>
        /// Total puffs over life of device
        /// </summary>
        public int DevicePuffs { get; set; }

        /// <summary>
        /// Total temperature protected puffs over life of device
        /// </summary>
        public int DeviceTemperatureProtectedPuffs { get; set; }
    }
}
