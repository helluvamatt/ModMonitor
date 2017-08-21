using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDnaSerial;
using System.Threading;
using System.IO.Ports;
using LibDnaSerial.Models;
using System.Diagnostics;

namespace LibDnaSerial.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing for serial port issues...");

            using (DnaConnection conn = new DnaConnection("COM3"))
            {
                Console.WriteLine(conn.GetSerialNumber());
            }

            if (Debugger.IsAttached) Debugger.Break();

            // Disabled this code, samples coming through the even handler are not queued
            /*
            Console.WriteLine("Processing samples...");

            double? minSampleTime = null;
            double? maxSampleTime = null;
            double totalSampleTime = 0;
            List<Sample> samples = new List<Sample>();
            do
            {
                Sample sample = sampleManager.GetSample();
                if (sample != null)
                {
                    var sampleTime = (sample.End - sample.Begin).TotalMilliseconds;
                    if (!maxSampleTime.HasValue || sampleTime > maxSampleTime.Value) maxSampleTime = sampleTime;
                    if (!minSampleTime.HasValue || sampleTime < minSampleTime.Value) minSampleTime = sampleTime;
                    totalSampleTime += sampleTime;
                    samples.Add(sample);
                }
                else break;
            }
            while (true);

            double averageSampleTime = totalSampleTime / samples.Count;

            Console.WriteLine("{0} samples collected", samples.Count);
            Console.WriteLine("  Average sample time: {0} ms", averageSampleTime);
            Console.WriteLine("  Min sample time:     {0} ms", minSampleTime);
            Console.WriteLine("  Max sample time:     {0} ms", maxSampleTime);

            Console.WriteLine("Press enter to finish...");
            Console.ReadLine();
            */
        }

        static DnaDevice WaitForDnaDevice()
        {
            Console.WriteLine("Waiting for DNA device...");
            string comPort = null;
            while (comPort == null)
            {
                var devices = DnaDeviceManager.ListDnaDevices();
                if (devices.Any())
                {
                    return devices[0];
                }
                Thread.Sleep(500);
            }
            return null;
        }
    }
}
