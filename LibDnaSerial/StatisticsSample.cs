using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDnaSerial
{
    /// <summary>
    /// Sample of all statistics values
    /// </summary>
    public class StatisticsSample
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
        /// Number of device resets over the lifetime of the device
        /// </summary>
        public int Resets { get; set; }

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
        public Temperature? LastTemperaturePeak { get; set; }

        /// <summary>
        /// Last puff time in seconds
        /// </summary>
        public float LastTime { get; set; }

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
