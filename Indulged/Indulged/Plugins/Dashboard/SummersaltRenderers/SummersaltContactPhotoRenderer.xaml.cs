using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using System.Windows.Media.Imaging;
using Indulged.API.Cinderella.Models;
using System.Windows.Documents;

namespace Indulged.Plugins.Dashboard.SummersaltRenderers
{
    public partial class SummersaltContactPhotoRenderer : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(SummersaltContactPhotoRenderer), new PropertyMetadata(OnPhotoSourcePropertyChanged));

        public Photo PhotoSource
        {
            get
            {
                return (Photo)GetValue(PhotoSourceProperty);
            }
            set
            {
                SetValue(PhotoSourceProperty, value);
            }
        }

        public static void OnPhotoSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SummersaltContactPhotoRenderer)sender).OnPhotoSourceChanged();
        }

        protected virtual void OnPhotoSourceChanged()
        {
            ImageView.Source = new BitmapImage { UriSource = new Uri(PhotoSource.GetImageUrl()), DecodePixelWidth = 400 };

            if (PhotoSource.Title != null && PhotoSource.Title.Length > 0)
            {
                TitleLabel.Text = PhotoSource.Title;
                TitleLabel.Visibility = Visibility.Visible;
            }
            else
            {
                TitleLabel.Text = "Untitled";
                TitleLabel.Visibility = Visibility.Visible;
            }

            // User label
            User photoOwner = Cinderella.CinderellaCore.UserCache[PhotoSource.UserId];
            UserLabel.Text = "By " + photoOwner.Name;

            if (PhotoSource.Description != null && PhotoSource.Description.Length > 0)
            {
                DescriptionLabel.Text = PhotoSource.Description;
                DescriptionLabel.Visibility = Visibility.Visible;
            }
            else
                DescriptionLabel.Visibility = Visibility.Collapsed;
        }

        // Constructor
        public SummersaltContactPhotoRenderer()
        {
            InitializeComponent();
        }

        private void UserLabel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
            currentPage.NavigationService.Navigate(new Uri("/Plugins/Profile/UserProfilePage.xaml?user_id=" + PhotoSource.UserId, UriKind.Relative));

        }


    }
}
