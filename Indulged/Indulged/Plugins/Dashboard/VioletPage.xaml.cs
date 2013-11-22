using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Events;
using Indulged.PolKit;
using Indulged.Plugins.Dashboard.Events;
using Indulged.API.Anaconda.Events;
using Indulged.Plugins.Chrome.Services;

namespace Indulged.Plugins.Dashboard
{
    public partial class VioletPage : UserControl, IDashboardPage
    {
        public string PageName
        {
            get
            {
                return "VioletPage";
            }
        }

        public string BackgroundImageUrl
        {
            get
            {
                //return "/Assets/Chrome/VioletBackground.png";
                return null;
            }
        }

        public bool ShouldUseLightBackground
        {
            get
            {
                return false;
            }
        }

        // Photo data source
        public ObservableCollection<PhotoGroup> PhotoCollection { get; set; }

        // Constructor
        public VioletPage()
        {
            InitializeComponent();

            // Initialize data providers
            PhotoCollection = new ObservableCollection<PhotoGroup>();
            PhotoStreamListView.ItemsSource = PhotoCollection;

            // Events
            PolicyKit.PolicyChanged += OnPolicyChanged;
            Cinderella.CinderellaCore.PhotoStreamUpdated += OnPhotoStreamUpdated;
            Cinderella.CinderellaCore.UploadedPhotoInfoReturned += OnPhotoUploaded;
            Cinderella.CinderellaCore.DiscoveryStreamUpdated += OnDiscoveryStreamUpdated;
            Cinderella.CinderellaCore.FavouriteStreamUpdated += OnFavouriteStreamUpdated;

            Anaconda.AnacondaCore.FavouriteStreamException += OnFavouriteStreamException;
            Anaconda.AnacondaCore.DiscoveryStreamException += OnDiscoveryStreamException;
            Anaconda.AnacondaCore.PhotoStreamException += OnPhotoStreamException;
        }

        public void OnNavigatedFromPage()
        {
            PhotoStreamListView.ItemsSource = null;
        }

        public void OnNavigatedToPage()
        {
            if (PhotoCollection == null)
                return;
            
            PhotoStreamListView.ItemsSource = PhotoCollection;
        }

        // Photo uploaded
        private void OnPhotoUploaded(object sender, UploadedPhotoInfoReturnedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (PolicyKit.VioletPageSubscription != PolicyKit.MyStream)
                    return;

                Photo newPhoto = Cinderella.CinderellaCore.PhotoCache[e.PhotoId];
                List<Photo> newPhotos = new List<Photo> { newPhoto };

                List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(newPhotos, PolicyKit.MyStream);
                PhotoCollection.Insert(0, newGroups[0]);
            });
        }

        private void UpdateStreamVisibility()
        {
            if (PhotoCollection.Count == 0)
            {
                StatusLabel.Text = "You don't have items in this stream";
                PhotoStreamListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                StatusLabel.Visibility = Visibility.Collapsed;
                PhotoStreamListView.Visibility = Visibility.Visible;
            }
        }

        private void ShowLoadingScreen()
        {
            StatusLabel.Text = "Loading stream...";
            StatusLabel.Visibility = Visibility.Visible;
            PhotoStreamListView.Visibility = Visibility.Collapsed;
        }

        // Photo stream exception
        private void OnPhotoStreamException(object sender, GetPhotoStreamExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;

                if (Cinderella.CinderellaCore.CurrentUser.Photos.Count == 0)
                {
                    StatusLabel.Text = "Cannot load your photo stream";
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;
                }
            });
        }

        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, PhotoStreamUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;

                if (Cinderella.CinderellaCore.CurrentUser.Photos.Count == 0)
                {
                    StatusLabel.Text = "No photos available";
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;
                    return;
                }

                StatusLabel.Visibility = Visibility.Collapsed;
                PhotoStreamListView.Visibility = Visibility.Visible;

                if (e.NewPhotos.Count == 0)
                    return;

                List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos, PolicyKit.MyStream);
                foreach (var group in newGroups)
                {
                    PhotoCollection.Add(group);
                }

                UpdateStreamVisibility();

                // Update live tiles
                var tilePhotos = new List<Photo>();
                for (int i = 0; i < Math.Min(LiveTileUpdateService.MAX_TILE_IMAGE_COUNT, e.NewPhotos.Count); i++)
                {
                    var photo = e.NewPhotos[i];
                    tilePhotos.Add(photo);
                }

                LiveTileUpdateService.Instance.StartNewRequests(tilePhotos);
            });
        }

        // Discovery stream exception
        private void OnDiscoveryStreamException(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (Cinderella.CinderellaCore.DiscoveryList.Count == 0)
                {
                    StatusLabel.Text = "Cannot load discovery stream";
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;
                }

            });
        }

        // Discovery stream updated
        private void OnDiscoveryStreamUpdated(object sender, DiscoveryStreamUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (Cinderella.CinderellaCore.DiscoveryList.Count == 0)
                {
                    StatusLabel.Text = "No photos available";
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;
                    return;
                }

                StatusLabel.Visibility = Visibility.Collapsed;
                PhotoStreamListView.Visibility = Visibility.Visible;

                if (e.NewPhotos.Count == 0)
                    return;
                
                List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos, PolicyKit.DiscoveryStream);
                foreach (var group in newGroups)
                {
                    PhotoCollection.Add(group);
                }

                UpdateStreamVisibility();

                // Update live tiles
                var tilePhotos = new List<Photo>();
                for (int i = 0; i < Math.Min(LiveTileUpdateService.MAX_TILE_IMAGE_COUNT, e.NewPhotos.Count); i++)
                {
                    var photo = e.NewPhotos[i];
                    tilePhotos.Add(photo);
                }

                LiveTileUpdateService.Instance.StartNewRequests(tilePhotos);
            });
        }

        // Favourite stream exception
        private void OnFavouriteStreamException(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (Cinderella.CinderellaCore.FavouriteList.Count == 0)
                {
                    StatusLabel.Text = "Cannot load favourite photos";
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;
                }

            });
        }

        // Favourite stream updated
        private void OnFavouriteStreamUpdated(object sender, FavouriteStreamUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (Cinderella.CinderellaCore.FavouriteList.Count == 0)
                {
                    StatusLabel.Text = "No photos available";
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;

                    return;
                }

                StatusLabel.Visibility = Visibility.Collapsed;
                PhotoStreamListView.Visibility = Visibility.Visible;

                if (e.NewPhotos.Count == 0)
                    return;

                List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos, PolicyKit.FavouriteStream);
                foreach (var group in newGroups)
                {
                    PhotoCollection.Add(group);
                }

                UpdateStreamVisibility();
            });
        }

        // Implementation of inifinite scrolling
        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            PhotoGroup photoGroup = e.Container.Content as PhotoGroup;
            if (photoGroup == null)
                return;

            int index = PhotoCollection.IndexOf(photoGroup);
            User currentUser = Cinderella.CinderellaCore.CurrentUser;

            bool canLoad = false;
            if (PolicyKit.VioletPageSubscription == PolicyKit.MyStream)
                canLoad = (!currentUser.IsLoadingPhotoStream && currentUser.Photos.Count < currentUser.PhotoCount && currentUser.PhotoCount != 0);
            else if (PolicyKit.VioletPageSubscription == PolicyKit.DiscoveryStream)
                canLoad = (!Anaconda.AnacondaCore.IsLoadingDiscoveryStream && Cinderella.CinderellaCore.DiscoveryList.Count < Cinderella.CinderellaCore.TotalDiscoveryPhotosCount && Cinderella.CinderellaCore.TotalDiscoveryPhotosCount != 0);
            else if (PolicyKit.VioletPageSubscription == PolicyKit.FavouriteStream)
                canLoad = (!Anaconda.AnacondaCore.isLoadingFavStream && Cinderella.CinderellaCore.FavouriteList.Count < Cinderella.CinderellaCore.TotalFavouritePhotosCount && Cinderella.CinderellaCore.TotalFavouritePhotosCount != 0);

            if (PhotoCollection.Count - index <= 2 && canLoad )
            {
                int page = 1;

                if (PolicyKit.VioletPageSubscription == PolicyKit.MyStream)
                {
                    page = currentUser.Photos.Count / PolicyKit.StreamItemsCountPerPage + 1;
                    Anaconda.AnacondaCore.GetPhotoStreamAsync(currentUser.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
                }
                else if (PolicyKit.VioletPageSubscription == PolicyKit.DiscoveryStream)
                {
                    page = Cinderella.CinderellaCore.DiscoveryList.Count / PolicyKit.StreamItemsCountPerPage + 1;
                    Anaconda.AnacondaCore.GetDiscoveryStreamAsync(new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
                }
                else if (PolicyKit.VioletPageSubscription == PolicyKit.FavouriteStream)
                {
                    page = Cinderella.CinderellaCore.FavouriteList.Count / PolicyKit.StreamItemsCountPerPage + 1;
                    Anaconda.AnacondaCore.GetFavouritePhotoStreamAsync(currentUser.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
                }
            }
        }

        private void OnPolicyChanged(object sender, PolicyChangedEventArgs e)
        {
            if (e.PolicyName != "VioletPageSubscription")
                return;

            ReloadStreams();
        }

        public void ReloadStreams()
        {
            PhotoCollection.Clear();

            User currentUser = Cinderella.CinderellaCore.CurrentUser;
            if (PolicyKit.VioletPageSubscription == PolicyKit.MyStream)
            {
                if (currentUser.Photos.Count > 0)
                {
                    List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(currentUser.Photos);
                    foreach (var group in newGroups)
                    {
                        PhotoCollection.Add(group);
                    }

                    StatusLabel.Visibility = Visibility.Collapsed;
                    PhotoStreamListView.Visibility = Visibility.Visible;
                }
                else
                {
                    ShowLoadingScreen();
                    Anaconda.AnacondaCore.GetPhotoStreamAsync(currentUser.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
                }

            }
            else if (PolicyKit.VioletPageSubscription == PolicyKit.DiscoveryStream)
            {
                if (Cinderella.CinderellaCore.DiscoveryList.Count > 0)
                {
                    List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(Cinderella.CinderellaCore.DiscoveryList);
                    foreach (var group in newGroups)
                    {
                        PhotoCollection.Add(group);
                    }

                    StatusLabel.Visibility = Visibility.Collapsed;
                    PhotoStreamListView.Visibility = Visibility.Visible;
                }
                else
                {
                    ShowLoadingScreen();
                    Anaconda.AnacondaCore.GetDiscoveryStreamAsync(new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
                }
            }
            else if (PolicyKit.VioletPageSubscription == PolicyKit.FavouriteStream)
            {
                if (Cinderella.CinderellaCore.FavouriteList.Count > 0)
                {
                    List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(Cinderella.CinderellaCore.FavouriteList);
                    foreach (var group in newGroups)
                    {
                        PhotoCollection.Add(group);
                    }

                    StatusLabel.Visibility = Visibility.Collapsed;
                    PhotoStreamListView.Visibility = Visibility.Visible;
                }
                else
                {
                    ShowLoadingScreen();
                    Anaconda.AnacondaCore.GetFavouritePhotoStreamAsync(currentUser.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
                }
            }
        }
    }
}
