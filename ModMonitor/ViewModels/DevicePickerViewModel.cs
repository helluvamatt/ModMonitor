using LibDnaSerial;
using LibDnaSerial.Models;
using MvvmFoundation.Wpf;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModMonitor.ViewModels
{
    class DevicePickerViewModel : DependencyObject
    {
        public ObservableCollection<DnaDevice> Devices { get; private set; }

        public bool NoDevices
        {
            get
            {
                return (bool)GetValue(NoDevicesProperty);
            }
            private set
            {
                SetValue(NoDevicesProperty, value);
            }
        }

        public static readonly DependencyProperty NoDevicesProperty = DependencyProperty.Register("NoDevices", typeof(bool), typeof(DevicePickerViewModel));

        public DnaDevice SelectedDevice
        {
            get
            {
                return (DnaDevice)GetValue(SelectedDeviceProperty);
            }
            set
            {
                SetValue(SelectedDeviceProperty, value);
                FocusSelectedItem();
            }
        }

        public static readonly DependencyProperty SelectedDeviceProperty = DependencyProperty.Register("SelectedDevice", typeof(DnaDevice), typeof(DevicePickerViewModel));

        public ICommand OkCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        public event Action<bool, DnaDevice> CloseDialog = (result, device) => { };

        public event Action FocusSelectedItem = () => { };

        private bool isDeviceChosen = false;

        private ILogger log;

        public DevicePickerViewModel()
        {
            log = LogManager.GetCurrentClassLogger();
            OkCommand = new RelayCommand(OkCommandExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);
            Devices = new ObservableCollection<DnaDevice>();
            new Thread(DeviceWatcherThread).Start();
        }

        private void OkCommandExecute()
        {
            isDeviceChosen = true;
            CloseDialog(SelectedDevice != null, SelectedDevice);
        }

        private void CancelCommandExecute()
        {
            isDeviceChosen = true;
            CloseDialog(false, null);
        }

        private void DeviceWatcherThread()
        {
            while (!isDeviceChosen)
            {
                log.Debug("Refreshing device list...");
                try
                {
                    List<DnaDevice> devices = DnaDeviceManager.ListDnaDevices();
                    if (devices.Count > 0)
                    {
                        log.Debug("Found {0} device(s)", devices.Count);
                        Invoke(() => {
                            var selectedSerialNo = SelectedDevice?.SerialNumber;
                            Devices.Clear();
                            foreach (var device in devices)
                            {
                                Devices.Add(device);
                            }
                            if (selectedSerialNo != null && Devices.Any(d => d.SerialNumber == selectedSerialNo))
                            {
                                SelectedDevice = Devices.First(d => d.SerialNumber == selectedSerialNo);
                            }
                            else
                            {
                                SelectedDevice = Devices[0];
                            }
                            NoDevices = false;
                        });
                    }
                    else
                    {
                        log.Debug("Found 0 device(s)");
                        Invoke(() => NoDevices = true);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Error refreshing device list");
                    Invoke(() =>
                    {
                        MessageBox.Show("An error occurred while querying for DNA devices.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    });
                }

                Thread.Sleep(500);
            }
        }

        private void Invoke(Action action)
        {
            if (!Dispatcher.HasShutdownStarted)
            {
                Dispatcher.Invoke(action);
            }
        }

        internal class QueryResult
        {
            public string Error { get; set; }
            public bool IsError { get; set; }
            public List<DnaDevice> Devices { get; set; }
        }
    }
}
