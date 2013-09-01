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

namespace Indulged.Plugins.Dashboard
{
    public partial class VioletPage : UserControl, IDashboardPage
    {
        public string BackgroundImageUrl
        {
            get
            {
                //return "/Assets/Chrome/VioletBackground.png";
                return null;
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

        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, PhotoStreamUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.NewPhotos.Count == 0 || e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;

                List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos, PolicyKit.MyStream);
                foreach (var group in newGroups)
                {
                    PhotoCollection.Add(group);
                }

                UpdateStreamVisibility();
            });
        }

        // Discovery stream updated
        private void OnDiscoveryStreamUpdated(object sender, DiscoveryStreamUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.NewPhotos.Count == 0)
                    return;
                
                List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos, PolicyKit.DiscoveryStream);
                foreach (var group in newGroups)
                {
                    PhotoCollection.Add(group);
                }

                UpdateStreamVisibility();
            });
        }

        // Favourite stream updated
        private void OnFavouriteStreamUpdated(object sender, FavouriteStreamUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
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
