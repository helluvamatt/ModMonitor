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
using LibDnaSerial.Models;

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

        #region IsCharging

        public bool IsCharging
        {
            get
            {
                return (bool)GetValue(IsChargingProperty);
            }
            set
            {
                SetValue(IsChargingProperty, value);
            }
        }

        public static readonly DependencyProperty IsChargingProperty = DependencyProperty.Register("IsCharging", typeof(bool), typeof(MainViewModel));

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

        #region IsStatsMode

        public bool IsStatsMode
        {
            get
            {
                return (bool)GetValue(IsStatsModeProperty);
            }
            set
            {
                SetValue(IsStatsModeProperty, value);
                if (value) RefreshStatistics();
            }
        }

        public static readonly DependencyProperty IsStatsModeProperty = DependencyProperty.Register("IsStatsMode", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

        #endregion

        #region IsDownloadingStatistics

        public bool IsDownloadingStatistics
        {
            get
            {
                return (bool)GetValue(IsDownloadingStatisticsProperty);
            }
            set
            {
                SetValue(IsDownloadingStatisticsProperty, value);
            }
        }

        public static readonly DependencyProperty IsDownloadingStatisticsProperty = DependencyProperty.Register("IsDownloadingStatistics", typeof(bool), typeof(MainViewModel), new UIPropertyMetadata(false));

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

        #region LatestStatisticSample

        public LastPuffStatisticsSample LatestStatisticSample
        {
            get
            {
                return (LastPuffStatisticsSample)GetValue(LatestStatisticSampleProperty);
            }
            set
            {
                SetValue(LatestStatisticSampleProperty, value);
            }
        }

        public static readonly DependencyProperty LatestStatisticSampleProperty = DependencyProperty.Register("LatestStatisticSample", typeof(LastPuffStatisticsSample), typeof(MainViewModel));

        #endregion

        #region LatestDetailedStatisticsSample

        public DetailedStatisticsSample LatestDetailedStatisticsSample
        {
            get
            {
                return (DetailedStatisticsSample)GetValue(LatestDetailedStatisticsSampleProperty);
            }
            set
            {
                SetValue(LatestDetailedStatisticsSampleProperty, value);
            }
        }

        public static readonly DependencyProperty LatestDetailedStatisticsSampleProperty = DependencyProperty.Register("LatestDetailedStatisticsSample", typeof(DetailedStatisticsSample), typeof(MainViewModel));

        #endregion

        #region CurrentPuffDuration

        public double CurrentPuffDuration
        {
            get
            {
                return (double)GetValue(CurrentPuffDurationProperty);
            }
            set
            {
                SetValue(CurrentPuffDurationProperty, value);
            }
        }

        public static readonly DependencyProperty CurrentPuffDurationProperty = DependencyProperty.Register("CurrentPuffDuration", typeof(double), typeof(MainViewModel));

        #endregion

        #region PeakTemperature

        public Temperature PeakTemperature
        {
            get
            {
                return (Temperature)GetValue(PeakTemperatureProperty);
            }
            set
            {
                SetValue(PeakTemperatureProperty, value);
            }
        }

        public static readonly DependencyProperty PeakTemperatureProperty = DependencyProperty.Register("PeakTemperature", typeof(Temperature), typeof(MainViewModel));

        #endregion

        #region BatterySag

        public float BatterySag
        {
            get
            {
                return (float)GetValue(BatterySagProperty);
            }
            set
            {
                SetValue(BatterySagProperty, value);
            }
        }

        public static readonly DependencyProperty BatterySagProperty = DependencyProperty.Register("BatterySag", typeof(float), typeof(MainViewModel));

        #endregion

        #region PreheatDuration

        public float PreheatDuration
        {
            get
            {
                return (float)GetValue(PreheatDurationProperty);
            }
            set
            {
                SetValue(PreheatDurationProperty, value);
            }
        }

        public static readonly DependencyProperty PreheatDurationProperty = DependencyProperty.Register("PreheatDuration", typeof(float), typeof(MainViewModel));

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

        #region GraphShowAxisTemperature

        public bool GraphShowAxisTemperature
        {
            get
            {
                return (bool)GetValue(GraphShowAxisTemperatureProperty);
            }
            set
            {
                SetValue(GraphShowAxisTemperatureProperty, value);
            }
        }

        public static readonly DependencyProperty GraphShowAxisTemperatureProperty = DependencyProperty.Register("GraphShowAxisTemperature", typeof(bool), typeof(MainViewModel));

        #endregion

        #region GraphShowAxisPower

        public bool GraphShowAxisPower
        {
            get
            {
                return (bool)GetValue(GraphShowAxisPowerProperty);
            }
            set
            {
                SetValue(GraphShowAxisPowerProperty, value);
            }
        }

        public static readonly DependencyProperty GraphShowAxisPowerProperty = DependencyProperty.Register("GraphShowAxisPower", typeof(bool), typeof(MainViewModel));

        #endregion

        #region GraphShowAxisVoltage

        public bool GraphShowAxisVoltage
        {
            get
            {
                return (bool)GetValue(GraphShowAxisVoltageProperty);
            }
            set
            {
                SetValue(GraphShowAxisVoltageProperty, value);
            }
        }

        public static readonly DependencyProperty GraphShowAxisVoltageProperty = DependencyProperty.Register("GraphShowAxisVoltage", typeof(bool), typeof(MainViewModel));

        #endregion

        #region GraphShowAxisCurrent

        public bool GraphShowAxisCurrent
        {
            get
            {
                return (bool)GetValue(GraphShowAxisCurrentProperty);
            }
            set
            {
                SetValue(GraphShowAxisCurrentProperty, value);
            }
        }

        public static readonly DependencyProperty GraphShowAxisCurrentProperty = DependencyProperty.Register("GraphShowAxisCurrent", typeof(bool), typeof(MainViewModel));

        #endregion

        #region GraphShowAxisResistance

        public bool GraphShowAxisResistance
        {
            get
            {
                return (bool)GetValue(GraphShowAxisResistanceProperty);
            }
            set
            {
                SetValue(GraphShowAxisResistanceProperty, value);
            }
        }

        public static readonly DependencyProperty GraphShowAxisResistanceProperty = DependencyProperty.Register("GraphShowAxisResistance", typeof(bool), typeof(MainViewModel));

        #endregion

        #region GraphShowAxisPercentage

        public bool GraphShowAxisPercentage
        {
            get
            {
                return (bool)GetValue(GraphShowAxisPercentageProperty);
            }
            set
            {
                SetValue(GraphShowAxisPercentageProperty, value);
            }
        }

        public static readonly DependencyProperty GraphShowAxisPercentageProperty = DependencyProperty.Register("GraphShowAxisPercentage", typeof(bool), typeof(MainViewModel));

        #endregion

        #region Commands

        public ICommand ConnectCommand { get; private set; }

        public ICommand StartRecordingCommand { get; private set; }

        public ICommand StopRecordingCommand { get; private set; }

        public ICommand EditSettingsCommand { get; private set; }

        public ICommand ShowAboutCommand { get; private set; }

        public ICommand RefreshStatisticsCommand { get; private set; }

        public ICommand StatisticsModeCommand { get; private set; }

        public ICommand LiveDataModeCommand { get; private set; }

        public ICommand SetTemperatureCommand { get; private set; }

        public ICommand SetPowerCommand { get; private set; }

        public ICommand SetProfileCommand { get; private set; }

        public ICommand FireCommand { get; private set; }

        public ICommand ConsoleCommand { get; private set; }

        #endregion

        #endregion

        #region Events

        public event SaveFileRequestedEvent SaveFileRequested;

        public event DevicePickerRequestedEvent DevicePickerRequested;

        public event EventHandler EditSettingsRequested;

        public event EventHandler ShowAboutRequested;

        public event SetTemperaturePromptRequestedEventHandler SetTemperaturePromptRequested;

        public event SetPowerPromptRequestedEventHandler SetPowerPromptRequested;

        public event SetProfilePromptRequestedEventHandler SetProfilePromptRequested;

        public event FirePromptRequestedEventHandler FirePromptRequested;

        public event ConsoleRequestedEventHandler ConsoleRequested;

        #endregion

        #region Private members

        private DnaSampleManager sampleManager = null;
        private SampleRecorder sampleRecorder = null;

        private bool isFiring = false;
        private bool hadPreheat = false;
        private DateTime puffStart;
        private FixedSizeQueue<float> batteryVoltages;

        private double hoveredX;
        private bool isHovered = false;

        private ILogger log;

        #endregion

        public MainViewModel()
        {
            log = LogManager.GetCurrentClassLogger();
            GraphData = new ObservableDictionary<double, Sample>();
            batteryVoltages = new FixedSizeQueue<float>(100);
            ConnectCommand = new RelayCommand(Connect);
            StartRecordingCommand = new RelayCommand(StartRecording, () => !IsRecording);
            StopRecordingCommand = new RelayCommand(StopRecording, () => IsRecording);
            EditSettingsCommand = new RelayCommand(EditSettings);
            ShowAboutCommand = new RelayCommand(ShowAbout);
            RefreshStatisticsCommand = new RelayCommand(RefreshStatistics);
            StatisticsModeCommand = new RelayCommand(() => IsStatsMode = true);
            LiveDataModeCommand = new RelayCommand(() => IsStatsMode = false);
            SetTemperatureCommand = new RelayCommand(() => { if (sampleManager != null) SetTemperaturePromptRequested(this, new SetTemperaturePromptRequestedEventArgs(SetTemperature, LatestSample.TemperatureSetpoint)); });
            SetPowerCommand = new RelayCommand(() => { if (sampleManager != null) SetPowerPromptRequested(this, new SetPowerPromptRequestedEventArgs(SetPower, LatestSample.PowerSetpoint)); });
            SetProfileCommand = new RelayCommand(() => { if (sampleManager != null) SetProfilePromptRequested(this, new SetProfilePromptRequestedEventArgs(SetProfile, LatestSample.Profile)); });
            FireCommand = new RelayCommand(() => { if (sampleManager != null) FirePromptRequested(this, new FirePromptRequestedEventArgs(Fire, Settings.Default.FireDuration)); });
            ConsoleCommand = new RelayCommand(() => { if (sampleManager != null) ConsoleRequested(this, new ConsoleRequestedEventArgs(SendConsoleCommand)); });
            SetGraphTemperatureUnit(Settings.Default.TemperatureUnitForce ? Settings.Default.TemperatureUnit : TemperatureUnit.F);
            SetGraphAxisVisibility();
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        public void SetHoveredSample(double normalizedX)
        {
            hoveredX = normalizedX;
            if (GraphData.Any() && normalizedX >= 0)
            {
                isHovered = true;
                var x = normalizedX * MaxOffset;
                if (x > GraphData.Max(kvp => kvp.Key))
                {
                    LatestSample = GraphData.Last().Value;
                }
                else
                {
                    double toTheRight = GraphData.Select(kvp => kvp.Key).FirstOrDefault(key => key >= x);
                    double toTheLeft = GraphData.Select(kvp => kvp.Key).LastOrDefault(key => key <= x);
                    LatestSample = GraphData[Math.Abs(x - toTheLeft) < Math.Abs(x - toTheRight) ? toTheLeft : toTheRight];
                }
            }
            else
            {
                isHovered = false;
            }
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
                    Invoke(OnDeviceDisconnected);
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
                        sampleManager.LastPuffStatisticsSampleCollected += LastPuffStatisticsSampleArrived;
                        sampleManager.PuffEnd += PuffEnd;
                        sampleManager.Error += Error;
                        Invoke(() => sampleManager.Paused = IsStatsMode); // Safe to set Paused property in UI thread, it just sets a bool, IsStatsMode must be read from UI thread
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
                            if (IsStatsMode) RefreshStatistics();
                            ConnectText = "Disconnect";
                            Status = string.Format("Connected to \"{0}\"", device);
                        });
                        log.Info("Connected to device \"{0}\", using native driver = {1}", device, sampleManager.UsingNativeDriver ? "Yes" : "No");
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "Error connecting to device \"{0}\"", device);
                        Invoke(() => MessageBox.Show("Error connecting to DNA device.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning));
                    }
                });
            }
        }

        private void LastPuffStatisticsSampleArrived(LastPuffStatisticsSample sample)
        {
            float batterySag = GraphData.Average(kvp => kvp.Value.BatteryVoltage) - batteryVoltages.Average();
            Temperature peakTemp = GraphData.Max(kvp => kvp.Value.Temperature);
            Invoke(() => {
                LatestStatisticSample = sample;
                CurrentPuffDuration = Settings.Default.FireDuration = sample.LastTime;
                PeakTemperature = peakTemp;
                BatterySag = batterySag;
            });
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

        private void SetTemperature(Temperature temperature)
        {
            Task.Run(() =>
            {
                if (sampleManager != null)
                {
                    sampleManager.SetTemperature(temperature);
                }
            });
        }

        private void SetPower(float watts)
        {
            Task.Run(() =>
            {
                if (sampleManager != null)
                {
                    sampleManager.SetPower(watts);
                }
            });
        }

        private void SetProfile(int profile)
        {
            Task.Run(() =>
            {
                if (sampleManager != null)
                {
                    sampleManager.SetProfile(profile);
                }
            });
        }

        private void Fire(float seconds)
        {
            Task.Run(() =>
            {
                if (sampleManager != null && seconds <= MAX_OFFSET && seconds > 0)
                {
                    sampleManager.Fire(seconds);
                }
            });
        }

        private void SendConsoleCommand(string command, Action<string> responseCallback)
        {
            if (sampleManager != null)
            {
                string response = sampleManager.SendRawCommand(command);
                responseCallback(response);
            }
        }

        private void RefreshStatistics()
        {
            if (sampleManager != null)
            {
                IsDownloadingStatistics = true;
                Task.Run(() =>
                {
                    var sample = sampleManager.GetDetailedStatisticsSample();
                    Invoke(() => { LatestDetailedStatisticsSample = sample; IsDownloadingStatistics = false; });
                });
            }
        }

        private void Error(string msg, Exception ex)
        {
            log.Error(ex, "Exception on device thread: {0}", msg);
            sampleManager.Dispose();
            sampleManager = null;
            Invoke(OnDeviceDisconnected);
        }

        private void PuffEnd()
        {
            Invoke(() => { if (IsStatsMode && Settings.Default.AutoDownloadStats) RefreshStatistics(); });
        }

        private void SampleArrived(Sample sample)
        {
            Invoke(() =>
            {
                if (!Settings.Default.TemperatureUnitForce)
                {
                    var unit = sample.BoardTemperature.Unit;
                    if (MaxTemp.Unit != unit) SetGraphTemperatureUnit(unit);
                }

                IsCharging = sample.Mode.HasFlag(Mode.Charging);

                if (!isHovered) LatestSample = sample;

                if (IsRecording && sampleRecorder != null)
                {
                    sampleRecorder.RecordSample(sample);
                }

                if (sample.Buttons.HasFlag(Buttons.Fire) || sample.Power > 0)
                {
                    if (!isFiring)
                    {
                        puffStart = sample.End;
                        ClearLiveData();
                    }
                    isFiring = true;
                    var duration = (sample.End - puffStart).TotalSeconds;
                    if (duration > 0 && sample.Temperature != null && sample.Temperature.Value > 0)
                    {
                        var isPreheating = sample.Power > sample.PowerSetpoint;
                        if (!hadPreheat)
                        {
                            if (isPreheating)
                            {
                                PreheatDuration = (float)duration;
                            }
                            else
                            {
                                hadPreheat = true;
                            }
                        }
                    }

                    GraphData.Add(duration, sample);
                    CurrentPuffDuration = duration;
                }
                else
                {
                    if (isFiring && Settings.Default.ExpandGraph) MaxOffset = GraphData.Keys.Max();
                    isFiring = false;
                    hadPreheat = false;
                    batteryVoltages.Enqueue(sample.BatteryVoltage);
                }

                SetHoveredSample(hoveredX);
            });
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SampleThrottle" && sampleManager != null)
            {
                sampleManager.Throttle = Settings.Default.SampleThrottle;
            }
            if ((e.PropertyName == "TemperatureUnitForce" || e.PropertyName == "TemperatureUnit") && Settings.Default.TemperatureUnitForce)
            {
                ClearLiveData();
                SetGraphTemperatureUnit(Settings.Default.TemperatureUnit);
            }
            if (e.PropertyName.StartsWith("Show"))
            {
                SetGraphAxisVisibility();
            }
        }

        private void SetGraphTemperatureUnit(TemperatureUnit unit)
        {
            MaxTemp = new Temperature { Unit = unit, Value = DEFAULT_MAX_TEMP.GetValue(unit) };
        }

        private void SetGraphAxisVisibility()
        {
            GraphShowAxisTemperature = Settings.Default.ShowTemperature || Settings.Default.ShowTemperatureSetpoint || Settings.Default.ShowBoardTemperature || Settings.Default.ShowRoomTemperature;
            GraphShowAxisPower = Settings.Default.ShowPower || Settings.Default.ShowPowerSetpoint;
            GraphShowAxisVoltage = Settings.Default.ShowVoltage || Settings.Default.ShowBatteryVoltage;
            GraphShowAxisCurrent = Settings.Default.ShowCurrent;
            GraphShowAxisResistance = Settings.Default.ShowLiveResistance || Settings.Default.ShowColdResistance;
            GraphShowAxisPercentage = Settings.Default.ShowBatteryLevel;
        }

        private void OnDeviceDisconnected()
        {
            CurrentDevice = null;
            LatestSample = null;
            LatestDetailedStatisticsSample = null;
            ClearLiveData();
            ConnectText = "Connect";
            Status = "Disconnected";
        }

        private void ClearLiveData()
        {
            GraphData.Clear();
            MaxOffset = MAX_OFFSET;
            PeakTemperature = default(Temperature);
            LatestStatisticSample = null;
            PreheatDuration = 0;
            BatterySag = 0;
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
