using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMonitor.Events
{
    internal delegate void ConsoleRequestedEventHandler(object sender, ConsoleRequestedEventArgs args);

    internal class ConsoleRequestedEventArgs : EventArgs
    {
        public ConsoleRequestedEventArgs(Action<string, Action<string>> cb)
        {
            Callback = cb;
        }

        public Action<string, Action<string>> Callback { get; private set; }
    }
}
