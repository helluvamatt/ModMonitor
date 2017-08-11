using System;
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
                    dev.Features = conn.GetFeatures();
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
    }
}
