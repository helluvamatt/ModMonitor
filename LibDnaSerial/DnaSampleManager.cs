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
        /// EventHandler for when a sample is collected
        /// </summary>
        /// <param name="sample">Sample</param>
        public delegate void SampleCollectedEventHandler(Sample sample);

        /// <summary>
        /// Event for when a sample has been collected
        /// </summary>
        public event SampleCollectedEventHandler SampleCollected;

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
                    Sample s = dnaConnection.GetSample();
                    if (SampleCollected != null)
                    {
                        SampleCollected.Invoke(s);
                    }
                    else
                    {
                        sampleQueue.Enqueue(s);
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
