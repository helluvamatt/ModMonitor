using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModMonitor
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        private Action<string, Action<string>> callback;

        private List<string> commandHistory;
        private int historyIndex;

        public ConsoleWindow(Action<string, Action<string>> cb)
        {
            InitializeComponent();
            callback = cb;
            commandHistory = new List<string>();
            commandHistory.Add("");
            historyIndex = 0;
            consoleInputTextBox.Focus();
        }

        private void consoleInputTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                string line = consoleInputTextBox.Text;
                consoleInputTextBox.Text = "";
                commandHistory.Add("");
                HandleInputLine(line);
            }
            else if (e.Key == Key.Up)
            {
                if (historyIndex > 0) historyIndex--;
                consoleInputTextBox.Text = commandHistory[historyIndex];
                consoleInputTextBox.Select(consoleInputTextBox.Text.Length, 0);
            }
            else if (e.Key == Key.Down)
            {
                if (historyIndex < commandHistory.Count - 1) historyIndex++;
                consoleInputTextBox.Text = commandHistory[historyIndex];
                consoleInputTextBox.Select(consoleInputTextBox.Text.Length, 0);
            }
            else
            {
                commandHistory[commandHistory.Count - 1] = consoleInputTextBox.Text;
            }
        }

        private void consoleInputTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (text.Contains("\r") || text.Contains("\n"))
                {
                    e.CancelCommand();
                    Task.Run(() =>
                    {
                        string[] lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < lines.Length - 1; i++)
                        {
                            string line = lines[i];
                            HandleInputLine(line);
                        }
                        ResponseArrived(lines[lines.Length - 1]);
                    });
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void HandleInputLine(string line)
        {
            commandHistory.Add(line);
            ResponseArrived(line);
            callback(line, ResponseArrived);
        }

        private void ResponseArrived(string response)
        {
            if (response != null)
            {
                Invoke(() =>
                {
                    consoleOutputTextBox.Text += response + "\r\n";
                    consoleOutputTextBox.ScrollToEnd();
                });
            }
        }

        private void Invoke(Action action)
        {
            if (!Dispatcher.HasShutdownStarted)
            {
                Dispatcher.Invoke(action);
            }
        }
    }
}
