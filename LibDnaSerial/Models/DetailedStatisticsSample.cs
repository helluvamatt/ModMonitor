using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDnaSerial.Models
{
    /// <summary>
    /// Detailed statistics sample with all values reported by the device
    /// </summary>
    public class DetailedStatisticsSample : BasicStatisticsSample
    {
        /// <summary>
        /// Mean puff energy since last reset
        /// </summary>
        public float MeanEnergy { get; set; }

        /// <summary>
        /// Mean average puff power since last reset
        /// </summary>
        public float MeanPower { get; set; }

        /// <summary>
        /// Mean average puff temperature since last reset
        /// </summary>
        public Temperature MeanTemperature { get; set; }

        /// <summary>
        /// Mean peak temperature since last reset
        /// </summary>
        public Temperature? MeanTemperaturePeak { get; set; }

        /// <summary>
        /// Mean puff time since last reset
        /// </summary>
        public float MeanTime { get; set; }

        /// <summary>
        /// Standard deviation puff energy since last reset
        /// </summary>
        public float StdDevEnergy { get; set; }

        /// <summary>
        /// Standard deviation average puff power since last reset
        /// </summary>
        public float StdDevPower { get; set; }

        /// <summary>
        /// Standard deviation average puff temperature since last reset
        /// </summary>
        public Temperature StdDevTemperature { get; set; }

        /// <summary>
        /// Standard deviation peak temperature since last reset
        /// </summary>
        public Temperature? StdDevTemperaturePeak { get; set; }

        /// <summary>
        /// Standard deviation puff time since last reset
        /// </summary>
        public float StdDevTime { get; set; }

        /// <summary>
        /// Total puff energy since last reset
        /// </summary>
        public float TotalEnergy { get; set; }

        /// <summary>
        /// Total puff time since last reset
        /// </summary>
        public float TotalTime { get; set; }

        /// <summary>
        /// Mean puff energy over life of device
        /// </summary>
        public float DeviceMeanEnergy { get; set; }

        /// <summary>
        /// Mean average puff power over life of device
        /// </summary>
        public float DeviceMeanPower { get; set; }

        /// <summary>
        /// Mean average puff temperature over life of device
        /// </summary>
        public Temperature DeviceMeanTemperature { get; set; }

        /// <summary>
        /// Mean peak puff temperature over life of device
        /// </summary>
        public Temperature? DeviceMeanTemperaturePeak { get; set; }

        /// <summary>
        /// Mean time of puffs over life of device
        /// </summary>
        public float DeviceMeanTime { get; set; }

        /// <summary>
        /// Standard deviation of puff energy over life of device
        /// </summary>
        public float DeviceStdDevEnergy { get; set; }

        /// <summary>
        /// Standard deviation of average puff power over life of device
        /// </summary>
        public float DeviceStdDevPower { get; set; }

        /// <summary>
        /// Standard deviation of average puff temperature over life of device
        /// </summary>
        public Temperature DeviceStdDevTemperature { get; set; }

        /// <summary>
        /// Standard deviation of peak temperature over life of device
        /// </summary>
        public Temperature? DeviceStdDevTemperaturePeak { get; set; }

        /// <summary>
        /// Standard deviation of puff time over life of device
        /// </summary>
        public float DeviceStdDevTime { get; set; }

        /// <summary>
        /// Total puff energy over life of device
        /// </summary>
        public float DeviceTotalEnergy { get; set; }

        /// <summary>
        /// Total puff time over life of device
        /// </summary>
        public float DeviceTotalTime { get; set; }
    }
}
