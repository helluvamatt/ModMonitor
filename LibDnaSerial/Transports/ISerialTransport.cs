using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDnaSerial.Transports
{
    internal interface ISerialTransport : IDisposable
    {
        string PortName { get; }

        void WriteLine(string line);

        string ReadLine();
    }
}
