using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDnaSerial;
using System.Threading;
using System.IO.Ports;

namespace LibDnaSerial.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string comPort = WaitForDnaDevice();

            Console.WriteLine("Collecting samples... Press Enter to stop.");

            DnaSampleManager sampleManager = new DnaSampleManager(comPort, 100);
            sampleManager.SampleCollected += (sample) => { Console.WriteLine("{0:0,0.0} °{1}  {2} W  {3} V  {4} Ω", sample.Temperature.Value, sample.Temperature.Unit, sample.Power, sample.Voltage, sample.LiveResistance); };
            sampleManager.Error += (msg, ex) => { Console.WriteLine("Device connection lost."); };
            sampleManager.Connect();
            Console.ReadLine();
            sampleManager.Disconnect();

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

        static string WaitForDnaDevice()
        {
            string comPort = null;
            while (comPort == null)
            {
                foreach (string port in SerialPort.GetPortNames())
                {
                    var dev = DnaDeviceManager.CheckForDnaDevice(port);
                    if (dev != null)
                    {
                        Console.WriteLine("Found \"{0}\" on {1}", dev, port);
                        return port;
                    }
                }
                Console.WriteLine("Waiting for DNA device...");
                Thread.Sleep(5000);
            }
            return null;
        }
    }
}
