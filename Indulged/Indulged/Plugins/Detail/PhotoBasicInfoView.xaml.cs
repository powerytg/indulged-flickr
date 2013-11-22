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

using Indulged.PolKit;
using Microsoft.Phone.Tasks;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using System.Windows.Media.Imaging;
using Indulged.Resources;

namespace Indulged.Plugins.Detail
{
    public partial class PhotoBasicInfoView : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(PhotoBasicInfoView), new PropertyMetadata(OnPhotoSourcePropertyChanged));

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
            ((PhotoBasicInfoView)sender).OnPhotoSourceChanged();
        }

        protected virtual void OnPhotoSourceChanged()
        {
            // Image view
            ImageView.Source = new BitmapImage { UriSource = new Uri(PhotoSource.GetImageUrl()), DecodePixelWidth = 480 };

            // User
            if (PhotoSource.UserId == null || !Cinderella.CinderellaCore.UserCache.ContainsKey(PhotoSource.UserId))
                UserRenderer.UserSource = null;
            else
            {
                User photoOwner = Cinderella.CinderellaCore.UserCache[PhotoSource.UserId];
                UserRenderer.UserSource = photoOwner;
            }

            // Favourite icon
            if (PhotoSource.IsFavourite)
                FavIconView.Visibility = Visibility.Visible;
            else
                FavIconView.Visibility = Visibility.Collapsed;

            StatView.PhotoSource = PhotoSource;

            // Title label
            if (PhotoSource.Title.Length > 0)
                TitleLabel.Text = PhotoSource.Title;
            else
                TitleLabel.Text = AppResources.GenericUntitledText;

            // Description
            if (PhotoSource.Description != null && PhotoSource.Description.Length > 0)
            {
                DescriptionLabel.Text = PhotoSource.Description;
                DescriptionLabel.Visibility = Visibility.Visible;
            }
            else
                DescriptionLabel.Visibility = Visibility.Collapsed;

            // License
            if (PhotoSource.LicenseId == null)
                LicenseButton.Content = "Unknown License";
            else
            {
                License license = PolicyKit.CurrentPolicy.Licenses[PhotoSource.LicenseId];
                LicenseButton.Content = license.Name;
            }

            
        }

        public PhotoBasicInfoView()
        {
            InitializeComponent();

            // Events
            Cinderella.CinderellaCore.PhotoInfoUpdated += OnPhotoInfoUpdated;
            Cinderella.CinderellaCore.PhotoAddedAsFavourite += OnAddedAsFavourite;
            Cinderella.CinderellaCore.PhotoRemovedFromFavourite += OnRemovedFromFavourite;

        }

        public void RemoveEventListeners()
        {
            Cinderella.CinderellaCore.PhotoInfoUpdated -= OnPhotoInfoUpdated;
            Cinderella.CinderellaCore.PhotoAddedAsFavourite -= OnAddedAsFavourite;
            Cinderella.CinderellaCore.PhotoRemovedFromFavourite -= OnRemovedFromFavourite;

        }


        private void OnPhotoInfoUpdated(object sender, PhotoInfoUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                // Favourite icon
                if (PhotoSource.IsFavourite)
                    FavIconView.Visibility = Visibility.Visible;
                else
                    FavIconView.Visibility = Visibility.Collapsed;

            });
        }

        private void OnLicenseButtonClick(object sender, RoutedEventArgs e)
        {
            if (PhotoSource.LicenseId == null)
                return;

            License license = PolicyKit.CurrentPolicy.Licenses[PhotoSource.LicenseId];
            if (license.Url == null)
                return;

            WebBrowserTask wbTask = new WebBrowserTask();
            wbTask.Uri = new Uri(license.Url, UriKind.RelativeOrAbsolute);
            wbTask.Show();
        }

        private void OnAddedAsFavourite(object sender, PhotoAddedAsFavouriteEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                // Favourite icon
                if (PhotoSource.IsFavourite)
                    FavIconView.Visibility = Visibility.Visible;
                else
                    FavIconView.Visibility = Visibility.Collapsed;
            });
        }

        private void OnRemovedFromFavourite(object sender, PhotoRemovedFromFavouriteEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                // Favourite icon
                if (PhotoSource.IsFavourite)
                    FavIconView.Visibility = Visibility.Visible;
                else
                    FavIconView.Visibility = Visibility.Collapsed;
            });
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            DetailPage.FullScreenRequest(this, null);
        }
    }
}
