using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.API.Utils;
using System.Windows.Media;

namespace Indulged.Plugins.Common.Renderers
{
    public partial class ShortTimestampView : UserControl
    {
        // ThemeColor source
        public static readonly DependencyProperty ThemeColorProperty = DependencyProperty.Register("ThemeColor", typeof(Color), typeof(ShortTimestampView), new PropertyMetadata(OnThemeColorPropertyChanged));

        public Color ThemeColor
        {
            get
            {
                return (Color)GetValue(ThemeColorProperty);
            }
            set
            {
                SetValue(ThemeColorProperty, value);
            }
        }

        public static void OnThemeColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ShortTimestampView)sender).OnThemeColorChanged();
        }

        protected void OnThemeColorChanged()
        {
            LayoutRoot.Background = new SolidColorBrush(ThemeColor);
        }

        // Timestamp source
        public static readonly DependencyProperty TimestampProperty = DependencyProperty.Register("Timestamp", typeof(DateTime), typeof(ShortTimestampView), new PropertyMetadata(OnTimestampPropertyChanged));

        public DateTime Timestamp
        {
            get
            {
                return (DateTime)GetValue(TimestampProperty);
            }
            set
            {
                SetValue(TimestampProperty, value);
            }
        }

        public static void OnTimestampPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ShortTimestampView)sender).OnTimestampChanged();
        }

        protected void OnTimestampChanged()
        {
            string displayText = Timestamp.ToTimestampString();
            if (displayText.Length >= 5)
                TimestampLabel.FontSize = 20;
            else
                TimestampLabel.FontSize = 28;

            TimestampLabel.Text = Timestamp.ToTimestampString();
        }

        // Constructor
        public ShortTimestampView()
        {
            InitializeComponent();
        }
    }
}
