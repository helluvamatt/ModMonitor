using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMonitor.Events
{
    delegate void SetPowerPromptRequestedEventHandler(object sender, SetPowerPromptRequestedEventArgs args);

    class SetPowerPromptRequestedEventArgs : EventArgs
    {
        public SetPowerPromptRequestedEventArgs(Action<float> cb, float currentPower)
        {
            Callback = cb;
            CurrentPower = currentPower;
        }

        public Action<float> Callback { get; private set; }

        public float CurrentPower { get; private set; }
    }
}
