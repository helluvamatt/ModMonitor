using LibDnaSerial;
using LibDnaSerial.Models;
using System;
using System.IO;

namespace ModMonitor.Models
{
    class SampleRecorder : IDisposable
    {
        private TextWriter writer;

        private object lockObject = new { };
        private bool disposed = false;

        public SampleRecorder(string filename)
        {
            writer = new StreamWriter(filename);
            writer.WriteLine(Sample.CSV_HEADER);
        }

        public void RecordSample(Sample sample)
        {
            lock (lockObject)
            {
                if (disposed) return;
                writer.WriteLine(sample.ToCsv());
            }
        }

        public void Dispose()
        {
            lock (lockObject) disposed = true;
            writer.Flush();
            writer.Close();
            writer.Dispose();
        }
    }
}
