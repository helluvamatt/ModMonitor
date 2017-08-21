using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibDnaSerial.Transports
{
    internal class NativeTransport : ISerialTransport
    {
        #region Native API

        private const int ERROR_FILE_NOT_FOUND = 2;

        private const int ERROR_ACCESS_DENIED = 5;

        private const int ERROR_BROKEN_PIPE = 109;

        private const int ERROR_INVALID_HANDLE = 6;

        private const int ERROR_HANDLE_EOF = 38;

        private const int ERROR_TIMEOUT = 1460;

        [DllImport("NativeSerialConnection.dll", SetLastError = true)]
        private static extern SafeFileHandle OpenSerialPort(string portName, int readTimeout);

        [DllImport("NativeSerialConnection.dll", SetLastError = true)]
        private static extern bool CloseSerialPort(SafeFileHandle handle);

        [DllImport("NativeSerialConnection.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string ReadLine(SafeFileHandle handle);

        [DllImport("NativeSerialConnection.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern int WriteLine(SafeFileHandle handle, [MarshalAs(UnmanagedType.LPStr)] string line);

        #endregion

        private const int BUFFER_SIZE = 64;

        private SafeFileHandle mPortHandle;

        public string PortName { get; private set; }

        public NativeTransport(string portName, int readTimeout)
        {
            PortName = portName;
            mPortHandle = OpenSerialPort(portName, readTimeout);
            if (mPortHandle.IsInvalid)
            {
                int errorCode = Marshal.GetLastWin32Error();
                switch (errorCode)
                {
                    case ERROR_FILE_NOT_FOUND:
                        throw new IOException("Failed to open serial port: The specified port does not exist.");
                    case ERROR_ACCESS_DENIED:
                        throw new IOException("Failed to open serial port: The specified port is in use.");
                    default:
                        throw new IOException("Failed to open serial port: Unknown error.");
                }
            }
        }

        public void WriteLine(string line)
        {
            AssertOpen();
            // Ensure the line ends with \r\n
            line = line.TrimEnd('\r', '\n');
            line = line + "\r\n";
            WriteLine(mPortHandle, line);
        }

        public string ReadLine()
        {
            AssertOpen();
            string line = ReadLine(mPortHandle);
            if (line == null)
            {
                switch (Marshal.GetLastWin32Error())
                {
                    case ERROR_BROKEN_PIPE:
                    case ERROR_HANDLE_EOF:
                    case ERROR_INVALID_HANDLE:
                        throw new IOException("Port is closed.");
                    case ERROR_TIMEOUT:
                        throw new TimeoutException("The read operation timed out.");
                    default:
                        throw new IOException("Unknown error on serial port read.");
                }
            }
            return line.TrimEnd('\r', '\n');
        }

        ~NativeTransport()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (mPortHandle != null && !mPortHandle.IsClosed && !mPortHandle.IsInvalid) mPortHandle.Dispose();
        }

        private void AssertOpen()
        {
            if (mPortHandle == null || mPortHandle.IsClosed || mPortHandle.IsInvalid) throw new InvalidOperationException("Port is closed.");
        }
    }
}
