using LibDnaSerial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModMonitor
{
    /// <summary>
    /// Interaction logic for DevicePicker.xaml
    /// </summary>
    public partial class DevicePicker : Window
    {
        private Action<DnaDevice> deviceSelectedCallback;

        public DevicePicker(Action<DnaDevice> cb)
        {
            InitializeComponent();
            deviceSelectedCallback = cb;
        }

        private void DevicePickerViewModel_CloseDialog(bool result, DnaDevice device)
        {
            deviceSelectedCallback(device);
            DialogResult = result;
            Close();
        }
    }
}
