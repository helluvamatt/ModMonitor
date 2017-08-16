using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMonitor.Events
{
    delegate void SetProfilePromptRequestedEventHandler(object sender, SetProfilePromptRequestedEventArgs args);

    class SetProfilePromptRequestedEventArgs : EventArgs
    {
        public SetProfilePromptRequestedEventArgs(Action<int> cb, int currentProfile)
        {
            Callback = cb;
            CurrentProfile = currentProfile;
        }

        public Action<int> Callback { get; private set; }

        public int CurrentProfile { get; private set; }
    }
}
