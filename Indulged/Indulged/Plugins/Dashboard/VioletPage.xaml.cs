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
        }

        // Photo uploaded
        private void OnPhotoUploaded(object sender, UploadedPhotoInfoReturnedEventArgs e)
        {
            if (PolicyKit.VioletPageSubscription != PolicyKit.MyStream)
                return;

            Photo newPhoto = Cinderella.CinderellaCore.PhotoCache[e.PhotoId];
            List<Photo> newPhotos = new List<Photo> { newPhoto };

            List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(newPhotos, PolicyKit.MyStream);
            PhotoCollection.Insert(0, newGroups[0]);
        }

        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, PhotoStreamUpdatedEventArgs e)
        {
            if (e.NewPhotos.Count == 0 || e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                return;

            List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos, PolicyKit.MyStream);
            foreach (var group in newGroups)
            {
                PhotoCollection.Add(group);
            }
        }

        // Discovery stream updated
        private void OnDiscoveryStreamUpdated(object sender, DiscoveryStreamUpdatedEventArgs e)
        {
            if (e.NewPhotos.Count == 0)
                return;

            List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos, PolicyKit.DiscoveryStream);
            foreach (var group in newGroups)
            {
                PhotoCollection.Add(group);
            }
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
                canLoad = (!currentUser.IsLoadingPhotoStream && currentUser.Photos.Count < currentUser.PhotoCount);
            else if (PolicyKit.VioletPageSubscription == PolicyKit.DiscoveryStream)
                canLoad = (!Anaconda.AnacondaCore.IsLoadingDiscoveryStream && Cinderella.CinderellaCore.DiscoveryList.Count < Cinderella.CinderellaCore.TotalDiscoveryPhotosCount);

            if (PhotoCollection.Count - index <= 2 && canLoad )
            {
                int page = currentUser.Photos.Count / 100 + 1;
                System.Diagnostics.Debug.WriteLine("page=" + page.ToString());

                if (PolicyKit.VioletPageSubscription == PolicyKit.MyStream)
                {
                    Anaconda.AnacondaCore.GetPhotoStreamAsync(currentUser.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", "100" } });
                }
                else if (PolicyKit.VioletPageSubscription == PolicyKit.DiscoveryStream)
                {
                    Anaconda.AnacondaCore.GetDiscoveryStreamAsync(new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", "100" } });
                }
            }
        }

        private void OnPolicyChanged(object sender, PolicyChangedEventArgs e)
        {
            if (e.PolicyName != "VioletPageSubscription")
                return;

            PhotoCollection.Clear();
            if (PolicyKit.VioletPageSubscription == PolicyKit.MyStream)
            {
                User currentUser = Cinderella.CinderellaCore.CurrentUser;
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
                    Anaconda.AnacondaCore.GetPhotoStreamAsync(currentUser.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", "100" } });
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
                    Anaconda.AnacondaCore.GetDiscoveryStreamAsync(new Dictionary<string, string> { { "page", "1" }, { "per_page", "100" } });
                }
            }
        }
    }
}
