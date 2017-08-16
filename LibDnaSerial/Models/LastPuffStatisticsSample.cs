using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDnaSerial.Models
{
    /// <summary>
    /// Statistics of just the last puff
    /// </summary>
    public class LastPuffStatisticsSample
    {
        /// <summary>
        /// Sample index for a particular connection
        /// </summary>
        public ulong Index { get; set; }

        /// <summary>
        /// Sample start timestamp
        /// </summary>
        public DateTime Begin { get; set; }

        /// <summary>
        /// Sample end timestamp
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Last energy in mWh
        /// </summary>
        public float LastEnergy { get; set; }

        /// <summary>
        /// Last average power
        /// </summary>
        public float LastPower { get; set; }

        /// <summary>
        /// Last average temperature
        /// </summary>
        public Temperature LastTemperature { get; set; }

        /// <summary>
        /// Last peak temperature
        /// </summary>
        public Temperature LastTemperaturePeak { get; set; }

        /// <summary>
        /// Last puff time in seconds
        /// </summary>
        public float LastTime { get; set; }
    }
}
