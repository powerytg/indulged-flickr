using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.API.Cinderella.Models;
using System.Windows.Media;

namespace Indulged.Plugins.Common.Renderers
{
    public partial class WeightedTagButton : UserControl
    {
        public static readonly DependencyProperty TagSourceProperty = DependencyProperty.Register("TagSource", typeof(PhotoTag), typeof(WeightedTagButton), new PropertyMetadata(OnTagPropertyChanged));

        public PhotoTag TagSource
        {
            get
            {
                return (PhotoTag)GetValue(TagSourceProperty);
            }
            set
            {
                SetValue(TagSourceProperty, value);
            }
        }

        public static void OnTagPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((WeightedTagButton)sender).OnTagChanged();
        }

        protected virtual void OnTagChanged()
        {
            TagButton.Content = "#" + TagSource.Name;
            BadgeLabel.Text = TagSource.Weight.ToString();
            if (TagSource.Weight >= 95)
            {
                BadgeIcon.Fill = new SolidColorBrush(Color.FromArgb(0xff, 0x33, 0x4d, 0x1a));
            }
            else if (TagSource.Weight >= 90 && TagSource.Weight < 95)
            {
                BadgeIcon.Fill = new SolidColorBrush(Color.FromArgb(0xff, 0x34, 0x1a, 0x4d));
            }
            else if (TagSource.Weight >= 85 && TagSource.Weight < 90)
            {
                BadgeIcon.Fill = new SolidColorBrush(Color.FromArgb(0xff, 0x4d, 0x1a, 0x44));
            }
            else
            {
                BadgeIcon.Fill = new SolidColorBrush(Color.FromArgb(0xff, 0x35, 0x4d, 0x1a));
            }

        }

        // Constructor
        public WeightedTagButton()
        {
            InitializeComponent();
        }

        private void OnTagButtonClick(object sender, RoutedEventArgs e)
        {
            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;

            // Get photo collection context
            currentPage.NavigationService.Navigate(new Uri("/Plugins/Search/SearchResultPage.xaml?tags=" + TagSource.Name, UriKind.Relative));

        }
    }
}
