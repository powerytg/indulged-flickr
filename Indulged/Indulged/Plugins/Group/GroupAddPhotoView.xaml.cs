using Indulged.API.Avarice.Controls.SupportClasses;
using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Avarice.Controls;
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using Indulged.Resources;

namespace Indulged.Plugins.Group
{
    public partial class GroupAddPhotoView : UserControl, IModalPopupContent
    {
        public FlickrGroup Group;
        private ObservableCollection<SelectablePhoto> PhotoCollection;

        // Selected photos
        private List<string> SelectedPhotos = new List<string>();

        // Can add to group?
        public bool CanAddPhotosToGroup { get; set; }

        // Message colors
        private SolidColorBrush normalMessageBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xae, 0xef));
        private SolidColorBrush errorMessageBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xf6, 0x70, 0x56));

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

            Cinderella.CinderellaCore.AddPhotoToGroupCompleted += OnAddPhotoCompleted;
            Anaconda.AnacondaCore.AddPhotoToGroupException += OnAddPhotoException;

            Cinderella.CinderellaCore.RemovePhotoFromGroupCompleted += OnRemovePhotoCompleted;
            Anaconda.AnacondaCore.RemovePhotoFromGroupException += OnRemovePhotoException;

            PhotoPickerRenderer.SelectionChanged += OnPhotoPickerToggled;

            // Get group info
            StatusLabel.Text = AppResources.GroupRetrievingInfoText;
            StatusProgressBar.Visibility = Visibility.Visible;
            Anaconda.AnacondaCore.GetGroupInfoAsync(groupSource.ResourceId);
        }

        private bool eventListenersRemoved = false;
        public void OnPopupRemoved()
        {
            if (eventListenersRemoved)
                return;

            eventListenersRemoved = true;

            Cinderella.CinderellaCore.GroupInfoUpdated -= OnGroupInfoReturned;
            Cinderella.CinderellaCore.PhotoStreamUpdated -= OnPhotoStreamUpdated;

            Cinderella.CinderellaCore.AddPhotoToGroupCompleted -= OnAddPhotoCompleted;
            Anaconda.AnacondaCore.AddPhotoToGroupException -= OnAddPhotoException;

            Cinderella.CinderellaCore.RemovePhotoFromGroupCompleted -= OnRemovePhotoCompleted;
            Anaconda.AnacondaCore.RemovePhotoFromGroupException -= OnRemovePhotoException;

            PhotoPickerRenderer.SelectionChanged -= OnPhotoPickerToggled;

            PhotoListView.ItemsSource = null;
            PhotoCollection.Clear();
            PhotoCollection = null;

            Group = null;
            SelectedPhotos.Clear();
            SelectedPhotos = null;
        }

        private void OnGroupInfoReturned(object sender, GroupInfoUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.GroupId != Group.ResourceId)
                    return;

                StatusLabel.Text = AppResources.GroupLoadingPhotoCollectionText;

                if (Group.ThrottleMode != "none")
                {
                    if (Group.ThrottleRemainingCount == 0)
                    {
                        CanAddPhotosToGroup = false;
                        StatusLabel.Text = AppResources.ThrottleReachedErrorText;
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
            ThrottleLabel.Foreground = normalMessageBrush;

            if (Group.ThrottleMode != "none")
            {
                int remainCount = Group.ThrottleRemainingCount;
                if (remainCount > 0)
                {
                    ThrottleLabel.Text = "You can add " + remainCount.ToString() + " out of " + Group.ThrottleMaxCount.ToString();
                }
                else
                {
                    StatusLabel.Text = AppResources.ThrottleReachedErrorText;
                    StatusProgressBar.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                ThrottleLabel.Text = "More photos can be added";
            }

        }

        private void UpdatePickerAvailbility()
        {
            if (Group.ThrottleMode == "none")
                CanAddPhotosToGroup = true;
            else if (Group.ThrottleRemainingCount > SelectedPhotos.Count)
                CanAddPhotosToGroup = true;
            else
                CanAddPhotosToGroup = false;

            PhotoPickerRenderer.CanSelect = CanAddPhotosToGroup;
        }


        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, PhotoStreamUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;

                if (e.NewPhotos.Count == 0 && PhotoCollection.Count != 0)
                    return;
                
                // Always fill in first page
                List<Photo> photos = null;
                if (PhotoCollection.Count == 0)
                {
                    StatusProgressBar.Visibility = Visibility.Collapsed;

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
            // Add or delete photo 
            if (e.Selected)
            {
                AddPhotoToGroup(e.PhotoId);
            }
            else
            {
                RemovePhotoFromGroup(e.PhotoId);
            }
        }

        private string currentProcessinPhotoId;

        private void AddPhotoToGroup(string photoId)
        {
            Photo selectedPhoto = Cinderella.CinderellaCore.PhotoCache[photoId];
            currentProcessinPhotoId = photoId;
            PhotoListView.IsEnabled = false;

            ThrottleProgressBar.Visibility = Visibility.Visible;
            ThrottleLabel.Text = "Adding photo";

            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.Duration = duration;

            DoubleAnimation pickerAnimation = new DoubleAnimation();
            animation.Children.Add(pickerAnimation);
            pickerAnimation.Duration = animation.Duration;
            pickerAnimation.To = 0.3;
            Storyboard.SetTarget(pickerAnimation, PhotoListView);
            Storyboard.SetTargetProperty(pickerAnimation, new PropertyPath("Opacity"));

            animation.Completed += (sender, e) => {
                Anaconda.AnacondaCore.AddPhotoToGroupAsync(photoId, Group.ResourceId);
            };

            animation.Begin();
            
        }

        private void RemovePhotoFromGroup(string photoId)
        {
            Photo selectedPhoto = Cinderella.CinderellaCore.PhotoCache[photoId];
            currentProcessinPhotoId = photoId;
            PhotoListView.IsEnabled = false;

            ThrottleProgressBar.Visibility = Visibility.Visible;
            ThrottleLabel.Text = AppResources.GroupRemovingPhotoText;

            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.Duration = duration;

            DoubleAnimation pickerAnimation = new DoubleAnimation();
            animation.Children.Add(pickerAnimation);
            pickerAnimation.Duration = animation.Duration;
            pickerAnimation.To = 0.3;
            Storyboard.SetTarget(pickerAnimation, PhotoListView);
            Storyboard.SetTargetProperty(pickerAnimation, new PropertyPath("Opacity"));

            animation.Completed += (sender, e) =>
            {
                Anaconda.AnacondaCore.RemovePhotoFromGroupAsync(photoId, Group.ResourceId);
            };

            animation.Begin();
        }

        private void OnAddPhotoCompleted(object sender, AddPhotoToGroupCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoId != currentProcessinPhotoId)
                    return;

                currentProcessinPhotoId = null;
                PhotoListView.Opacity = 1;
                PhotoListView.IsEnabled = true;

                ThrottleProgressBar.Visibility = Visibility.Collapsed;
                ThrottleLabel.Foreground = normalMessageBrush;
                UpdateThrottleLabel();

                SelectedPhotos.Add(e.PhotoId);
            });
        }

        private void OnAddPhotoException(object sender, AddPhotoToGroupExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoId != currentProcessinPhotoId)
                    return;

                currentProcessinPhotoId = null;
                PhotoListView.Opacity = 1;
                PhotoListView.IsEnabled = true;

                ThrottleProgressBar.Visibility = Visibility.Collapsed;
                ThrottleLabel.Foreground = errorMessageBrush;
                ThrottleLabel.Text = e.ErrorMessage;

                // Revert renderer
                var evt = new PhotoPickerRendererEventArgs();
                evt.PhotoId = e.PhotoId;
                evt.Selected = false;
                PhotoPickerRenderer.PhotoSourceSelectionStateChanged(this, evt);
            });
        }

        private void OnRemovePhotoCompleted(object sender, RemovePhotoFromGroupCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoId != currentProcessinPhotoId)
                    return;

                currentProcessinPhotoId = null;
                PhotoListView.Opacity = 1;
                PhotoListView.IsEnabled = true;

                ThrottleProgressBar.Visibility = Visibility.Collapsed;
                ThrottleLabel.Foreground = normalMessageBrush;
                UpdateThrottleLabel();

                if (SelectedPhotos.Contains(e.PhotoId))
                    SelectedPhotos.Remove(e.PhotoId);
            });
        }

        private void OnRemovePhotoException(object sender, RemovePhotoFromGroupExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoId != currentProcessinPhotoId)
                    return;

                currentProcessinPhotoId = null;
                PhotoListView.Opacity = 1;
                PhotoListView.IsEnabled = true;

                ThrottleProgressBar.Visibility = Visibility.Collapsed;
                ThrottleLabel.Foreground = errorMessageBrush;
                ThrottleLabel.Text = e.ErrorMessage;

                // Revert renderer
                var evt = new PhotoPickerRendererEventArgs();
                evt.PhotoId = e.PhotoId;
                evt.Selected = true;
                PhotoPickerRenderer.PhotoSourceSelectionStateChanged(this, evt);
            });
            
        }
    }
}
