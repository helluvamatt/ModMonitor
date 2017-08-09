using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDnaSerial
{
    /// <summary>
    /// Represents a complete sample of parameters
    /// </summary>
    public class Sample
    {
        /// <summary>
        /// Timestamp for when collection begins
        /// </summary>
        public DateTime Begin { get; set; }

        /// <summary>
        /// Timestamp for when collection ends
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Total battery voltage
        /// </summary>
        /// <see cref="DnaConnection.GetBatteryVoltage"/>
        public float BatteryVoltage { get; set; }

        /// <summary>
        /// Cell voltages
        /// </summary>
        /// <see cref="DnaConnection.GetCellVoltages"/>
        public List<float> CellVoltages { get; set; }

        /// <summary>
        /// Battery capacity in Watt-hours if supported
        /// </summary>
        /// <see cref="DnaConnection.GetBatteryCapacityWh"/>
        public float BatteryCapacity { get; set; }

        /// <summary>
        /// Battery level percentage
        /// </summary>
        /// <see cref="DnaConnection.GetBatteryLevelPercent"/>
        public float BatteryLevel { get; set; }

        /// <summary>
        /// Current while firing
        /// </summary>
        /// <see cref="DnaConnection.GetCurrent"/>
        public float Current { get; set; }

        /// <summary>
        /// Get power in watts while firing
        /// </summary>
        /// <see cref="DnaConnection.GetPower"/>
        public float Power { get; set; }

        /// <summary>
        /// Power setpoint in watts
        /// </summary>
        /// <see cref="DnaConnection.GetPowerSetpoint"/>
        public float PowerSetpoint { get; set; }

        /// <summary>
        /// Cold resistance
        /// </summary>
        /// <see cref="DnaConnection.GetResistance"/>
        public float ColdResistance { get; set; }

        /// <summary>
        /// Live coil resistance while firing
        /// </summary>
        /// <see cref="DnaConnection.GetLiveResistance"/>
        public float LiveResistance { get; set; }

        /// <summary>
        /// Coil temperature for temperature sensing coils while firing
        /// </summary>
        /// <see cref="DnaConnection.GetTemperature"/>
        public Temperature Temperature { get; set; }

        /// <summary>
        /// Board/chip temperature
        /// </summary>
        /// <see cref="DnaConnection.GetBoardTemperature"/>
        public Temperature BoardTemperature { get; set; }

        /// <summary>
        /// Room temperature
        /// </summary>
        /// <see cref="DnaConnection.GetRoomTemperature"/>
        public Temperature RoomTemperature { get; set; }

        /// <summary>
        /// Coil voltage while firing
        /// </summary>
        /// <see cref="DnaConnection.GetVoltage"/>
        public float Voltage { get; set; }

    }
}
