using LibDnaSerial;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DnaDeviceMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DataContext is IDisposable)
            {
                (DataContext as IDisposable).Dispose();
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MainViewModel_SaveFileRequested(object sender, Events.SaveFileRequestedEventArgs args)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = args.Title;
            dialog.InitialDirectory = args.InitialDirectory;
            dialog.Filter = args.Filters;
            if (dialog.ShowDialog() == true)
            {
                args.Callback(dialog.FileName);
            }
        }
    }
}
