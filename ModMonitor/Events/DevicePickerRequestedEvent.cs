using LibDnaSerial;
using LibDnaSerial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMonitor.Events
{
    public delegate void DevicePickerRequestedEvent(object sender, DevicePickerRequestedEventArgs args);

    public class DevicePickerRequestedEventArgs : EventArgs
    {
        public DevicePickerRequestedEventArgs(Action<DnaDevice> cb)
        {
            Callback = cb;
        }

        public Action<DnaDevice> Callback { get; private set; }
    }
}
