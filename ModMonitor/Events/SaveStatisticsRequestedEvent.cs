using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMonitor.Events
{
    delegate void SaveStatisticsRequestedEvent(object sender, SaveStatisticsRequestedEventArgs args);

    class SaveStatisticsRequestedEventArgs : EventArgs
    {
        public SaveStatisticsRequestedEventArgs(Action<string> callback, string initialValue)
        {
            Callback = callback;
            InitialValue = initialValue;
        }

        public Action<string> Callback { get; private set; }

        public string InitialValue { get; private set; }
    }
}
