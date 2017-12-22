using LibDnaSerial.Models;
using System.Collections.Generic;
using System.IO.Ports;

namespace LibDnaSerial
{
    /// <summary>
    /// Provides classes for querying the system for connected DNA devices
    /// </summary>
    public class DnaDeviceManager
    {
        /// <summary>
        /// Checks if the device on a particular serial port responds to DNA device commands by requesting the manufacturer and product names
        /// </summary>
        /// <param name="serialPort">Serial port to query</param>
        /// <returns>Device string or null if unable to query device</returns>
        public static DnaDevice CheckForDnaDevice(string serialPort)
        {
            try
            {
                using (DnaConnection conn = new DnaConnection(serialPort, 500))
                {
                    DnaDevice dev = new DnaDevice();
                    dev.SerialPort = serialPort;
                    dev.Manufacturer = conn.GetManufacturer();
                    dev.ProductName = conn.GetProductName();
                    dev.SerialNumber = conn.GetSerialNumber();
                    dev.FirmwareVersion = conn.GetFirmwareVersion();
                    dev.Features = conn.GetFeatures();
                    dev.CellCount = conn.GetCellCount();
                    dev.MaxPower = GetMaxPower(dev.Manufacturer, dev.ProductName, dev.CellCount);
                    return dev;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Scan for DNA devices, checking all the computer's listed serial ports
        /// </summary>
        /// <returns>List of connected DNA devices</returns>
        /// <see cref="CheckForDnaDevice(string)"/>
        public static List<DnaDevice> ListDnaDevices()
        {
            List<DnaDevice> devices = new List<DnaDevice>();
            foreach (string serialPort in SerialPort.GetPortNames())
            {
                var dev = CheckForDnaDevice(serialPort);
                if (dev != null)
                {
                    devices.Add(dev);
                }
            }
            return devices;
        }

        // Table of devices to max power outputs
        private static readonly Dictionary<string, float> MaxPowerTable = new Dictionary<string, float>()
        {
            { "Evolv_DNA 250_3", 250 },
            { "Evolv_DNA 200_3", 200 },
            { "Evolv_DNA 250_2", 166 },
            { "Evolv_DNA 200_2", 133 },
            { "Evolv_DNA 75_1", 75 },
            { "Evolv_DNA 75 Color_1", 75 },
            { "Evolv_DNA 60_1", 60 },
            { "Evolv_DNA 40_1", 40 },
        };

        private static float? GetMaxPower(string manufacturer, string boardName, uint cellCount)
        {
            var key = string.Format("{0}_{1}_{2}", manufacturer, boardName, cellCount);
            if (MaxPowerTable.ContainsKey(key))
            {
                return MaxPowerTable[key];
            }
            return null;
        }
    }
}
