using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModMonitor
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void EditSettingsViewModel_EditColor(object sender, Events.EditColorEventArgs args)
        {
            ColorDialog cd = new ColorDialog();
            cd.FullOpen = true;
            cd.Color = args.InitialSetting;
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                args.Callback(args.ColorSettingName, cd.Color);
            }
        }
    }
}
