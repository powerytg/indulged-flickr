using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.Plugins.ProFX.Filters;
using System.Windows.Media;

namespace Indulged.Plugins.ProFX.Controls
{
    public partial class FilterDropletControl : UserControl
    {
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(FilterDropletControl), new PropertyMetadata(OnSelectedPropertyChanged));

        private static SolidColorBrush normalBackgroundBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x1d, 0x27, 0x33));
        private static SolidColorBrush normalStroke = new SolidColorBrush(Color.FromArgb(0x00, 0x02, 0xb9, 0xa2));
        private static SolidColorBrush selectedBackgroundBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x01, 0x40, 0x3b));
        private static SolidColorBrush selectedStroke = new SolidColorBrush(Color.FromArgb(0xff, 0x02, 0xb9, 0xa2));

        public bool Selected
        {
            get
            {
                return (bool)GetValue(SelectedProperty);
            }
            set
            {
                SetValue(SelectedProperty, value);
            }
        }

        public static void OnSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((FilterDropletControl)sender).OnSelectedChanged();
        }

        protected virtual void OnSelectedChanged()
        {
            if (Selected)
            {
                Icon.Visibility = Visibility.Visible;
                BackgroundBorder.Background = selectedBackgroundBrush;
                BackgroundBorder.BorderBrush = selectedStroke;
            }
            else
            {
                Icon.Visibility = Visibility.Collapsed;
                BackgroundBorder.Background = normalBackgroundBrush;
                BackgroundBorder.BorderBrush = normalStroke;
            }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(FilterBase), typeof(FilterDropletControl), new PropertyMetadata(OnFilterPropertyChanged));

        public FilterBase Filter
        {
            get
            {
                return (FilterBase)GetValue(FilterProperty);
            }
            set
            {
                SetValue(FilterProperty, value);
            }
        }

        public static void OnFilterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((FilterDropletControl)sender).OnFilterChanged();
        }

        protected virtual void OnFilterChanged()
        {
            Label.Text = Filter.DisplayName;
        }

        // Constructor
        public FilterDropletControl()
        {
            InitializeComponent();
        }
    }
}
