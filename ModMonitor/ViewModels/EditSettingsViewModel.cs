using ModMonitor.Events;
using ModMonitor.Properties;
using MvvmFoundation.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModMonitor.ViewModels
{
    public class EditSettingsViewModel : DependencyObject
    {
        private ICommand editColorCommand;
        private ICommand resetCommand;

        public event EditColorEventHandler EditColor = (sender, args) => { };

        public ICommand EditColorCommand
        {
            get
            {
                return editColorCommand;
            }
        }

        public ICommand ResetCommand
        {
            get
            {
                return resetCommand;
            }
        }

        public EditSettingsViewModel()
        {
            editColorCommand = new RelayCommand<string>(DoEditColor);
            resetCommand = new RelayCommand(ResetSettings);
        }

        private void DoEditColor(string param)
        {
            EditColor(this, new EditColorEventArgs(SetColor) { ColorSettingName = param, InitialSetting = (Color)Settings.Default[param] });
        }

        private void SetColor(string name, Color c)
        {
            Settings.Default[name] = c;
        }

        private void ResetSettings()
        {
            Settings.Default.Reset();
        }
    }
}
