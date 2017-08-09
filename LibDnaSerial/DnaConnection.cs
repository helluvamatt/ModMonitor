﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace LibDnaSerial
{
    public class DnaConnection : IDisposable
    {
        public const char CODE_BATTERY = 'B';
        public const char CODE_CAPACITY = 'C';
        public const char CODE_INFO = 'E';
        public const char CODE_FIRE = 'F';
        public const char CODE_CURRENT = 'I';
        public const char CODE_PROFILES = 'M';
        public const char CODE_POWER = 'P';
        public const char CODE_RESISTANCE = 'R';
        public const char CODE_STATISTICS = 'S';
        public const char CODE_TEMPERATURE = 'T';
        public const char CODE_USB = 'U';
        public const char CODE_VOLTAGE = 'V';

        private const string UNIT_OHM = "OHM";
        private const string UNIT_VOLT = "V";
        private const string UNIT_WATTHOUR = "WH";
        private const string UNIT_AMP = "A";

        private const string MESSAGE_PATTERN = @".*?([A-Z])=(.*)";
        private const string TEMP_PATTERN = @"(\d+(?:\.\d+)?)([FCK])";

        private SerialPort serialPort;
        private object lockObject = new { };

        private Regex messageRegEx;
        private Regex tempRegex;

        private uint cellCount = 0;

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="device">Device to connect to</param>
        public DnaConnection(DnaDevice device) : this(device.SerialPort) { }

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="portName">Serial port name</param>
        public DnaConnection(string portName) : this(portName, 30000) { }

        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="portName">Serial port name</param>
        /// <param name="readTimeout">Read timeout (ms)</param>
        public DnaConnection(string portName, int readTimeout)
        {
            serialPort = new SerialPort(portName);
            serialPort.ReadTimeout = readTimeout;
            serialPort.Open();
            messageRegEx = new Regex(MESSAGE_PATTERN);
            tempRegex = new Regex(TEMP_PATTERN);
            cellCount = GetCellCount();
        }

        public void Dispose()
        {
            serialPort.Dispose();
        }

        /// <summary>
        /// Sample all available data points at once
        /// </summary>
        /// <returns></returns>
        public Sample GetSample()
        {
            Sample s = new Sample();
            s.Begin = DateTime.Now;
            s.BatteryVoltage = GetBatteryVoltage();
            s.CellVoltages = GetCellVoltages();
            s.BatteryCapacity = GetBatteryCapacityWh();
            s.BatteryLevel = GetBatteryLevelPercent();
            s.Current = GetCurrent();
            s.Power = GetPower();
            s.PowerSetpoint = GetPowerSetpoint();
            s.ColdResistance = GetResistance();
            s.LiveResistance = GetLiveResistance();
            s.Temperature = GetTemperature();
            s.BoardTemperature = GetBoardTemperature();
            s.RoomTemperature = GetRoomTemperature();
            s.Voltage = GetVoltage();
            s.End = DateTime.Now;
            return s;
        }

        /// <summary>
        /// Get total battery voltage
        /// </summary>
        /// <returns>Voltage</returns>
        public float GetBatteryVoltage()
        {
            SendMessage(new Message(CODE_BATTERY, "GET"));
            var m = ReadMessage();
            return ParseVoltage(m.Argument);
        }

        /// <summary>
        /// Get individual cell voltage
        /// 
        /// Note: 1x 18650 is usually considered one cell. Single configurations like the DNA 75 have only one cell, DNA 250 may have 3. Depends on board/mod model and configuration.
        /// </summary>
        /// <param name="cell">1-based cell index</param>
        /// <returns>Voltage</returns>
        public float GetCellVoltage(uint cell)
        {
            if (cell > cellCount) throw new ArgumentException("Cell index exceeds the number of cells present.");
            SendMessage(new Message(CODE_BATTERY, string.Format("GET CELL {0}", cell)));
            var m = ReadMessage();
            return ParseVoltage(m.Argument);
        }

        /// <summary>
        /// Get voltages for all cells
        /// </summary>
        /// <returns>List of voltage values</returns>
        /// <see cref="GetCellVoltage(uint)"/>
        public List<float> GetCellVoltages()
        {
            List<float> cellVoltages = new List<float>();
            for (uint i = 0; i < cellCount; i++)
            {
                cellVoltages.Add(GetCellVoltage(i + 1));
            }
            return cellVoltages;
        }

        /// <summary>
        /// Get the cell count
        /// </summary>
        /// <returns>Number of individual cells connected to the mod</returns>
        public uint GetCellCount()
        {
            SendMessage(new Message(CODE_BATTERY, "GET CELLS"));
            var m = ReadMessage();
            return uint.Parse(m.Argument);
        }

        /// <summary>
        /// Get the battery capacity in Watt-hours, if supported/configured
        /// 
        /// Support can be tested by checking for "FG" in the features list from GetFeatures
        /// </summary>
        /// <returns>Total battery capacity in watt-hours, null if not supported/configured</returns>
        public float GetBatteryCapacityWh()
        {
            SendMessage(new Message(CODE_CAPACITY, "GET"));
            var m = ReadMessage();
            if (m.Argument == "?") return 0f;
            return float.Parse(TrimUnit(m.Argument, UNIT_WATTHOUR));
        }

        /// <summary>
        /// Get the current battery level percentage
        /// </summary>
        /// <returns>0-100%, as reported by the DNA board</returns>
        public float GetBatteryLevelPercent()
        {
            SendMessage(new Message(CODE_CAPACITY, "GET%"));
            var m = ReadMessage();
            return float.Parse(m.Argument.TrimEnd('%'));
        }

        /// <summary>
        /// Get the manufacturer name of the board, e.g. "Evolv"
        /// </summary>
        /// <returns>Manufacturer name</returns>
        public string GetManufacturer()
        {
            SendMessage(new Message(CODE_INFO, "GET MFR"));
            var m = ReadMessage();
            return m.Argument;
        }

        /// <summary>
        /// Get the product name of the board, e.g. "DNA 75"
        /// </summary>
        /// <returns>Product name</returns>
        public string GetProductName()
        {
            SendMessage(new Message(CODE_INFO, "GET PRODUCT"));
            var m = ReadMessage();
            return m.Argument;
        }

        /// <summary>
        /// Get feature codes
        /// </summary>
        /// <param name="featureNumber">1-based index</param>
        /// <see cref="https://github.com/hobbyquaker/dna-commands"/>
        /// <returns>Feature code returned by the board</returns>
        public string GetFeature(uint featureNumber)
        {
            SendMessage(new Message(CODE_INFO, string.Format("GET FEATURE {0}", featureNumber)));
            var m = ReadMessage();
            if (m.Argument == "?") return null;
            return m.Argument;
        }

        /// <summary>
        /// Get all feature codes
        /// </summary>
        /// <returns>List of feature codes</returns>
        public List<string> GetFeatures()
        {
            List<string> features = new List<string>();

            uint featureNumber = 0;
            do
            {
                featureNumber++;
                string feature = GetFeature(featureNumber);
                if (feature != null) features.Add(feature);
                else break;
            }
            while (true);
            return features;
        }

        /// <summary>
        /// Fire the mod
        /// </summary>
        /// <param name="duration">Duration of fire</param>
        public void Fire(float duration)
        {
            if (duration < 0) throw new ArgumentOutOfRangeException("Duration must be positive");
            SendMessage(new Message(CODE_FIRE, string.Format("{0:0,0.0}S", duration)));
        }

        /// <summary>
        /// Get the current while firing
        /// </summary>
        /// <returns>Current in amps, null if not firing</returns>
        public float GetCurrent()
        {
            SendMessage(new Message(CODE_CURRENT, "GET"));
            var m = ReadMessage();
            if (m.Argument == "?") return 0f;
            return float.Parse(TrimUnit(m.Argument, UNIT_AMP));
        }

        /// <summary>
        /// Get the current profile index
        /// 
        /// For example, the DNA 75 has 8 profiles, 1-8
        /// </summary>
        /// <returns>1-based profile number</returns>
        public uint GetCurrentProfile()
        {
            SendMessage(new Message(CODE_PROFILES, "GET"));
            var m = ReadMessage();
            return uint.Parse(m.Argument);
        }

        /// <summary>
        /// Set the current profile
        /// </summary>
        /// <param name="profileNumber">1-based profile number</param>
        public void SetCurrentProfile(uint profileNumber)
        {
            SendMessage(new Message(CODE_PROFILES, profileNumber.ToString()));
        }

        /// <summary>
        /// Get the current power level (only useful while firing)
        /// </summary>
        /// <returns>Get the current firing power, null if not firing</returns>
        public float GetPower()
        {
            SendMessage(new Message(CODE_POWER, "GET"));
            var m = ReadMessage();
            if (m.Argument == "?") return 0f;
            return float.Parse(TrimUnit(m.Argument, "W"));
        }

        /// <summary>
        /// Set the device to the given wattage.
        /// 
        /// Note: the value is passed through directly, but the device will clamp if it's out of supported range
        /// </summary>
        /// <param name="watts">Power in watts to set</param>
        public void SetPower(float watts)
        {
            SendMessage(new Message(CODE_POWER, string.Format("{0}W", watts)));
        }

        /// <summary>
        /// Get the current power set point
        /// </summary>
        /// <returns>The power set point in Watts</returns>
        public float GetPowerSetpoint()
        {
            SendMessage(new Message(CODE_POWER, "GET SP"));
            var m = ReadMessage();
            return float.Parse(TrimUnit(m.Argument, "W"));
        }

        /// <summary>
        /// Get the cold resistance for the current coil
        /// </summary>
        /// <returns>Resistance in ohms</returns>
        public float GetResistance()
        {
            SendMessage(new Message(CODE_RESISTANCE, "GET"));
            var m = ReadMessage();
            return float.Parse(TrimUnit(m.Argument, UNIT_OHM));
        }

        /// <summary>
        /// Get the current live resistance while firing
        /// </summary>
        /// <returns>Live resistance in ohms, null while not firing</returns>
        public float GetLiveResistance()
        {
            SendMessage(new Message(CODE_RESISTANCE, "GET LIVE"));
            var m = ReadMessage();
            if (m.Argument == "?") return 0f;
            return float.Parse(TrimUnit(m.Argument, UNIT_OHM));
        }

        /// <summary>
        /// Get the current live temperature of a temperature sensing coil
        /// </summary>
        /// <returns>Temperature model, null if not firing</returns>
        public Temperature GetTemperature()
        {
            SendMessage(new Message(CODE_TEMPERATURE, "GET"));
            var m = ReadMessage();
            if (m.Argument == "?") return new Temperature { Unit = TemperatureUnit.C, Value = 0f };
            return ParseTemperature(m.Argument);
        }

        /// <summary>
        /// Set the temperature limit for temperature sensing coils, actual operation depends on the selected profile settings
        /// </summary>
        /// <param name="temperature">Temperature model, conversion from Kelvin is supported</param>
        public void SetTemperature(Temperature temperature)
        {
            if (temperature.Unit == TemperatureUnit.K)
            {
                // DNA does not support Kelvin, easy conversion
                temperature.Unit = TemperatureUnit.C;
                temperature.Value = temperature.Value - 273.15f;
            }

            SendMessage(new Message(CODE_TEMPERATURE, string.Format("{0:0,0.000}{1}", temperature.Value, temperature.Unit)));
        }

        /// <summary>
        /// Get the board/chip temperature
        /// </summary>
        /// <returns>Temperature model</returns>
        public Temperature GetBoardTemperature()
        {
            SendMessage(new Message(CODE_TEMPERATURE, "GET BOARD"));
            var m = ReadMessage();
            if (m.Argument == "?") return new Temperature { Unit = TemperatureUnit.C, Value = 0f };
            return ParseTemperature(m.Argument);
        }

        /// <summary>
        /// Get the room/ambient temperature
        /// </summary>
        /// <returns>Temperature model</returns>
        public Temperature GetRoomTemperature()
        {
            SendMessage(new Message(CODE_TEMPERATURE, "GET ROOM"));
            var m = ReadMessage();
            if (m.Argument == "?") return new Temperature { Unit = TemperatureUnit.C, Value = 0f };
            return ParseTemperature(m.Argument);
        }

        /// <summary>
        /// Get the live voltage while firing
        /// </summary>
        /// <returns>Voltage, null if not firing</returns>
        public float GetVoltage()
        {
            SendMessage(new Message(CODE_VOLTAGE, "GET"));
            var m = ReadMessage();
            if (m.Argument == "?") return 0f;
            return ParseVoltage(m.Argument);
        }

        private void SendMessage(Message message)
        {
            serialPort.WriteLine(message.ToString());
        }

        private Message ReadMessage()
        {
            string line = serialPort.ReadLine();
            Match m = messageRegEx.Match(line);
            if (!m.Success) throw new Exception(string.Format("Invalid line read from {0}: \"{1}\"", serialPort.PortName, line));
            return new Message(m.Groups[1].Value[0], m.Groups[2].Value.TrimEnd());
        }

        private Temperature ParseTemperature(string argument)
        {
            Match match = tempRegex.Match(argument);
            if (!match.Success) throw new ArgumentException("Argument is not a valid temperature string.");
            Temperature t = new Temperature();
            t.Value = float.Parse(match.Groups[1].Value);
            TemperatureUnit unit;
            if (!Enum.TryParse(match.Groups[2].Value, out unit)) throw new ArgumentException("Unit is not one of K, C, or F");
            t.Unit = unit;
            return t;
        }

        private float ParseVoltage(string argument)
        {
            return float.Parse(TrimUnit(argument, UNIT_VOLT));
        }

        private string TrimUnit(string argument, string unit)
        {
            return argument.Substring(0, argument.LastIndexOf(unit));
        }
    }
}