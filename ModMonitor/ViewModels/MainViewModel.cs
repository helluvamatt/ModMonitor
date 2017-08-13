using ModMonitor.Events;
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
using System.Windows.Media;
using System.Windows.Threading;
using ModMonitor.Models;
using ModMonitor.Utils;
using System.Collections.Concurrent;
using ModMonitor.Properties;
using System.ComponentModel;
using NLog;

namespace ModMonitor.ViewModels
{
    class MainViewModel : DependencyObject, IDisposable
    {
        private const double MAX_OFFSET = 10.0;

        private const float DEFAULT_MAX_POWER = 300f;

        #region Dependency Properties

        #region Status

        public string Status
        {
            get
            {
                return (string)GetValue(StatusProperty);
            }
            set
            {
                SetValue(StatusProperty, value);
            }
        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(string), typeof(MainViewModel), new UIPropertyMetadata("Disconnected"));

        #endregion

        #region ConnectText

        public string ConnectText
        {
            get
            {
                return (string)GetValue(ConnectTextProperty);
            }
            set
            {
                SetValue(ConnectTextProperty, value);
            }
        }

        public static readonly DependencyProperty ConnectTextProperty = DependencyProperty.Register("ConnectText", typeof(string), typeof(MainViewModel), new UIPropertyMetadata("Connect"));

        #endregion

        #region IsRecording

        public bool IsRecording
        {
            get
            {
                return (bool)GetValue(IsRecordingProperty);
            }
            set
            {
                SetValue(IsRecordingProperty, value);
            }
        }

        public static readonly DependencyProperty IsRecordingProperty = DependencyProperty.Register("IsRecording", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region LatestSample

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

        public static readonly DependencyProperty LatestSampleProperty = DependencyProperty.Register("LatestSample", typeof(Sample), typeof(MainViewModel), new UIPropertyMetadata(new Sample()));

        #endregion

        #region CurrentDevice

        public DnaDevice CurrentDevice
        {
            get
            {
                return (DnaDevice)GetValue(CurrentDeviceProperty);
            }
            set
            {
                SetValue(CurrentDeviceProperty, value);
            }
        }

        public static readonly DependencyProperty CurrentDeviceProperty = DependencyProperty.Register("CurrentDevice", typeof(DnaDevice), typeof(MainViewModel));

        #endregion

        #region GraphData

        public ObservableDictionary<double, Sample> GraphData { get; private set; }

        #endregion

        #region MaxTemp

        public Temperature MaxTemp
        {
            get
            {
                return (Temperature)GetValue(MaxTempProperty);
            }
            private set
            {
                SetValue(MaxTempProperty, value);
            }
        }

        public static readonly DependencyProperty MaxTempProperty = DependencyProperty.Register("MaxTemp", typeof(Temperature), typeof(MainViewModel), new UIPropertyMetadata(DEFAULT_MAX_TEMP));

        public static readonly Temperature DEFAULT_MAX_TEMP = new Temperature { Unit = TemperatureUnit.F, Value = 600f };

        #endregion

        #region MaxTempLow

        public Temperature MaxTempLow
        {
            get
            {
                return (Temperature)GetValue(MaxTempLowProperty);
            }
            private set
            {
                SetValue(MaxTempLowProperty, value);
            }
        }

        public static readonly DependencyProperty MaxTempLowProperty = DependencyProperty.Register("MaxTempLow", typeof(Temperature), typeof(MainViewModel), new UIPropertyMetadata(DEFAULT_MAX_TEMP_LOW));

        public static readonly Temperature DEFAULT_MAX_TEMP_LOW = new Temperature { Unit = TemperatureUnit.F, Value = 200f };

        #endregion

        #region MaxPower

        public float MaxPower
        {
            get
            {
                return (float)GetValue(MaxPowerProperty);
            }
            set
            {
                SetValue(MaxPowerProperty, value);
            }
        }

        public static readonly DependencyProperty MaxPowerProperty = DependencyProperty.Register("MaxPower", typeof(float), typeof(MainViewModel), new UIPropertyMetadata(300f));

        #endregion

        #region MaxOffset

        public double MaxOffset
        {
            get
            {
                return (double)GetValue(MaxOffsetProperty);
            }
            set
            {
                SetValue(MaxOffsetProperty, value);
            }
        }

        public static readonly DependencyProperty MaxOffsetProperty = DependencyProperty.Register("MaxOffset", typeof(double), typeof(MainViewModel), new UIPropertyMetadata(MAX_OFFSET));

        #endregion

        #region Commands

        public ICommand ConnectCommand
        {
            get
            {
                return connectCommand;
            }
        }

        public ICommand StartRecordingCommand
        {
            get
            {
                return startRecordingCommand;
            }
        }

        public ICommand StopRecordingCommand
        {
            get
            {
                return stopRecordingCommand;
            }
        }

        public ICommand EditSettingsCommand
        {
            get
            {
                return editSettingsCommand;
            }
        }

        public ICommand ShowAboutCommand
        {
            get
            {
                return showAboutCommand;
            }
        }

        #endregion

        #endregion

        #region Events

        public event SaveFileRequestedEvent SaveFileRequested = (sender, args) => { };

        public event DevicePickerRequestedEvent DevicePickerRequested = (sender, args) => { };

        public event EventHandler EditSettingsRequested = (sender, args) => { };

        public event EventHandler ShowAboutRequested = (sender, args) => { };

        #endregion

        #region Private members

        private DnaSampleManager sampleManager = null;
        private SampleRecorder sampleRecorder = null;

        private ICommand connectCommand;
        private ICommand startRecordingCommand;
        private ICommand stopRecordingCommand;
        private ICommand editSettingsCommand;
        private ICommand showAboutCommand;

        private bool isFiring = false;
        private DateTime puffStart;

        private ILogger log;

        #endregion

        public MainViewModel()
        {
            GraphData = new ObservableDictionary<double, Sample>();
            connectCommand = new RelayCommand(Connect);
            startRecordingCommand = new RelayCommand(StartRecording, () => !IsRecording);
            stopRecordingCommand = new RelayCommand(StopRecording, () => IsRecording);
            editSettingsCommand = new RelayCommand(EditSettings);
            showAboutCommand = new RelayCommand(ShowAbout);
            SetGraphTemperatureUnit(Settings.Default.TemperatureUnitForce ? Settings.Default.TemperatureUnit : TemperatureUnit.F);
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            log = LogManager.GetCurrentClassLogger();
        }

        private void Connect()
        {
            if (sampleManager != null)
            {
                // "Disconnect"
                Task.Run(() =>
                {
                    log.Info("Disconnectiong...");
                    sampleManager.Dispose();
                    sampleManager = null;
                    Invoke(() =>
                    {
                        CurrentDevice = null;
                        LatestSample = null;
                        ConnectText = "Connect";
                        Status = "Disconnected";
                        GraphData.Clear();
                    });
                    log.Info("Disconnected");
                });
            }
            else
            {
                log.Info("Connect request processing...");
                DevicePickerRequested(this, new DevicePickerRequestedEventArgs(Connect));
            }
        }

        private void Connect(DnaDevice device)
        {
            if (device != null)
            {
                Task.Run(() =>
                {
                    log.Info("Connecting to device \"{0}\"...", device);
                    try
                    {
                        sampleManager = new DnaSampleManager(device.SerialPort, Settings.Default.SampleThrottle);
                        sampleManager.SampleCollected += SampleArrived;
                        sampleManager.Error += Error;
                        sampleManager.Connect();
                        Invoke(() =>
                        {
                            CurrentDevice = device;
                            if (CurrentDevice.MaxPower.HasValue)
                            {
                                MaxPower = (((int)CurrentDevice.MaxPower.Value / 50) + 1) * 50f;
                            }
                            else
                            {
                                MaxPower = DEFAULT_MAX_POWER;
                            }
                            ConnectText = "Disconnect";
                            Status = string.Format("Connected to \"{0}\"", device);
                        });
                        log.Info("Connected to device \"{0}\"", device);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "Error connecting to device \"{0}\"", device);
                        Invoke(() => MessageBox.Show("Error connecting to DNA device.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning));
                    }
                });
            }
        }

        private void StartRecording()
        {
            log.Info("Recording start request...");
            var args = new SaveFileRequestedEventArgs(new Action<string>(StartRecording));
            args.Filters = "CSV files|*.csv|All files|*.*";
            args.Title = "Save CSV file...";
            SaveFileRequested(this, args);
        }

        private void StartRecording(string fileName)
        {
            log.Info("Recording started. Recording to file \"{0}\"", fileName);
            sampleRecorder = new SampleRecorder(fileName);
            IsRecording = true;
        }

        private void StopRecording()
        {
            IsRecording = false;
            sampleRecorder.Dispose();
            sampleRecorder = null;
            log.Info("Recording stopped");
        }

        private void EditSettings()
        {
            EditSettingsRequested(this, EventArgs.Empty);
        }

        private void ShowAbout()
        {
            ShowAboutRequested(this, EventArgs.Empty);
        }

        private void Error(string msg, Exception ex)
        {
            log.Error(ex, "Exception on device thread: {0}", msg);
            sampleManager.Dispose();
            sampleManager = null;
            Invoke(() => {
                LatestSample = null;
                ConnectText = "Connect";
                Status = "Disconnected";
                GraphData.Clear();
            });
        }

        private void SampleArrived(Sample sample)
        {
            Invoke(() =>
            {
                if (Settings.Default.TemperatureUnitForce)
                {
                    var unit = Settings.Default.TemperatureUnit;
                    SetGraphTemperatureUnit(unit);
                    if (sample.Temperature.Unit != unit) sample.Temperature = new Temperature { Unit = unit, Value = sample.Temperature.GetValue(unit) };
                    if (sample.TemperatureSetpoint.Unit != unit) sample.TemperatureSetpoint = new Temperature { Unit = unit, Value = sample.TemperatureSetpoint.GetValue(unit) };
                    if (sample.BoardTemperature.Unit != unit) sample.BoardTemperature = new Temperature { Unit = unit, Value = sample.BoardTemperature.GetValue(unit) };
                    if (sample.RoomTemperature.Unit != unit) sample.RoomTemperature = new Temperature { Unit = unit, Value = sample.RoomTemperature.GetValue(unit) };
                }
                else
                {
                    SetGraphTemperatureUnit(sample.BoardTemperature.Unit);
                }

                LatestSample = sample;

                if (IsRecording && sampleRecorder != null)
                {
                    sampleRecorder.RecordSample(sample);
                }

                if (sample.Buttons.HasFlag(Buttons.Fire))
                {
                    if (!isFiring)
                    {
                        puffStart = sample.End;
                        GraphData.Clear();
                        MaxOffset = MAX_OFFSET;
                    }
                    isFiring = true;
                    GraphData.Add((sample.End - puffStart).TotalSeconds, sample);
                }
                else
                {
                    if (isFiring && Settings.Default.ExpandGraph)
                    {
                        MaxOffset = GraphData.Keys.Max();
                    }
                    isFiring = false;
                }
            });
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SampleThrottle" && sampleManager != null)
            {
                sampleManager.Throttle = Settings.Default.SampleThrottle;
            }
        }

        private void SetGraphTemperatureUnit(TemperatureUnit unit)
        {
            MaxTempLow = new Temperature { Unit = unit, Value = DEFAULT_MAX_TEMP_LOW.GetValue(unit) };
            MaxTemp = new Temperature { Unit = unit, Value = DEFAULT_MAX_TEMP.GetValue(unit) };
        }

        private void Invoke(Action action)
        {
            if (!Dispatcher.HasShutdownStarted)
            {
                Dispatcher.Invoke(action);
            }
        }

        public void Dispose()
        {
            log.Debug("Dispose called, async shutdown of sample manager...");
            Task.Run(() =>
            {
                if (sampleManager != null)
                {
                    sampleManager.Dispose();
                }
            });
        }

        internal class ConnectResult
        {
            public string Error { get; set; }
            public bool IsError { get; set; }
        }
    }
}
