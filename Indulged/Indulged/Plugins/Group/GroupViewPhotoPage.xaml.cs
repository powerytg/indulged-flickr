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
                    List<PhotoGroup> photoGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(_group.Photos, Group.ResourceId, "Group");
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
        }

        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, GroupPhotoListUpdatedEventArgs e)
        {
            if (e.NewPhotos.Count == 0 || e.GroupId != Group.ResourceId)
                return;

            List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos, Group.ResourceId, "Group");
            foreach (var group in newGroups)
            {
                PhotoCollection.Add(group);
            }
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
                int page = Group.Photos.Count / 100 + 1;
                Anaconda.AnacondaCore.GetGroupPhotosAsync(Group.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
            }

        }
    }
}
