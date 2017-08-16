using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMonitor.Events
{
    delegate void FirePromptRequestedEventHandler(object sender, FirePromptRequestedEventArgs args);

    class FirePromptRequestedEventArgs : EventArgs
    {
        public FirePromptRequestedEventArgs(Action<float> cb, float lastUsedDuration)
        {
            Callback = cb;
            LastUsedDuration = lastUsedDuration;
        }

        public Action<float> Callback { get; private set; }

        public float LastUsedDuration { get; private set; }
    }
}
