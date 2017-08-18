using LibDnaSerial.Models;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LibDnaSerial
{
    /// <summary>
    /// Uses a separate thread to collect samples in a queue that can be processed on another thread
    /// </summary>
    /// <remarks>
    /// Thread safe: public methods can be called on any thread
    /// </remarks>
    public class DnaSampleManager : IDisposable
    {
        private DnaConnection dnaConnection;

        private bool running;
        private Thread runThread;
        private object lockObject = new { };
        private object sampleLockObject = new { };
        private bool isFiring = false;

        private ConcurrentQueue<Sample> sampleQueue;

        private string _SerialPort;

        /// <summary>
        /// Serial port for the DNA board (e.g. "COM3")
        /// </summary>
        public string SerialPort
        {
            get
            {
                return _SerialPort;
            }
            set
            {
                bool wasConnected = running;
                if (wasConnected) Disconnect();
                _SerialPort = value;
                if (wasConnected) Connect();
            }
        }

        /// <summary>
        /// Delay in milliseconds between sample collections
        /// </summary>
        public long Throttle { get; set; }

        /// <summary>
        /// Pause sending samples, but stay connected
        /// </summary>
        /// <remarks>Set this to true to stop sending samples, but keep the connection open. You can use this to pause sending samples while you are requesting (or about to request) statistics samples.</remarks>
        public bool Paused { get; set; }

        /// <summary>
        /// C'tor specifying a SerialPort property value and a Throttle property value
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="throttle"></param>
        public DnaSampleManager(string serialPort, long throttle) : this(serialPort)
        {
            Throttle = throttle;
        }

        /// <summary>
        /// C'tor specifying a ComPort property value
        /// </summary>
        /// <param name="serialPort">SerialPort to connect to</param>
        public DnaSampleManager(string serialPort) : this()
        {
            SerialPort = serialPort;
        }

        /// <summary>
        /// C'tor
        /// </summary>
        public DnaSampleManager()
        {
            sampleQueue = new ConcurrentQueue<Sample>();
        }

        /// <summary>
        /// Connect to the DNA board and start collecting samples in the queue
        /// 
        /// Does nothing if already connected
        /// </summary>
        public void Connect()
        {
            if (!running)
            {
                lock (lockObject)
                {
                    dnaConnection = new DnaConnection(SerialPort);
                    running = true;
                    runThread = new Thread(RequestThreadRunner);
                    runThread.Start();
                }
            }
        }

        /// <summary>
        /// Stop collecting samples and disconnect from the DNA board
        /// 
        /// Does nothing if not connected
        /// </summary>
        public void Disconnect()
        {
            if (running)
            {
                lock (lockObject)
                {
                    running = false;
                    runThread.Join();
                    dnaConnection.Dispose();
                    dnaConnection = null;
                }
            }
        }

        /// <summary>
        /// Return a sample from the sample queue, returns null when no more samples are available
        /// </summary>
        /// <returns>Earliest sample available in the queue</returns>
        public Sample GetSample()
        {
            Sample s;
            if (!sampleQueue.TryDequeue(out s)) return null;
            return s;
        }

        /// <summary>
        /// Get a statistics sample containing only the data for the last puff
        /// </summary>
        /// <returns>Last puff statistics sample</returns>
        public LastPuffStatisticsSample GetLastPuffStatistics()
        {
            lock (sampleLockObject)
            {
                if (dnaConnection != null) return dnaConnection.GetLastPuffStatisticsSample();
                return null;
            }
        }

        /// <summary>
        /// Get the basic statistics
        /// </summary>
        /// <returns>Basic statistics sample</returns>
        public BasicStatisticsSample GetBasicStatisticsSample()
        {
            lock (sampleLockObject)
            {
                if (dnaConnection != null) return dnaConnection.GetBasicStatisticsSample();
                return null;
            }
        }

        /// <summary>
        /// Get the full detailed statistics
        /// </summary>
        /// <returns>Detailed statistics sample</returns>
        public DetailedStatisticsSample GetDetailedStatisticsSample()
        {
            lock (sampleLockObject)
            {
                if (dnaConnection != null) return dnaConnection.GetDetailedStatisticsSample();
                return null;
            }
        }

        /// <summary>
        /// Set the currently selected device profile
        /// </summary>
        /// <param name="profile">Profile index, profile numbers start at 1, typically there are 8 profiles</param>
        /// <see cref="DnaConnection.SetProfile(int)"/>
        public void SetProfile(int profile)
        {
            lock (sampleLockObject)
            {
                if (dnaConnection != null) dnaConnection.SetProfile(profile);
            }
        }

        /// <summary>
        /// Set the temperature limit to the specified temperature
        /// </summary>
        /// <param name="temperature">Temperature setpoint</param>
        /// <see cref="DnaConnection.SetTemperature(Temperature)"/>
        public void SetTemperature(Temperature temperature)
        {
            lock (sampleLockObject)
            {
                if (dnaConnection != null) dnaConnection.SetTemperature(temperature);
            }
        }

        /// <summary>
        /// Set the power limit setpoint to the specified wattage
        /// </summary>
        /// <param name="watts">Power setpoint in watts</param>
        /// <see cref="DnaConnection.SetPower(float)"/>
        public void SetPower(float watts)
        {
            lock (sampleLockObject)
            {
                if (dnaConnection != null) dnaConnection.SetPower(watts);
            }
        }

        /// <summary>
        /// Fire a puff for the specified duration
        /// </summary>
        /// <param name="seconds">Duration of fire in seconds</param>
        /// <see cref="DnaConnection.Fire(float)"/>
        public void Fire(float seconds)
        {
            lock (sampleLockObject)
            {
                if (dnaConnection != null) dnaConnection.Fire(seconds);
            }
        }

        /// <summary>
        /// EventHandler for when a sample is collected
        /// </summary>
        /// <param name="sample">Sample</param>
        public delegate void SampleCollectedEventHandler(Sample sample);

        /// <summary>
        /// Event for when a sample has been collected
        /// </summary>
        public event SampleCollectedEventHandler SampleCollected;

        /// <summary>
        /// Event handler for when a LastPuffStatisticsSample is collected
        /// </summary>
        /// <param name="sample"></param>
        public delegate void LastPuffStatisticsCollectedEventHandler(LastPuffStatisticsSample sample);

        /// <summary>
        /// Event for when a last puff statistics sample has been collected
        /// </summary>
        /// <remarks>
        /// Consume this event if you want to automatically receive a LastPuffStatistics when the library detects a puff has completed. The library uses the Buttons field on the sample to determine when the fire button is released, and will automatically query for last puff statistics and send them through this handler.
        /// </remarks>
        public event LastPuffStatisticsCollectedEventHandler LastPuffStatisticsSampleCollected;

        /// <summary>
        /// Event for when a puff has begun
        /// </summary>
        public event Action PuffBegin;

        /// <summary>
        /// Event for when a puff has ended
        /// </summary>
        public event Action PuffEnd;

        /// <summary>
        /// EventHandler for when an error occurs
        /// </summary>
        /// <param name="msg">Error message</param>
        /// <param name="ex">Exception, if present</param>
        public delegate void ErrorEventHandler(string msg, Exception ex);

        /// <summary>
        /// Event for when an error occurs
        /// </summary>
        public event ErrorEventHandler Error;

        public void Dispose()
        {
            Disconnect();
        }

        private void RequestThreadRunner()
        {
            try
            {
                while (running)
                {
                    DateTime start = DateTime.Now;
                    if (!Paused)
                    {
                        Sample s;
                        lock (sampleLockObject)
                        {
                            s = dnaConnection.GetSample();
                        }
                        if (SampleCollected != null)
                        {
                            SampleCollected.Invoke(s);
                        }
                        else
                        {
                            sampleQueue.Enqueue(s);
                        }
                        var sampleIsFiring = s.Buttons.HasFlag(Buttons.Fire) || s.Power > 0;
                        if (!sampleIsFiring && isFiring)
                        {
                            PuffEnd?.Invoke();
                            if (LastPuffStatisticsSampleCollected != null)
                            {
                                LastPuffStatisticsSample lastPuffSample;
                                lock (sampleLockObject)
                                {
                                    lastPuffSample = dnaConnection.GetLastPuffStatisticsSample();
                                }
                                LastPuffStatisticsSampleCollected.Invoke(lastPuffSample);
                            }
                        }
                        if (sampleIsFiring && !isFiring)
                        {
                            PuffBegin?.Invoke();
                        }
                        isFiring = sampleIsFiring;
                    }
                    else
                    {
                        Buttons buttons;
                        float power = 0;
                        lock (sampleLockObject)
                        {
                            buttons = dnaConnection.GetButtons();
                            power = dnaConnection.GetPower();
                        }
                        var sampleIsFiring = buttons.HasFlag(Buttons.Fire) || power > 0;
                        if (!sampleIsFiring && isFiring)
                        {
                            PuffEnd?.Invoke();
                        }
                        if (sampleIsFiring && !isFiring)
                        {
                            PuffBegin?.Invoke();
                        }
                        isFiring = sampleIsFiring;
                    }
                    long millis = (DateTime.Now - start).Ticks / TimeSpan.TicksPerMillisecond;
                    if (Throttle > millis)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(Throttle - millis));
                    }
                }

            }
            catch (Exception ex)
            {
                lock (lockObject)
                {
                    running = false;
                    try
                    {
                        dnaConnection.Dispose();
                    }
                    catch { } // Ignore, it's already gone
                    dnaConnection = null;
                }
                Error?.Invoke("A device error occurred.", ex);
            }
        }
    }
}
