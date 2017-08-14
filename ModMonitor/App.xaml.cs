using ModMonitor.Properties;
using ModMonitor.Utils;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace ModMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILogger log;

        public App()
        {
            var productName = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
            if (bool.Parse(ConfigurationManager.AppSettings["portableMode"]))
            {
                var portableProvider = new PortableSettingsProvider(AppDomain.CurrentDomain.BaseDirectory);
                portableProvider.ApplicationName = productName;
                Settings.Default.Providers.Add(portableProvider);
                foreach (SettingsProperty prop in Settings.Default.Properties)
                {
                    prop.Provider = portableProvider;
                }
                Settings.Default.Reload();
            }
            else
            {
                LogManager.Configuration.FindTargetByName<FileTarget>("defaultFileTarget").FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), productName, "logs", "${shortdate}.log");
            }
            
            log = LogManager.GetCurrentClassLogger();
            log.Info("Application started, logging configured.");
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            log.Info("ModMonitor application startup. Arguments = \"{0}\"", string.Join(" ", e.Args));
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            log.Debug("Settings property changed: '{0}' -> '{1}'", e.PropertyName, Settings.Default[e.PropertyName]);
            Settings.Default.Save();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            log.Debug("Application shutting down, saving settings...");
            Settings.Default.Save();
        }
    }
}
