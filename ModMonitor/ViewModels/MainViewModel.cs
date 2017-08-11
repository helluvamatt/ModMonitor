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

namespace ModMonitor.ViewModels
{
    class MainViewModel : DependencyObject, IDisposable
    {
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

        #region ConnectEnabled

        public bool ConnectEnabled
        {
            get
            {
                return (bool)GetValue(ConnectEnabledProperty);
            }
            set
            {
                SetValue(ConnectEnabledProperty, value);
            }
        }

        public static readonly DependencyProperty ConnectEnabledProperty = DependencyProperty.Register("ConnectEnabled", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(true));

        #endregion

        #region IsGraphPaused

        public bool IsGraphPaused
        {
            get
            {
                return (bool)GetValue(IsGraphPausedProperty);
            }
            set
            {
                SetValue(IsGraphPausedProperty, value);
            }
        }

        public static readonly DependencyProperty IsGraphPausedProperty = DependencyProperty.Register("IsGraphPaused", typeof(bool), typeof(MainViewModel));

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

        #region MinTime

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

        public static readonly DependencyProperty MinTimeProperty = DependencyProperty.Register("MinTime", typeof(DateTime), typeof(MainViewModel), new UIPropertyMetadata(DateTime.Now));

        #endregion

        #region MaxTime

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

        public static readonly DependencyProperty MaxTimeProperty = DependencyProperty.Register("MaxTime", typeof(DateTime), typeof(MainViewModel), new UIPropertyMetadata(DateTime.Now));

        #endregion

        #region TemperatureUnit

        public TemperatureUnit TemperatureUnit
        {
            get
            {
                return (TemperatureUnit)GetValue(TemperatureUnitProperty);
            }
            set
            {
                SetValue(TemperatureUnitProperty, value);
                MaxTemp.Unit = value;
                MaxTempLow.Unit = value;
            }
        }

        public static readonly DependencyProperty TemperatureUnitProperty = DependencyProperty.Register("TemperatureUnit", typeof(TemperatureUnit), typeof(MainViewModel), new UIPropertyMetadata(TemperatureUnit.F));

        public ImmutableTemperature MaxTemp { get; private set; }

        public ImmutableTemperature MaxTempLow { get; private set; }

        #endregion

        #region ShowPower

        public bool ShowPower
        {
            get
            {
                return (bool)GetValue(ShowPowerProperty);
            }
            set
            {
                SetValue(ShowPowerProperty, value);
            }
        }

        public static readonly DependencyProperty ShowPowerProperty = DependencyProperty.Register("ShowPower", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(true));

        #endregion

        #region ShowPowerSetpoint

        public bool ShowPowerSetpoint
        {
            get
            {
                return (bool)GetValue(ShowPowerSetpointProperty);
            }
            set
            {
                SetValue(ShowPowerSetpointProperty, value);
            }
        }

        public static readonly DependencyProperty ShowPowerSetpointProperty = DependencyProperty.Register("ShowPowerSetpoint", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region ShowTemperature

        public bool ShowTemperature
        {
            get
            {
                return (bool)GetValue(ShowTemperatureProperty);
            }
            set
            {
                SetValue(ShowTemperatureProperty, value);
            }
        }

        public static readonly DependencyProperty ShowTemperatureProperty = DependencyProperty.Register("ShowTemperature", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(true));

        #endregion

        #region ShowTemperatureSetpoint

        public bool ShowTemperatureSetpoint
        {
            get
            {
                return (bool)GetValue(ShowTemperatureSetpointProperty);
            }
            set
            {
                SetValue(ShowTemperatureSetpointProperty, value);
            }
        }

        public static readonly DependencyProperty ShowTemperatureSetpointProperty = DependencyProperty.Register("ShowTemperatureSetpoint", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region ShowVoltage

        public bool ShowVoltage
        {
            get
            {
                return (bool)GetValue(ShowVoltageProperty);
            }
            set
            {
                SetValue(ShowVoltageProperty, value);
            }
        }

        public static readonly DependencyProperty ShowVoltageProperty = DependencyProperty.Register("ShowVoltage", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region ShowCurrent

        public bool ShowCurrent
        {
            get
            {
                return (bool)GetValue(ShowCurrentProperty);
            }
            set
            {
                SetValue(ShowCurrentProperty, value);
            }
        }

        public static readonly DependencyProperty ShowCurrentProperty = DependencyProperty.Register("ShowCurrent", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region ShowLiveResistance

        public bool ShowLiveResistance
        {
            get
            {
                return (bool)GetValue(ShowLiveResistanceProperty);
            }
            set
            {
                SetValue(ShowLiveResistanceProperty, value);
            }
        }

        public static readonly DependencyProperty ShowLiveResistanceProperty = DependencyProperty.Register("ShowLiveResistance", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region ShowColdResistance

        public bool ShowColdResistance
        {
            get
            {
                return (bool)GetValue(ShowColdResistanceProperty);
            }
            set
            {
                SetValue(ShowColdResistanceProperty, value);
            }
        }

        public static readonly DependencyProperty ShowColdResistanceProperty = DependencyProperty.Register("ShowColdResistance", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region ShowBatteryVoltage

        public bool ShowBatteryVoltage
        {
            get
            {
                return (bool)GetValue(ShowBatteryVoltageProperty);
            }
            set
            {
                SetValue(ShowBatteryVoltageProperty, value);
            }
        }

        public static readonly DependencyProperty ShowBatteryVoltageProperty = DependencyProperty.Register("ShowBatteryVoltage", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region ShowBatteryLevel

        public bool ShowBatteryLevel
        {
            get
            {
                return (bool)GetValue(ShowBatteryLevelProperty);
            }
            set
            {
                SetValue(ShowBatteryLevelProperty, value);
            }
        }

        public static readonly DependencyProperty ShowBatteryLevelProperty = DependencyProperty.Register("ShowBatteryLevel", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region ShowBoardTemperature

        public bool ShowBoardTemperature
        {
            get
            {
                return (bool)GetValue(ShowBoardTemperatureProperty);
            }
            set
            {
                SetValue(ShowBoardTemperatureProperty, value);
            }
        }

        public static readonly DependencyProperty ShowBoardTemperatureProperty = DependencyProperty.Register("ShowBoardTemperature", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region ShowRoomTemperature

        public bool ShowRoomTemperature
        {
            get
            {
                return (bool)GetValue(ShowRoomTemperatureProperty);
            }
            set
            {
                SetValue(ShowRoomTemperatureProperty, value);
            }
        }

        public static readonly DependencyProperty ShowRoomTemperatureProperty = DependencyProperty.Register("ShowRoomTemperature", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region ColorPower

        public SolidColorBrush ColorPower
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorPowerProperty);
            }
            set
            {
                SetValue(ColorPowerProperty, value);
            }
        }

        public static readonly DependencyProperty ColorPowerProperty = DependencyProperty.Register("ColorPower", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.Lime)));

        #endregion

        #region ColorPowerSetpoint

        public SolidColorBrush ColorPowerSetpoint
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorPowerSetpointProperty);
            }
            set
            {
                SetValue(ColorPowerSetpointProperty, value);
            }
        }

        public static readonly DependencyProperty ColorPowerSetpointProperty = DependencyProperty.Register("ColorPowerSetpoint", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.Green)));

        #endregion

        #region ColorTemperature

        public SolidColorBrush ColorTemperature
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorTemperatureProperty);
            }
            set
            {
                SetValue(ColorTemperatureProperty, value);
            }
        }

        public static readonly DependencyProperty ColorTemperatureProperty = DependencyProperty.Register("ColorTemperature", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.Red)));

        #endregion

        #region ColorTemperatureSetpoint

        public SolidColorBrush ColorTemperatureSetpoint
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorTemperatureSetpointProperty);
            }
            set
            {
                SetValue(ColorTemperatureSetpointProperty, value);
            }
        }

        public static readonly DependencyProperty ColorTemperatureSetpointProperty = DependencyProperty.Register("ColorTemperatureSetpoint", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.DarkRed)));

        #endregion

        #region ColorVoltage

        public SolidColorBrush ColorVoltage
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorVoltageProperty);
            }
            set
            {
                SetValue(ColorVoltageProperty, value);
            }
        }

        public static readonly DependencyProperty ColorVoltageProperty = DependencyProperty.Register("ColorVoltage", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.Cyan)));

        #endregion

        #region ColorCurrent

        public SolidColorBrush ColorCurrent
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorCurrentProperty);
            }
            set
            {
                SetValue(ColorCurrentProperty, value);
            }
        }

        public static readonly DependencyProperty ColorCurrentProperty = DependencyProperty.Register("ColorCurrent", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.DarkGoldenrod)));

        #endregion

        #region ColorLiveResistance

        public SolidColorBrush ColorLiveResistance
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorLiveResistanceProperty);
            }
            set
            {
                SetValue(ColorLiveResistanceProperty, value);
            }
        }

        public static readonly DependencyProperty ColorLiveResistanceProperty = DependencyProperty.Register("ColorLiveResistance", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.MediumPurple)));

        #endregion

        #region ColorColdResistance

        public SolidColorBrush ColorColdResistance
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorColdResistanceProperty);
            }
            set
            {
                SetValue(ColorColdResistanceProperty, value);
            }
        }

        public static readonly DependencyProperty ColorColdResistanceProperty = DependencyProperty.Register("ColorColdResistance", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.Purple)));

        #endregion

        #region ColorBatteryVoltage

        public SolidColorBrush ColorBatteryVoltage
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorBatteryVoltageProperty);
            }
            set
            {
                SetValue(ColorBatteryVoltageProperty, value);
            }
        }

        public static readonly DependencyProperty ColorBatteryVoltageProperty = DependencyProperty.Register("ColorBatteryVoltage", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region ColorBatteryLevel

        public SolidColorBrush ColorBatteryLevel
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorBatteryLevelProperty);
            }
            set
            {
                SetValue(ColorBatteryLevelProperty, value);
            }
        }

        public static readonly DependencyProperty ColorBatteryLevelProperty = DependencyProperty.Register("ColorBatteryLevel", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        #endregion

        #region ColorBoardTemperature

        public SolidColorBrush ColorBoardTemperature
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorBoardTemperatureProperty);
            }
            set
            {
                SetValue(ColorBoardTemperatureProperty, value);
            }
        }

        public static readonly DependencyProperty ColorBoardTemperatureProperty = DependencyProperty.Register("ColorBoardTemperature", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.DarkSalmon)));

        #endregion

        #region ColorRoomTemperature

        public SolidColorBrush ColorRoomTemperature
        {
            get
            {
                return (SolidColorBrush)GetValue(ColorRoomTemperatureProperty);
            }
            set
            {
                SetValue(ColorRoomTemperatureProperty, value);
            }
        }

        public static readonly DependencyProperty ColorRoomTemperatureProperty = DependencyProperty.Register("ColorRoomTemperature", typeof(SolidColorBrush), typeof(MainViewModel), new UIPropertyMetadata(new SolidColorBrush(Colors.BurlyWood)));

        #endregion

        #region GraphData

        public ObservableDictionary<DateTime, Sample> GraphData { get; private set; }

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

        #endregion

        #endregion

        public event SaveFileRequestedEvent SaveFileRequested = (sender, args) => { };

        public event DevicePickerRequestedEvent DevicePickerRequested = (sender, args) => { };

        private DnaSampleManager sampleManager = null;
        private SampleRecorder sampleRecorder = null;

        private ICommand connectCommand;
        private ICommand startRecordingCommand;
        private ICommand stopRecordingCommand;

        public MainViewModel()
        {
            GraphData = new ObservableDictionary<DateTime, Sample>();
            connectCommand = new RelayCommand(Connect, () => ConnectEnabled);
            startRecordingCommand = new RelayCommand(StartRecording, () => !IsRecording);
            stopRecordingCommand = new RelayCommand(StopRecording, () => IsRecording);
            MaxTemp = new ImmutableTemperature(new Temperature { Value = 600, Unit = TemperatureUnit.F });
            MaxTempLow = new ImmutableTemperature(new Temperature { Value = 200, Unit = TemperatureUnit.F });
            MinTime = DateTime.Now;
            MaxTime = DateTime.Now + TimeSpan.FromSeconds(30);
        }

        private void Connect()
        {
            ConnectEnabled = false;
            if (sampleManager != null)
            {
                Task.Run(() =>
                {
                    sampleManager.Dispose();
                    sampleManager = null;
                    Invoke(() =>
                    {
                        LatestSample = null;
                        ConnectText = "Connect";
                        Status = "Disconnected";
                        ConnectEnabled = true;
                    });
                });
            }
            else
            {
                DevicePickerRequested(this, new DevicePickerRequestedEventArgs(Connect));
            }
        }

        private void Connect(DnaDevice device)
        {
            if (device != null)
            {
                Task.Run(() =>
                {
                    try
                    {
                        sampleManager = new DnaSampleManager(device.SerialPort);
                        sampleManager.SampleCollected += SampleArrived;
                        sampleManager.Error += Error;
                        sampleManager.Connect();
                        Invoke(() =>
                        {
                            ConnectText = "Disconnect";
                            Status = string.Format("Connected to \"{0}\"", device);
                        });
                    }
                    catch (Exception)
                    {
                        // TODO Log the exception
                        Invoke(() => MessageBox.Show("Error connecting to DNA device.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning));
                    }
                    Invoke(() => ConnectEnabled = true);
                });
            }
            else
            {
                ConnectEnabled = true;
            }
        }

        private void StartRecording()
        {
            var args = new SaveFileRequestedEventArgs(new Action<string>(StartRecording));
            args.Filters = "CSV files|*.csv|All files|*.*";
            args.Title = "Save CSV file...";
            SaveFileRequested(this, args);
        }

        private void StartRecording(string fileName)
        {
            sampleRecorder = new SampleRecorder(fileName);
            IsRecording = true;
            IsGraphPaused = true;
        }

        private void StopRecording()
        {
            IsRecording = false;
            IsGraphPaused = false;
            sampleRecorder.Dispose();
            sampleRecorder = null;
        }

        private void Error(string msg, Exception ex)
        {
            Console.Error.WriteLine("Error occurred in device thread: {0}", ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
            sampleManager.Dispose();
            sampleManager = null;
            Invoke(() => {
                LatestSample = null;
                ConnectText = "Connect";
                Status = "Disconnected";
            });
        }

        private void SampleArrived(Sample sample)
        {
            Invoke(() =>
            {
                LatestSample = sample;
                if (!IsGraphPaused)
                {
                    MaxTime = sample.End;
                    MinTime = sample.End - TimeSpan.FromSeconds(30);
                    if (sample.Index % 2 == 0)
                    {
                        GraphData.RemoveAll(k => k < MinTime);
                        GraphData.Add(sample.End, sample);
                    }
                }
                if (IsRecording && sampleRecorder != null)
                {
                    sampleRecorder.RecordSample(sample);
                }
            });
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
