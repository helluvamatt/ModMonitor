using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDnaSerial.Models
{
    /// <summary>
    /// Represents a complete sample of parameters
    /// </summary>
    public class Sample
    {
        /// <summary>
        /// Sample index for a particular connection
        /// </summary>
        public ulong Index { get; set; }

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
        /// Temperature setpoint
        /// </summary>
        /// <see cref="DnaConnection.GetTemperatureSetpoint"/>
        public Temperature TemperatureSetpoint { get; set; }

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

        /// <summary>
        /// Which buttons are currently being pressed
        /// </summary>
        /// <see cref="DnaConnection.GetButtons"/>
        public Buttons Buttons { get; set; }

        /// <summary>
        /// Format the line as CSV
        /// </summary>
        /// <returns></returns>
        public string ToCsv()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}{15},{16}{17},{18}{19},{20}{21},{22}",
                Index,
                Begin,
                End,
                BatteryVoltage,
                CellVoltages.Count > 0 ? CellVoltages[0] : 0f,
                CellVoltages.Count > 1 ? CellVoltages[1] : 0f,
                CellVoltages.Count > 2 ? CellVoltages[2] : 0f,
                BatteryCapacity,
                BatteryLevel,
                Current,
                Power,
                PowerSetpoint,
                ColdResistance,
                LiveResistance,
                Temperature.Value,
                Temperature.Unit,
                TemperatureSetpoint.Value,
                TemperatureSetpoint.Unit,
                BoardTemperature.Value,
                BoardTemperature.Unit,
                RoomTemperature.Value,
                RoomTemperature.Unit,
                Voltage
            );
        }

        /// <summary>
        /// CSV header showing order of fields
        /// </summary>
        public const string CSV_HEADER = "Index,Begin,End,BatteryVoltage,Cell1Voltage,Cell2Voltage,Cell3Voltage,BatteryCapacity,BatteryLevel,Current,Power,PowerSetpoint,ColdResistance,LiveResistance,Temperature,TemperatureSetpoint,BoardTemperature,RoomTemperature,Voltage";
    }
}
