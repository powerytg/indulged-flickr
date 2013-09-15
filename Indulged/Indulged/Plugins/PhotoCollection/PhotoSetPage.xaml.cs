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
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.Plugins.Dashboard;
using Indulged.API.Anaconda;
using Indulged.PolKit;
using Indulged.API.Avarice.Controls;

namespace Indulged.Plugins.PhotoCollection
{
    public partial class PhotoSetPage : PhoneApplicationPage
    {
        // Data source
        public PhotoSet PhotoSetSource { get; set; }

        private ObservableCollection<PhotoSetPhoto> PhotoCollection;

        // Constructor
        public PhotoSetPage()
        {
            InitializeComponent();

            // Initialize data providers
            PhotoCollection = new ObservableCollection<PhotoSetPhoto>();
            PhotoStreamListView.ItemsSource = PhotoCollection;

            // Events
            Cinderella.CinderellaCore.PhotoSetPhotosUpdated += OnPhotoStreamUpdated;
            Cinderella.CinderellaCore.AddPhotoToSetCompleted += OnPhotoAddedToSet;
            Cinderella.CinderellaCore.RemovePhotoFromSetCompleted += OnPhotoRemovedFromSet;

        }

        private bool executedOnce;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (executedOnce)
                return;

            executedOnce = true;

            string setId = NavigationContext.QueryString["photoset_id"];
            PhotoSetSource = Cinderella.CinderellaCore.PhotoSetCache[setId];
            this.DataContext = PhotoSetSource;

            // Initial items
            if (PhotoSetSource.Photos.Count > 0)
            {
                foreach (var photo in PhotoSetSource.Photos)
                {
                    var setPhoto = new PhotoSetPhoto { PhotoSource = photo, PhotoSetId = PhotoSetSource.ResourceId };
                    PhotoCollection.Add(setPhoto);
                }

            }

            // App bar
            ApplicationBar = Resources["PhotoPageAppBar"] as ApplicationBar;

            // Show loading progress indicator
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "loading photos";


            // Get first page of photo stream in the set
            Anaconda.AnacondaCore.GetPhotoSetPhotosAsync(PhotoSetSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });

        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (ModalPopup.HasPopupHistory())
            {
                e.Cancel = true;
                ModalPopup.RemoveLastPopup();
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }


        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, PhotoSetPhotosUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoSetId != PhotoSetSource.ResourceId)
                    return;

                SystemTray.ProgressIndicator.IsVisible = false;

                if (PhotoSetSource.Photos.Count == 0)
                {
                    StatusLabel.Text = "This set has no photos";
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StatusLabel.Visibility = Visibility.Collapsed;
                    PhotoStreamListView.Visibility = Visibility.Visible;
                    foreach (var photo in e.NewPhotos)
                    {
                        var setPhoto = new PhotoSetPhoto { PhotoSource = photo, PhotoSetId = PhotoSetSource.ResourceId };
                        PhotoCollection.Add(setPhoto);
                    }
                }
            });
        }


        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            PhotoSetPhoto setPhoto = e.Container.Content as PhotoSetPhoto;
            if (setPhoto == null)
                return;

            int index = PhotoCollection.IndexOf(setPhoto);

            bool canLoad = (PhotoSetSource.Photos.Count < PhotoSetSource.PhotoCount);
            if (PhotoCollection.Count - index <= 2 && canLoad)
            {
                // Show progress indicator
                SystemTray.ProgressIndicator.IsVisible = true;

                int page = PhotoSetSource.Photos.Count / Anaconda.DefaultItemsPerPage + 1;
                Anaconda.AnacondaCore.GetPhotoSetPhotosAsync(PhotoSetSource.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
            }
        }

        private void RefreshPhotoListButton_Click(object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "loading photos";

            Anaconda.AnacondaCore.GetPhotoSetPhotosAsync(PhotoSetSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

        private PhotoSetAddPhotoView addPhotoView;
        private void AddPhotoButton_Click(object sender, EventArgs e)
        {
            addPhotoView = new PhotoSetAddPhotoView(PhotoSetSource);
            var addPhotoDialog = ModalPopup.Show(addPhotoView, "Add To Photo Set", new List<string> { "Done Adding Photos" });
        }

        private void OnPhotoAddedToSet(object sender, AddPhotoToSetCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.SetId != PhotoSetSource.ResourceId)
                    return;

                Photo newPhoto = Cinderella.CinderellaCore.PhotoCache[e.PhotoId];
                var setPhoto = new PhotoSetPhoto { PhotoSource = newPhoto, PhotoSetId = PhotoSetSource.ResourceId };
                PhotoCollection.Insert(0, setPhoto);

                if (PhotoSetSource.Photos.Count > 0)
                {
                    StatusLabel.Visibility = Visibility.Collapsed;
                    PhotoStreamListView.Visibility = Visibility.Visible;
                }
            });
        }

        private void OnPhotoRemovedFromSet(object sender, RemovePhotoFromSetCompleteEventArgs e)
        {

            Dispatcher.BeginInvoke(() =>
            {
                if (e.SetId != PhotoSetSource.ResourceId || PhotoSetSource.Photos.Count == 0)
                    return;

                PhotoSetPhoto photoToRemove = null;
                foreach (var setPhoto in PhotoCollection)
                {
                    if (setPhoto.PhotoSource.ResourceId == e.PhotoId)
                    {
                        photoToRemove = setPhoto;
                        break;
                    }
                }

                if (photoToRemove != null)
                {
                    PhotoCollection.Remove(photoToRemove);
                }

                if (PhotoSetSource.Photos.Count == 0)
                {
                    StatusLabel.Text = "This set has no photos";
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;

                }
            });

        }
    }
}