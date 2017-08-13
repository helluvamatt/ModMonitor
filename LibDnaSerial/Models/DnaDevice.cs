using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDnaSerial.Models
{
    /// <summary>
    /// Represents a DNA device
    /// </summary>
    public class DnaDevice
    {
        /// <summary>
        /// Serial port the device is connected to
        /// </summary>
        public string SerialPort { get; set; }

        /// <summary>
        /// Manufacturer name read from device
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Product name read from device
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Serial number read from device
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Firmware version reported by device
        /// </summary>
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// Max power of the device, provided by a table of known devices
        /// </summary>
        public float? MaxPower { get; set; }

        /// <summary>
        /// Features read from device
        /// </summary>
        public List<string> Features { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} ({2}) on {3}", Manufacturer, ProductName, SerialNumber, SerialPort);
        }
    }
}
