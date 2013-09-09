﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.API.Cinderella;
using System.Collections.ObjectModel;
using Indulged.API.Cinderella.Models;
using Indulged.API.Anaconda;
using Indulged.PolKit;
using Indulged.API.Cinderella.Events;

namespace Indulged.Plugins.Favourite
{
    public partial class FavouritePage : PhoneApplicationPage
    {
        private ObservableCollection<Photo> _photos = new ObservableCollection<Photo>();

        // Constructor
        public FavouritePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Show loading progress indicator
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = false;
            SystemTray.ProgressIndicator.Text = "retrieving photos";

            // Events
            Cinderella.CinderellaCore.FavouriteStreamUpdated += OnFavouriteStreamUpdated;

            if (Cinderella.CinderellaCore.FavouriteList.Count > 0)
            {
                StatusLabel.Visibility = Visibility.Collapsed;
                _photos.Clear();
                foreach (var photo in Cinderella.CinderellaCore.FavouriteList)
                {
                    _photos.Add(photo);
                }
            }
            else
            {
                StatusLabel.Visibility = Visibility.Visible;

                SystemTray.ProgressIndicator.IsVisible = true;
                var currentUser = Cinderella.CinderellaCore.CurrentUser;
                Anaconda.AnacondaCore.GetFavouritePhotoStreamAsync(currentUser.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
            }

            ResultListView.ItemsSource = _photos;
        }

        // Favourite stream updated
        private void OnFavouriteStreamUpdated(object sender, FavouriteStreamUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if(SystemTray.ProgressIndicator != null)
                    SystemTray.ProgressIndicator.IsVisible = false;

                if (Cinderella.CinderellaCore.FavouriteList.Count == 0)
                {
                    StatusLabel.Visibility = Visibility.Visible;
                    StatusLabel.Text = "You don't have any favourite photos";
                    ResultListView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StatusLabel.Visibility = Visibility.Collapsed;
                    ResultListView.Visibility = Visibility.Visible;
                }

                if (e.NewPhotos.Count == 0)
                    return;

                foreach (var photo in e.NewPhotos)
                {
                    if(!_photos.Contains(photo))
                        _photos.Add(photo);
                }

            });
        }

        private void ResultListView_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            Photo photo = e.Container.Content as Photo;
            if (photo == null)
                return;

            bool canLoad = canLoad = (!Anaconda.AnacondaCore.isLoadingFavStream && Cinderella.CinderellaCore.FavouriteList.Count < Cinderella.CinderellaCore.TotalFavouritePhotosCount && Cinderella.CinderellaCore.TotalFavouritePhotosCount != 0);
            int index = _photos.IndexOf(photo);

            if (_photos.Count - index <= 2 && canLoad)
            {
                var currentUser = Cinderella.CinderellaCore.CurrentUser;
                int page = page = Cinderella.CinderellaCore.FavouriteList.Count / PolicyKit.StreamItemsCountPerPage + 1;

                SystemTray.ProgressIndicator.IsVisible = true;
                Anaconda.AnacondaCore.GetFavouritePhotoStreamAsync(currentUser.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
            }
        }

        private void RefreshPhotoListButton_Click(object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator.IsVisible = true;

            var currentUser = Cinderella.CinderellaCore.CurrentUser; 
            Anaconda.AnacondaCore.GetFavouritePhotoStreamAsync(currentUser.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });

        }
    }
}