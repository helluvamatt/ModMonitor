using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMonitor.Events
{
    public delegate void EditColorEventHandler(object sender, EditColorEventArgs args);

    public class EditColorEventArgs : EventArgs
    {
        public Color InitialSetting { get; set; }

        public string ColorSettingName { get; set; }

        public EditColorEventArgs(Action<string, Color> cb)
        {
            Callback = cb;
        }

        public Action<string, Color> Callback { get; private set; }
    }
}
