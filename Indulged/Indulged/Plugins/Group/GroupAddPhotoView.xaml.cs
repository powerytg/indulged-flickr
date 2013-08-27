using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Models;
using Indulged.Plugins.Group.events;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Indulged.Plugins.Group
{
    public partial class GroupAddPhotoView : UserControl
    {
        public FlickrGroup Group;
        private ObservableCollection<SelectablePhoto> PhotoCollection;

        // Selected photos
        private List<string> SelectedPhotos = new List<string>();

        // Can add to group?
        public bool CanAddPhotosToGroup { get; set; }

        // Constructor
        public GroupAddPhotoView(FlickrGroup groupSource)
        {
            InitializeComponent();

            Group = groupSource;

            // Initialize data providers
            PhotoCollection = new ObservableCollection<SelectablePhoto>();
            PhotoListView.ItemsSource = PhotoCollection;

            // Renderers
            PhotoPickerRenderer.CanSelect = true;

            // Events
            Cinderella.CinderellaCore.GroupInfoUpdated += OnGroupInfoReturned;
            Cinderella.CinderellaCore.PhotoStreamUpdated += OnPhotoStreamUpdated;

            PhotoPickerRenderer.SelectionChanged += OnPhotoPickerToggled;

            // Get group info
            StatusLabel.Text = "Retrieving group info";
            StatusProgressBar.Visibility = Visibility.Visible;
            Anaconda.AnacondaCore.GetGroupInfoAsync(groupSource.ResourceId);
        }

        private void OnGroupInfoReturned(object sender, GroupInfoUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.GroupId != Group.ResourceId)
                    return;

                StatusLabel.Text = "Loading photo collection";

                if (Group.ThrottleMode != null)
                {
                    if (Group.ThrottleRemainingCount == 0)
                    {
                        CanAddPhotosToGroup = false;
                        StatusLabel.Text = "You have reached limitation";
                        StatusProgressBar.Visibility = Visibility.Collapsed;
                        return;
                    }
                }

                // Get the list of user photos to choose from
                CanAddPhotosToGroup = true;
                Anaconda.AnacondaCore.GetPhotoStreamAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", "40" } });

            });
        }

        private void UpdateThrottleLabel()
        {
            StatusProgressBar.Visibility = Visibility.Collapsed;

            if (Group.ThrottleMode != null)
            {
                if (Group.ThrottleRemainingCount > 0)
                {
                    ThrottleLabel.Text = "You can add " + Group.ThrottleRemainingCount.ToString() + " out of " + Group.ThrottleMaxCount.ToString();
                }
                else
                {
                    StatusLabel.Text = "You have reached limitation";
                    StatusProgressBar.Visibility = Visibility.Collapsed;
                }
            }

        }

        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, PhotoStreamUpdatedEventArgs e)
        {
            if(e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                return;

            if (e.NewPhotos.Count == 0 && PhotoCollection.Count != 0)
                return;

            Dispatcher.BeginInvoke(() =>
            {
                // Always fill in first page
                List<Photo> photos = null;
                if (PhotoCollection.Count == 0)
                {
                    UpdateThrottleLabel();
                    StatusView.Visibility = Visibility.Collapsed;
                    photos = Cinderella.CinderellaCore.CurrentUser.Photos;
                }
                else
                    photos = e.NewPhotos;

                foreach (var photo in photos)
                {
                    SelectablePhoto photoItem = new SelectablePhoto();
                    photoItem.PhotoSource = photo;
                    photoItem.Selected = (SelectedPhotos.Contains(photo.ResourceId));
                    PhotoCollection.Add(photoItem);
                }
            });
        }

        // Implementation of inifinite scrolling
        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            SelectablePhoto photoItem = e.Container.Content as SelectablePhoto;
            if (photoItem == null)
                return;

            User currentUser = Cinderella.CinderellaCore.CurrentUser;
            int index = currentUser.Photos.IndexOf(photoItem.PhotoSource);

            bool canLoad = (!currentUser.IsLoadingPhotoStream && currentUser.Photos.Count < currentUser.PhotoCount);
            if (!canLoad)
                return;

            if (PhotoCollection.Count - index <= 2)
            {
                int page = currentUser.Photos.Count / 40 + 1;
                Anaconda.AnacondaCore.GetPhotoStreamAsync(currentUser.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", "40" } });
            }
        }

        private void OnPhotoPickerToggled(object sender, PhotoPickerRendererEventArgs e)
        {
            if (e.Selected && !SelectedPhotos.Contains(e.PhotoId))
            {
                bool canAddPhoto = false;
                if (Group.ThrottleMode == null)
                    canAddPhoto = true;
                else if (Group.ThrottleRemainingCount > SelectedPhotos.Count)
                    canAddPhoto = true;

                if (canAddPhoto)
                {
                    SelectedPhotos.Add(e.PhotoId);
                }
            }
            else if (!e.Selected && SelectedPhotos.Contains(e.PhotoId))
            {
                SelectedPhotos.Remove(e.PhotoId);
            }

            if (Group.ThrottleMode != null)
            {
                int remainCount = Math.Max(0, Group.ThrottleRemainingCount - SelectedPhotos.Count);
                ThrottleLabel.Text = "You can add " + remainCount.ToString() + " out of " + Group.ThrottleMaxCount.ToString();

                if (remainCount == 0)
                {
                    PhotoPickerRenderer.CanSelect = false;
                }
                else
                {
                    PhotoPickerRenderer.CanSelect = true;
                }

            }
        }

    }
}
