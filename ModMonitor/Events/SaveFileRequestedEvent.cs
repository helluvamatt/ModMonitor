using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMonitor.Events
{
    public delegate void SaveFileRequestedEvent(object sender, SaveFileRequestedEventArgs args);

    public class SaveFileRequestedEventArgs : EventArgs
    {
        public SaveFileRequestedEventArgs(Action<string> cb)
        {
            Callback = cb;
        }

        public string Title { get; set; }

        public string InitialDirectory { get; set; }

        public string Filters { get; set; }

        public Action<string> Callback { get; private set; }
    }
}
