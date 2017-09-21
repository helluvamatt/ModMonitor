using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        public float MinLevel
        {
            get
            {
                return (float)GetValue(MinLevelProperty);
            }
            set
            {
                SetValue(MinLevelProperty, value);
            }
        }

        public static readonly DependencyProperty MinLevelProperty = DependencyProperty.Register("MinLevel", typeof(float), typeof(BatteryIndicator), new UIPropertyMetadata(0f, VisualPropertyChanged));

        public float MaxLevel
        {
            get
            {
                return (float)GetValue(MaxLevelProperty);
            }
            set
            {
                SetValue(MaxLevelProperty, value);
            }
        }

        public static readonly DependencyProperty MaxLevelProperty = DependencyProperty.Register("MaxLevel", typeof(float), typeof(BatteryIndicator), new UIPropertyMetadata(100f, VisualPropertyChanged));

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

        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(BatteryIndicator), new UIPropertyMetadata(Orientation.Horizontal, VisualPropertyChanged));

        public float WarningLevel
        {
            get
            {
                return (float)GetValue(WarningLevelProperty);
            }
            set
            {
                SetValue(WarningLevelProperty, value);
            }
        }

        public static readonly DependencyProperty WarningLevelProperty = DependencyProperty.Register("WarningLevel", typeof(float), typeof(BatteryIndicator), new UIPropertyMetadata(20f, VisualPropertyChanged));

        public float CriticalLevel
        {
            get
            {
                return (float)GetValue(CriticalLevelProperty);
            }
            set
            {
                SetValue(CriticalLevelProperty, value);
            }
        }

        public static readonly DependencyProperty CriticalLevelProperty = DependencyProperty.Register("CriticalLevel", typeof(float), typeof(BatteryIndicator), new UIPropertyMetadata(10f, VisualPropertyChanged));

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
                if (Level <= CriticalLevel) fill = Brushes.Red;
                else if (Level <= WarningLevel) fill = Brushes.Orange;
                if (Orientation == Orientation.Horizontal)
                {
                    double fillWidth = (RenderSize.Width - 2) * Map(MinLevel, MaxLevel, Level);
                    drawingContext.DrawRectangle(fill, null, new Rect(0, 0, fillWidth, RenderSize.Height));
                    drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Black, 1), new Rect(0, 0, RenderSize.Width - 2, RenderSize.Height));
                    drawingContext.DrawRectangle(Brushes.Black, null, new Rect(RenderSize.Width - 2, ((RenderSize.Height - 4) / 2), 2, 4));
                }
                else
                {
                    double fillHeight = (RenderSize.Height - 6) * Map(MinLevel, MaxLevel, Level);
                    drawingContext.DrawRectangle(fill, null, new Rect(0, RenderSize.Height - fillHeight, RenderSize.Width, fillHeight));
                    drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Black, 1), new Rect(0, 6, RenderSize.Width, RenderSize.Height - 6));
                    drawingContext.DrawRectangle(Brushes.Black, null, new Rect(((RenderSize.Width - 12) / 2), 0, 12, 6));
                }
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

        // Maps a value in a range to a value in the range 0-1
        private float Map(float min, float max, float value)
        {
            if (max <= min) throw new ArgumentOutOfRangeException("MinLevel must be less than MaxLevel.");
            if (value < min || value > max) throw new ArgumentOutOfRangeException("Level is out of range of MinLevel and MaxLevel.");
            return ((value - min) / (max - min));
        }
    }
}
