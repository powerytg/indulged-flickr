using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.Plugins.Dashboard;
using Indulged.API.Anaconda;
using Indulged.Plugins.Common.PhotoGroupRenderers;

namespace Indulged.Plugins.Group
{
    public partial class GroupViewPhotoPage : UserControl
    {
        private FlickrGroup _group;
        public FlickrGroup Group 
        {
            get
            {
                return _group;
            }

            set
            {
                _group = value;

                if (_group.Photos.Count > 0)
                {
                    List<PhotoGroup> photoGroups = CommonPhotoGroupFactory.GeneratePhotoGroup(_group.Photos, Group.ResourceId, "Group");
                    foreach (var group in photoGroups)
                    {
                        PhotoCollection.Add(group);
                    }

                }
            }
        }

        // Photo data source
        public ObservableCollection<PhotoGroup> PhotoCollection { get; set; }

        // Constructor
        public GroupViewPhotoPage()
        {
            InitializeComponent();

            // Initialize data providers
            PhotoCollection = new ObservableCollection<PhotoGroup>();
            PhotoStreamListView.ItemsSource = PhotoCollection;

            // Events
            Cinderella.CinderellaCore.GroupPhotoListUpdated += OnPhotoStreamUpdated;
            Cinderella.CinderellaCore.AddPhotoToGroupCompleted += OnPhotoAddedToGroup;
            Cinderella.CinderellaCore.RemovePhotoFromGroupCompleted += OnPhotoRemovedFromGroup;
        }

        private void OnPhotoAddedToGroup(object sender, AddPhotoToGroupCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.GroupId != Group.ResourceId)
                    return;

                Photo newPhoto = Cinderella.CinderellaCore.PhotoCache[e.PhotoId];
                List<PhotoGroup> photoGroups = CommonPhotoGroupFactory.GeneratePhotoGroup(new List<Photo> { newPhoto }, Group.ResourceId, "Group");
                foreach (var group in photoGroups)
                {
                    PhotoCollection.Insert(0, group);
                }
            });
        }

        private void OnPhotoRemovedFromGroup(object sender, RemovePhotoFromGroupCompleteEventArgs e)
        {

            Dispatcher.BeginInvoke(() =>
            {
                if (e.GroupId != Group.ResourceId || Group.Photos.Count == 0)
                    return;

                PhotoCollection.Clear();
                List<PhotoGroup> photoGroups = CommonPhotoGroupFactory.GeneratePhotoGroup(Group.Photos, Group.ResourceId, "Group");
                foreach (var group in photoGroups)
                {
                    PhotoCollection.Add(group);
                }
            });
            
        }

        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, GroupPhotoListUpdatedEventArgs e)
        {
            if (e.NewPhotos.Count == 0 || e.GroupId != Group.ResourceId)
                return;

            Dispatcher.BeginInvoke(() => {
                if (e.Page == 1)
                    PhotoCollection.Clear();

                List<PhotoGroup> newGroups = CommonPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos, Group.ResourceId, "Group");
                foreach (var group in newGroups)
                {
                    PhotoCollection.Add(group);
                }
            });
        }

        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            PhotoGroup photoGroup = e.Container.Content as PhotoGroup;
            if (photoGroup == null)
                return;

            int index = PhotoCollection.IndexOf(photoGroup);

            bool canLoad = (Group.Photos.Count < Group.PhotoCount);
            if (PhotoCollection.Count - index <= 2 && canLoad)
            {
                // Show progress indicator
                SystemTray.ProgressIndicator.IsVisible = true;

                int page = Group.Photos.Count / Anaconda.DefaultItemsPerPage + 1;
                Anaconda.AnacondaCore.GetGroupPhotosAsync(Group.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
            }

        }
    }
}
