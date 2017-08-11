using LibDnaSerial;
using MvvmFoundation.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModMonitor.ViewModels
{
    class DevicePickerViewModel : DependencyObject
    {
        public ObservableCollection<DnaDevice> Devices { get; private set; }

        public bool Loading
        {
            get
            {
                return (bool)GetValue(LoadingProperty);
            }
            private set
            {
                SetValue(LoadingProperty, value);
            }
        }

        public static readonly DependencyProperty LoadingProperty = DependencyProperty.Register("Loading", typeof(bool), typeof(DevicePickerViewModel));

        public DnaDevice SelectedDevice
        {
            get
            {
                return (DnaDevice)GetValue(SelectedDeviceProperty);
            }
            set
            {
                SetValue(SelectedDeviceProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedDeviceProperty = DependencyProperty.Register("SelectedDevice", typeof(DnaDevice), typeof(DevicePickerViewModel));

        public ICommand OkCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        public event Action<bool, DnaDevice> CloseDialog = (result, device) => { };

        public DevicePickerViewModel()
        {
            OkCommand = new RelayCommand(OkCommandExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);
            RefreshCommand = new RelayCommand(Refresh);
            Devices = new ObservableCollection<DnaDevice>();
            Refresh();
        }

        private void OkCommandExecute()
        {
            CloseDialog(SelectedDevice != null, SelectedDevice);
        }

        private void CancelCommandExecute()
        {
            CloseDialog(false, null);
        }

        private async void Refresh()
        {
            Loading = true;
            var result = await Task.Run(() =>
            {
                try
                {
                    List<DnaDevice> devices = DnaDeviceManager.ListDnaDevices();
                    if (devices.Count > 0)
                    {
                        return new QueryResult { Devices = devices };
                    }
                    else
                    {
                        return new QueryResult { IsError = true, Error = "No DNA devices found. Please connect a DNA device and try again." };
                    }
                }
                catch (Exception)
                {
                    return new QueryResult { IsError = true, Error = "An error occurred while querying for DNA devices." };
                    // TODO Log the exception
                }
            });
            if (result.IsError)
            {
                MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Devices.Clear();
                foreach (var device in result.Devices)
                {
                    Devices.Add(device);
                }
            }
            Loading = false;
        }

        internal class QueryResult
        {
            public string Error { get; set; }
            public bool IsError { get; set; }
            public List<DnaDevice> Devices { get; set; }
        }
    }
}
