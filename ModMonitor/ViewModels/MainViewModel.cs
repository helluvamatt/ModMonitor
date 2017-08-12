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

        #region GraphData

        public ObservableDictionary<DateTime, Sample> GraphData { get; private set; }

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

        public event SaveFileRequestedEvent SaveFileRequested = (sender, args) => { };

        public event DevicePickerRequestedEvent DevicePickerRequested = (sender, args) => { };

        public event EventHandler EditSettingsRequested = (sender, args) => { };

        public event EventHandler ShowAboutRequested = (sender, args) => { };

        private DnaSampleManager sampleManager = null;
        private SampleRecorder sampleRecorder = null;

        private ICommand connectCommand;
        private ICommand startRecordingCommand;
        private ICommand stopRecordingCommand;
        private ICommand editSettingsCommand;
        private ICommand showAboutCommand;

        public MainViewModel()
        {
            GraphData = new ObservableDictionary<DateTime, Sample>();
            connectCommand = new RelayCommand(Connect);
            startRecordingCommand = new RelayCommand(StartRecording, () => !IsRecording);
            stopRecordingCommand = new RelayCommand(StopRecording, () => IsRecording);
            editSettingsCommand = new RelayCommand(EditSettings);
            showAboutCommand = new RelayCommand(ShowAbout);
            MinTime = DateTime.Now;
            MaxTime = DateTime.Now + TimeSpan.FromSeconds(30);
            SetGraphTemperatureUnit(Settings.Default.TemperatureUnitForce ? Settings.Default.TemperatureUnit : TemperatureUnit.F);
        }

        private void Connect()
        {
            if (sampleManager != null)
            {
                // "Disconnect"
                Task.Run(() =>
                {
                    sampleManager.Dispose();
                    sampleManager = null;
                    Invoke(() =>
                    {
                        LatestSample = null;
                        ConnectText = "Connect";
                        Status = "Disconnected";
                        GraphData.Clear();
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
                });
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
            Console.Error.WriteLine("Error occurred in device thread: {0}", ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
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
                if (!IsGraphPaused)
                {
                    MaxTime = sample.End;
                    MinTime = sample.End - TimeSpan.FromSeconds(30);
                    if (sample.Index % Settings.Default.GraphResolution == 0)
                    {
                        GraphData.RemoveAll(k => k < MinTime);
                        GraphData.Add(sample.End, sample);
                    }
                }

            });
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
