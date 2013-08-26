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
using Indulged.API.Anaconda;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella;
using System.Collections.ObjectModel;
using Indulged.Plugins.Group.SupportClasses;

namespace Indulged.Plugins.Group
{
    public partial class GroupAddPhotoView : UserControl
    {
        public FlickrGroup Group;
        private ObservableCollection<PhotoGroup> PhotoCollection;

        public GroupAddPhotoView(FlickrGroup groupSource)
        {
            InitializeComponent();

            Group = groupSource;

            // Initialize data providers
            PhotoCollection = new ObservableCollection<PhotoGroup>();
            PhotoListView.ItemsSource = PhotoCollection;

            // Events
            Cinderella.CinderellaCore.GroupInfoUpdated += OnGroupInfoReturned;
            Cinderella.CinderellaCore.PhotoStreamUpdated += OnPhotoStreamUpdated;

            // Get group info
            StatusLabel.Text = "Retrieving group info...";
            StatusProgressBar.Visibility = Visibility.Visible;
            Anaconda.AnacondaCore.GetGroupInfoAsync(groupSource.ResourceId);
        }

        private void OnGroupInfoReturned(object sender, GroupInfoUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.GroupId != Group.ResourceId)
                    return;

                StatusProgressBar.Visibility = Visibility.Collapsed;

                if (Group.ThrottleMode != null)
                {
                    StatusLabel.Text = "You can add " + Group.ThrottleRemainingCount.ToString() + " out of " + Group.ThrottleMaxCount.ToString();

                    // Get the list of user photos to choose from

                }
            });
        }

        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, PhotoStreamUpdatedEventArgs e)
        {
            if (e.NewPhotos.Count == 0 || e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                return;

            Dispatcher.BeginInvoke(() =>
            {
                List<PhotoGroup> newGroups = PhotoGridFactory.GeneratePhotoGroup(e.NewPhotos);
                foreach (var group in newGroups)
                {
                    PhotoCollection.Add(group);
                }
            });
        }

    }
}
