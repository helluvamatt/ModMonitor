using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDnaSerial.Transports
{
    internal class SerialPortTransport : ISerialTransport
    {
        private SerialPort serialPort;

        public SerialPortTransport(string portName, int readTimeout)
        {
            serialPort = new SerialPort(portName);
            serialPort.ReadTimeout = readTimeout;
            serialPort.Open();
        }

        ~SerialPortTransport()
        {
            Dispose(false);
        }

        public string PortName
        {
            get
            {
                return serialPort != null ? serialPort.PortName : null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (serialPort != null) serialPort.Dispose();
        }

        public string ReadLine()
        {
            return serialPort.ReadLine();
        }

        public void WriteLine(string line)
        {
            serialPort.WriteLine(line);
        }
    }
}
