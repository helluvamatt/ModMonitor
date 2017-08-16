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
    /// Interaction logic for SetProfileWindow.xaml
    /// </summary>
    public partial class SetProfileWindow : Window
    {
        private Action<int> _Callback;

        public SetProfileWindow(Action<int> callback, int initialValue)
        {
            InitializeComponent();
            _Callback = callback;
            inputField.Text = initialValue.ToString();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            int result;
            if (int.TryParse(inputField.Text, out result))
            {
                _Callback(result);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter a number.");
            }
        }
    }
}
