using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ModMonitor.Utils
{
    class EnterKeyPressTrigger : EventTrigger
    {
        public EnterKeyPressTrigger() : base("KeyUp") { }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as KeyEventArgs;
            if (e != null && e.Key == Key.Enter)
            {
                InvokeActions(eventArgs);
            }
        }
    }
}
