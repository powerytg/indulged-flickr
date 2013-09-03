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

            // Description
            if (PhotoSource.Description.Length > 0)
                DescriptionLabel.Visibility = Visibility.Visible;
            else
                DescriptionLabel.Visibility = Visibility.Collapsed;
        }

        public PhotoBasicInfoView()
        {
            InitializeComponent();

            // Events
            Cinderella.CinderellaCore.PhotoInfoUpdated += OnPhotoInfoUpdated;
            Cinderella.CinderellaCore.PhotoAddedAsFavourite += OnAddedAsFavourite;
            Cinderella.CinderellaCore.PhotoRemovedFromFavourite += OnRemovedFromFavourite;

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
