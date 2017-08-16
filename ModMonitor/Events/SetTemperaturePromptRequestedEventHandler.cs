using LibDnaSerial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMonitor.Events
{
    delegate void SetTemperaturePromptRequestedEventHandler(object sender, SetTemperaturePromptRequestedEventArgs args);

    class SetTemperaturePromptRequestedEventArgs : EventArgs
    {
        public SetTemperaturePromptRequestedEventArgs(Action<Temperature> cb, Temperature currentTemperature)
        {
            Callback = cb;
            CurrentTemperature = currentTemperature;
        }

        public Action<Temperature> Callback { get; private set; }

        public Temperature CurrentTemperature { get; private set; }
    }
}
