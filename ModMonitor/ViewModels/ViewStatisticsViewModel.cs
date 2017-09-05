using ModMonitor.Events;
using ModMonitor.Models;
using ModMonitor.Utils;
using MvvmFoundation.Wpf;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModMonitor.ViewModels
{
    class ViewStatisticsViewModel : DependencyObject
    {
        #region Dependency Properties

        #region StatisticsData

        public ObservableCollection<Statistics> StatisticsData { get; private set; }

        #endregion

        #region IsLoading

        public bool IsLoading
        {
            get
            {
                return (bool)GetValue(IsLoadingProperty);
            }
            set
            {
                SetValue(IsLoadingProperty, value);
            }
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(ViewStatisticsViewModel));

        #endregion

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; private set; }

        public ICommand ExportCommand { get; private set; }

        #endregion

        #region Events

        public event SaveFileRequestedEvent SaveFileRequested;

        #endregion

        private readonly ILogger log;

        public ViewStatisticsViewModel()
        {
            log = LogManager.GetCurrentClassLogger();
            StatisticsData = new ObservableCollection<Statistics>();
            RefreshCommand = new RelayCommand(Refresh);
            ExportCommand = new RelayCommand(Export);
            Refresh();
        }

        private void Refresh()
        {
            IsLoading = true;
            StatisticsData.Clear();
            Task.Run(() =>
            {
                try
                {
                    using (var db = StatisticsDatabase.Open())
                    {
                        foreach (var record in db.Statistics.OrderBy(r => r.Timestamp))
                        {
                            Invoke(() => StatisticsData.Add(record));
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Failed to load statistics database.");
                    Invoke(() => MessageBox.Show(string.Format("Failed to load statistics database: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error));
                }
                Invoke(() => IsLoading = false);
            });
        }

        private void Export()
        {
            var args = new SaveFileRequestedEventArgs(new Action<string>(Export));
            args.Filters = "CSV files|*.csv|All files|*.*";
            args.Title = "Save CSV file...";
            SaveFileRequested(this, args);
        }

        private void Export(string filename)
        {
            IsLoading = true;
            Task.Run(() =>
            {
                try
                {
                    using (var output = new StreamWriter(File.OpenWrite(filename)))
                    {
                        using (var db = StatisticsDatabase.Open())
                        {
                            output.WriteLine(CsvUtils.GetCsvHeader(typeof(Statistics)));
                            foreach (var record in db.Statistics.OrderBy(r => r.Timestamp))
                            {
                                output.WriteLine(CsvUtils.GetCsv(record));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Failed to export statistics.");
                    Invoke(() => MessageBox.Show(string.Format("Failed to export statistics: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error));
                }
                Invoke(() => IsLoading = false);
            });
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
