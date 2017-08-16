using LibDnaSerial;
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
    /// Interaction logic for SetTemperatureWindow.xaml
    /// </summary>
    public partial class SetTemperatureWindow : Window
    {
        public bool IsTemperatureEnabled
        {
            get
            {
                return (bool)GetValue(IsTemperatureEnabledProperty);
            }
            set
            {
                SetValue(IsTemperatureEnabledProperty, value);
            }
        }

        public static readonly DependencyProperty IsTemperatureEnabledProperty = DependencyProperty.Register("IsTemperatureEnabled", typeof(bool), typeof(SetTemperatureWindow));

        private Action<Temperature> _Callback;

        public SetTemperatureWindow(Action<Temperature> callback, Temperature initialValue)
        {
            InitializeComponent();
            _Callback = callback;
            if (initialValue != null)
            {
                IsTemperatureEnabled = true;
                valueField.Text = initialValue.Value.ToString();
                unitField.SelectedItem = initialValue.Unit;
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsTemperatureEnabled)
            {
                float result;
                if (float.TryParse(valueField.Text, out result))
                {
                    _Callback(new Temperature { Value = result, Unit = (TemperatureUnit)unitField.SelectedItem });
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Please enter a number.");
                }
            }
            else
            {
                _Callback(null);
                DialogResult = true;
                Close();
            }
        }
    }
}
