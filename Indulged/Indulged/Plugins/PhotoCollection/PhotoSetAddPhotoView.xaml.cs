using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Avarice.Controls;
using Indulged.API.Avarice.Controls.SupportClasses;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Models;
using Indulged.Plugins.Group;
using Indulged.Plugins.Group.events;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Indulged.Plugins.PhotoCollection
{
    public partial class PhotoSetAddPhotoView : UserControl, IModalPopupContent
    {
        public PhotoSet PhotoSetSource;
        private ObservableCollection<SelectablePhoto> PhotoCollection;

        // Selected photos
        private List<string> SelectedPhotos = new List<string>();

        // Message colors
        private SolidColorBrush normalMessageBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xae, 0xef));
        private SolidColorBrush errorMessageBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xf6, 0x70, 0x56));

        // Constructor
        public PhotoSetAddPhotoView(PhotoSet setSource)
        {
            InitializeComponent();

            PhotoSetSource = setSource;

            // Initialize data providers
            PhotoCollection = new ObservableCollection<SelectablePhoto>();
            PhotoListView.ItemsSource = PhotoCollection;

            // Renderers
            PhotoPickerRenderer.CanSelect = true;

            // Events
            Cinderella.CinderellaCore.PhotoStreamUpdated += OnPhotoStreamUpdated;

            Cinderella.CinderellaCore.AddPhotoToSetCompleted += OnAddPhotoCompleted;
            Anaconda.AnacondaCore.AddPhotoToSetException += OnAddPhotoException;

            Cinderella.CinderellaCore.RemovePhotoFromSetCompleted += OnRemovePhotoCompleted;
            Anaconda.AnacondaCore.RemovePhotoFromSetException += OnRemovePhotoException;

            PhotoPickerRenderer.SelectionChanged += OnPhotoPickerToggled;

            // Get group info
            StatusLabel.Text = "Loading photo collection";
            StatusProgressBar.Visibility = Visibility.Visible;
            Anaconda.AnacondaCore.GetPhotoStreamAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", "40" } });
        }

        public void OnPopupRemoved()
        {
            Cinderella.CinderellaCore.PhotoStreamUpdated -= OnPhotoStreamUpdated;

            Cinderella.CinderellaCore.AddPhotoToSetCompleted -= OnAddPhotoCompleted;
            Anaconda.AnacondaCore.AddPhotoToSetException -= OnAddPhotoException;

            Cinderella.CinderellaCore.RemovePhotoFromSetCompleted -= OnRemovePhotoCompleted;
            Anaconda.AnacondaCore.RemovePhotoFromSetException -= OnRemovePhotoException;

            PhotoPickerRenderer.SelectionChanged -= OnPhotoPickerToggled;

            PhotoListView.ItemsSource = null;

            PhotoCollection.Clear();
            PhotoCollection = null;

            SelectedPhotos.Clear();
            SelectedPhotos = null;

            PhotoSetSource = null;
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
                AddPhotoToSet(e.PhotoId);
            }
            else
            {
                RemovePhotoFromSet(e.PhotoId);
            }
        }

        private string currentProcessinPhotoId;

        private void AddPhotoToSet(string photoId)
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
                Anaconda.AnacondaCore.AddPhotoToSetAsync(photoId, PhotoSetSource.ResourceId);
            };

            animation.Begin();
            
        }

        private void RemovePhotoFromSet(string photoId)
        {
            Photo selectedPhoto = Cinderella.CinderellaCore.PhotoCache[photoId];
            currentProcessinPhotoId = photoId;
            PhotoListView.IsEnabled = false;

            ThrottleProgressBar.Visibility = Visibility.Visible;
            ThrottleLabel.Text = "Removing photo";

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
                Anaconda.AnacondaCore.RemovePhotoFromSetAsync(photoId, PhotoSetSource.ResourceId);
            };

            animation.Begin();
        }

        private void OnAddPhotoCompleted(object sender, AddPhotoToSetCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoId != currentProcessinPhotoId)
                    return;

                currentProcessinPhotoId = null;
                PhotoListView.Opacity = 1;
                PhotoListView.IsEnabled = true;

                ThrottleProgressBar.Visibility = Visibility.Collapsed;
                ThrottleLabel.Foreground = normalMessageBrush;
                ThrottleLabel.Text = "Photo has been added to set";

                SelectedPhotos.Add(e.PhotoId);
            });
        }

        private void OnAddPhotoException(object sender, AddPhotoToSetExceptionEventArgs e)
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

        private void OnRemovePhotoCompleted(object sender, RemovePhotoFromSetCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoId != currentProcessinPhotoId)
                    return;

                currentProcessinPhotoId = null;
                PhotoListView.Opacity = 1;
                PhotoListView.IsEnabled = true;

                ThrottleProgressBar.Visibility = Visibility.Collapsed;
                ThrottleLabel.Foreground = normalMessageBrush;
                ThrottleLabel.Text = "Photo has been removed from set";

                if (SelectedPhotos.Contains(e.PhotoId))
                    SelectedPhotos.Remove(e.PhotoId);
            });
        }

        private void OnRemovePhotoException(object sender, RemovePhotoFromSetExceptionEventArgs e)
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
