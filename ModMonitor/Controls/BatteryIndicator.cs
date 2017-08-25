using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ModMonitor.Controls
{
    internal class BatteryIndicator : FrameworkElement
    {
        #region Visual properties

        public float Level
        {
            get
            {
                return (float)GetValue(LevelProperty);
            }
            set
            {
                SetValue(LevelProperty, value);
            }
        }

        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", typeof(float), typeof(BatteryIndicator), new UIPropertyMetadata(VisualPropertyChanged));

        public bool IsCharging
        {
            get
            {
                return (bool)GetValue(IsChargingProperty);
            }
            set
            {
                SetValue(IsChargingProperty, value);
            }
        }

        public static readonly DependencyProperty IsChargingProperty = DependencyProperty.Register("IsCharging", typeof(bool), typeof(BatteryIndicator), new UIPropertyMetadata(VisualPropertyChanged));

        private static void VisualPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != args.OldValue) (sender as BatteryIndicator).InvalidateVisual();
        }

        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!RenderSize.IsEmpty && Visibility == Visibility.Visible)
            {
                Brush fill = Brushes.Lime;
                float level = Level;
                if (level < 10) fill = Brushes.Red;
                else if (level < 20) fill = Brushes.Orange;
                double fillWidth = (RenderSize.Width - 2) * (level / 100);
                drawingContext.DrawRectangle(fill, null, new Rect(0, 0, fillWidth, RenderSize.Height));
                drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Black, 1), new Rect(0, 0, RenderSize.Width - 2, RenderSize.Height));
                drawingContext.DrawRectangle(Brushes.Black, null, new Rect(RenderSize.Width - 2, ((RenderSize.Height - 4) / 2), 2, 4));
                if (IsCharging) drawingContext.DrawImage(FontAwesome.WPF.ImageAwesome.CreateImageSource(FontAwesome.WPF.FontAwesomeIcon.Bolt, Brushes.Black), new Rect((RenderSize.Width - 8) / 2, (RenderSize.Height - 8) / 2, 8, 8));
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Visibility == Visibility.Collapsed)
                return Size.Empty;
            else
                return new Size(48, 16);
        }
    }
}
