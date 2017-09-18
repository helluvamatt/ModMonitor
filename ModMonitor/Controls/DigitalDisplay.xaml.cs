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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModMonitor.Controls
{
    /// <summary>
    /// Interaction logic for DigitalDisplay.xaml
    /// </summary>
    public partial class DigitalDisplay : UserControl
    {
        #region Dependency Properties

        #region Value

        public float Value
        {
            get
            {
                return (float)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(float), typeof(DigitalDisplay));

        #endregion

        #region Unit

        public string Unit
        {
            get
            {
                return (string)GetValue(UnitProperty);
            }
            set
            {
                SetValue(UnitProperty, value);
            }
        }

        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(DigitalDisplay));

        #endregion

        #region SizeDigits

        public string SizeDigits
        {
            get
            {
                return (string)GetValue(SizeDigitsProperty);
            }
            private set
            {
                SetValue(SizeDigitsProperty, value);
            }
        }

        public static readonly DependencyProperty SizeDigitsProperty = DependencyProperty.Register("SizeDigits", typeof(string), typeof(DigitalDisplay), new UIPropertyMetadata("88888"));

        #endregion

        #region Size

        public int Size
        {
            get
            {
                return (int)GetValue(SizeProperty);
            }
            set
            {
                SetValue(SizeProperty, value);
            }
        }

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(int), typeof(DigitalDisplay), new UIPropertyMetadata(5, SizePropertyChangedHandler));

        private static void SizePropertyChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as DigitalDisplay).SizeDigits = new string('8', (int)args.NewValue);
        }

        #endregion

        #region NumberFormat

        public string NumberFormat
        {
            get
            {
                return (string)GetValue(NumberFormatProperty);
            }
            set
            {
                SetValue(NumberFormatProperty, value);
            }
        }

        public static readonly DependencyProperty NumberFormatProperty = DependencyProperty.Register("NumberFormat", typeof(string), typeof(DigitalDisplay), new UIPropertyMetadata("{0:##0.00}"));

        #endregion

        #endregion

        public DigitalDisplay()
        {
            InitializeComponent();
        }
    }
}
