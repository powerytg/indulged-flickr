using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Avarice.Controls.SupportClasses;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Models;
using Indulged.Plugins.Group;
using Indulged.Plugins.Group.events;
using Indulged.Resources;
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
    public partial class PhotoSetPrimaryChooserView : UserControl, IModalPopupContent
    {
        public PhotoSet PhotoSetSource;
        private ObservableCollection<Photo> PhotoCollection;

        // Message colors
        private static SolidColorBrush normalMessageBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xae, 0xef));
        private static SolidColorBrush errorMessageBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xf6, 0x70, 0x56));

        // Constructor
        public PhotoSetPrimaryChooserView(PhotoSet setSource)
        {
            InitializeComponent();

            PhotoSetSource = setSource;

            // Initialize data providers
            PhotoCollection = new ObservableCollection<Photo>();
            foreach (var photo in PhotoSetSource.Photos)
            {
                PhotoCollection.Add(photo);
            }

            PhotoListView.ItemsSource = PhotoCollection;

            // Events
            Cinderella.CinderellaCore.PhotoSetPhotosUpdated += OnPhotoStreamUpdated;
            Cinderella.CinderellaCore.PhotoSetPrimaryChanged += OnPrimaryPhotoChanged;
            Anaconda.AnacondaCore.PhotoSetChangePrimaryException += OnPrimaryPhotoException;
        }

        public void OnPopupRemoved()
        {
            Cinderella.CinderellaCore.PhotoSetPhotosUpdated -= OnPhotoStreamUpdated;
            Cinderella.CinderellaCore.PhotoSetPrimaryChanged -= OnPrimaryPhotoChanged;
            Anaconda.AnacondaCore.PhotoSetChangePrimaryException -= OnPrimaryPhotoException;

            PhotoListView.ItemsSource = null;

            PhotoCollection.Clear();
            PhotoCollection = null;

            PhotoSetSource = null;
        }

        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, PhotoSetPhotosUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.PhotoSetId != PhotoSetSource.ResourceId)
                    return;

                if (e.NewPhotos.Count == 0 && PhotoCollection.Count != 0)
                    return;

                foreach (var photo in e.NewPhotos)
                {
                    PhotoCollection.Add(photo);
                }
            });
        }

        // Implementation of inifinite scrolling
        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            Photo photoItem = e.Container.Content as Photo;
            if (photoItem == null)
                return;

            int index = PhotoCollection.IndexOf(photoItem);

            bool canLoad = (PhotoSetSource.Photos.Count < PhotoSetSource.PhotoCount);
            if (PhotoCollection.Count - index <= 2 && canLoad)
            {
                int page = PhotoSetSource.Photos.Count / Anaconda.DefaultItemsPerPage + 1;
                Anaconda.AnacondaCore.GetPhotoSetPhotosAsync(PhotoSetSource.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
            }
        }

        private string currentProcessingPhotoId;
        private void SetPhotoAsPrimary(string photoId)
        {
            currentProcessingPhotoId = photoId;
            PhotoListView.IsEnabled = false;

            StatusProgressBar.Visibility = Visibility.Visible;
            StatusLabel.Text = "Setting photo as cover...";

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
                Anaconda.AnacondaCore.ChangePhotoSetPrimaryPhotoAsync(PhotoSetSource.ResourceId, photoId);
            };

            animation.Begin();
            
        }

        

        private void OnPrimaryPhotoChanged(object sender, PhotoSetPrimaryUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoSetId != PhotoSetSource.ResourceId)
                    return;

                currentProcessingPhotoId = null;
                PhotoListView.Opacity = 1;
                PhotoListView.IsEnabled = true;

                StatusProgressBar.Visibility = Visibility.Collapsed;
                StatusLabel.Foreground = normalMessageBrush;
                StatusLabel.Text = "Done";
            });
        }

        private void OnPrimaryPhotoException(object sender, ChangePhotoSetPrimaryExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoId != currentProcessingPhotoId)
                    return;

                currentProcessingPhotoId = null;
                PhotoListView.Opacity = 1;
                PhotoListView.IsEnabled = true;

                StatusProgressBar.Visibility = Visibility.Collapsed;
                StatusLabel.Foreground = errorMessageBrush;
                StatusLabel.Text = e.ErrorMessage;

                PhotoListView.SelectedItem = null;
            });
        }

       
        private void PhotoListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Photo selectedPhoto = PhotoListView.SelectedItem as Photo;
            SetPhotoAsPrimary(selectedPhoto.ResourceId);
        }
    }
}
