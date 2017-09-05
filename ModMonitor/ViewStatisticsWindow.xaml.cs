using Microsoft.Win32;
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
    /// Interaction logic for ViewStatisticsWindow.xaml
    /// </summary>
    public partial class ViewStatisticsWindow : Window
    {
        public ViewStatisticsWindow()
        {
            InitializeComponent();
        }

        private void ViewStatisticsViewModel_SaveFileRequested(object sender, Events.SaveFileRequestedEventArgs args)
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
