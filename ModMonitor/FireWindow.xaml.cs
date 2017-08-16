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
    /// Interaction logic for FireWindow.xaml
    /// </summary>
    public partial class FireWindow : Window
    {
        private Action<float> _Callback;

        public FireWindow(Action<float> callback, float initialValue)
        {
            InitializeComponent();
            _Callback = callback;
            inputField.Text = initialValue.ToString();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            float result;
            if (float.TryParse(inputField.Text, out result))
            {
                _Callback(result);
                DialogResult = true;
                Close();
            }
        }
    }
}
