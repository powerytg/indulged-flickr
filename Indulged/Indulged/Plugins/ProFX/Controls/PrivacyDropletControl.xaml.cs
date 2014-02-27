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
    public partial class PrivacyDropletControl : UserControl
    {
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(PrivacyDropletControl), new PropertyMetadata(OnSelectedPropertyChanged));

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
            ((PrivacyDropletControl)sender).OnSelectedChanged();
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

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PrivacyDropletControl), new PropertyMetadata(OnTitlePropertyChanged));

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static void OnTitlePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((PrivacyDropletControl)sender).OnTitleChanged();
        }

        protected virtual void OnTitleChanged()
        {
            Label.Text = Title;
        }

        // Constructor
        public PrivacyDropletControl()
        {
            InitializeComponent();
            Selected = true;
        }

        private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Selected = !Selected;
        }
    }
}
