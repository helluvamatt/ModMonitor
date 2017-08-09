using LibDnaSerial;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DnaDeviceMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DependencyProperty LatestSampleProperty = DependencyProperty.Register("LatestSample", typeof(Sample), typeof(MainWindow), new UIPropertyMetadata(new Sample()));
        public static DependencyProperty MinTimeProperty = DependencyProperty.Register("MinTime", typeof(DateTime), typeof(MainWindow), new UIPropertyMetadata(DateTime.Now));
        public static DependencyProperty MaxTimeProperty = DependencyProperty.Register("MaxTime", typeof(DateTime), typeof(MainWindow), new UIPropertyMetadata(DateTime.Now));

        public Sample LatestSample
        {
            get
            {
                return (Sample)GetValue(LatestSampleProperty);
            }
            set
            {
                SetValue(LatestSampleProperty, value);
            }
        }

        public DateTime MinTime
        {
            get
            {
                return (DateTime)GetValue(MinTimeProperty);
            }
            set
            {
                SetValue(MinTimeProperty, value);
            }
        }

        public DateTime MaxTime
        {
            get
            {
                return (DateTime)GetValue(MaxTimeProperty);
            }
            set
            {
                SetValue(MaxTimeProperty, value);
            }
        }

        public ObservableCollection<Sample> GraphData { get; private set; }

        public string Status
        {
            get
            {
                return lblStatus.Text;
            }
            set
            {
                lblStatus.Text = value;
            }
        }

        private DnaSampleManager sampleManager = null;

        public MainWindow()
        {
            InitializeComponent();
            GraphData = new ObservableCollection<Sample>();
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            btnConnect.IsEnabled = false;
            if (sampleManager != null)
            {
                await Task.Run(() =>
                {
                    sampleManager.Dispose();
                    sampleManager = null;
                });
                LatestSample = null;
                btnConnect.Content = "Connect";
                lblStatus.Text = "Disconnected";
            }
            else
            {
                var result = await Task.Run(() =>
                {
                    try
                    {
                        List<DnaDevice> devices = DnaDeviceManager.ListDnaDevices();
                        if (devices.Count > 0)
                        {
                            var device = devices[0];
                            sampleManager = new DnaSampleManager(device.SerialPort, 50);
                            sampleManager.SampleCollected += SampleArrived;
                            sampleManager.Error += Error;
                            sampleManager.Connect();
                            return new ConnectResult { Device = device };
                        }
                        else
                        {
                            return new ConnectResult { IsError = true, Error = "No DNA devices found. Please connect a DNA device and try again." };
                        }
                    }
                    catch (Exception)
                    {
                        return new ConnectResult { IsError = true, Error = "Error connecting to DNA device." };
                        // TODO Log the exception
                    }
                });
                if (result.IsError)
                {
                    MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    btnConnect.Content = "Disconnect";
                    lblStatus.Text = string.Format("Connected to \"{0}\"", result.Device);
                }
            }
            btnConnect.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Task.Run(() =>
            {
                if (sampleManager != null)
                {
                    sampleManager.Dispose();
                }
            });
        }

        private void Error(string msg, Exception ex)
        {
            Console.WriteLine("Error occurred in device thread: {0}", ex.Message);
            sampleManager.Dispose();
            sampleManager = null;
            if (!Dispatcher.HasShutdownStarted) Dispatcher.Invoke(() => {
                LatestSample = null;
                btnConnect.Content = "Connect";
                lblStatus.Text = "Disconnected";
            });
        }

        private void SampleArrived(Sample sample)
        {
            if (!Dispatcher.HasShutdownStarted) Dispatcher.Invoke(() =>
            {
                LatestSample = sample;
                MinTime = sample.End - TimeSpan.FromSeconds(30);
                MaxTime = sample.End;
                GraphData.Add(sample);
            });
        }
    }

    internal class ConnectResult
    {
        public string Error { get; set; }
        public bool IsError { get; set; }
        public DnaDevice Device { get; set; }
    }
}
