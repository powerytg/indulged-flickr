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
using Indulged.API.Anaconda.Events;
using Indulged.PolKit;
using Indulged.API.Avarice.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Indulged.Resources;
using Indulged.Plugins.Common.PhotoGroupRenderers;
using Indulged.Plugins.PhotoCollection.Renderers;

namespace Indulged.Plugins.PhotoCollection
{
    public partial class PhotoSetPage : PhoneApplicationPage
    {
        // Data source
        public PhotoSet PhotoSetSource { get; set; }
        private ObservableCollection<PhotoGroup> PhotoCollection;
        private CommonPhotoGroupFactory rendererFactory;

        // Constructor
        public PhotoSetPage()
        {
            InitializeComponent();

            // Initialize data providers
            PhotoCollection = new ObservableCollection<PhotoGroup>();
            PhotoStreamListView.ItemsSource = PhotoCollection;

            // Events
            Cinderella.CinderellaCore.PhotoSetPhotosUpdated += OnPhotoStreamUpdated;
            Anaconda.AnacondaCore.PhotoSetPhotosException += OnPhotoStreamException;

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
            rendererFactory = new CommonPhotoGroupFactory();
            rendererFactory.Context = PhotoSetSource.ResourceId;
            rendererFactory.ContextType = "PhotoSet";

            PerformAppearanceAnimation();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                PerformDisappearanceAnimation();
            }
        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            Cinderella.CinderellaCore.PhotoSetPhotosUpdated -= OnPhotoStreamUpdated;
            Cinderella.CinderellaCore.AddPhotoToSetCompleted -= OnPhotoAddedToSet;
            Cinderella.CinderellaCore.RemovePhotoFromSetCompleted -= OnPhotoRemovedFromSet;

            Anaconda.AnacondaCore.PhotoSetPhotosException -= OnPhotoStreamException;

            base.OnRemovedFromJournal(e);
        }

        private void OnPageAppeared()
        {
            this.DataContext = PhotoSetSource;

            // Show loading progress indicator
            StatusLabel.Visibility = Visibility.Visible;
            PhotoStreamListView.Visibility = Visibility.Collapsed;
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = AppResources.GroupLoadingPhotosText;

            // Refresh first page
            Anaconda.AnacondaCore.GetPhotoSetPhotosAsync(PhotoSetSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });

            // App bar
            ApplicationBar = Resources["PhotoPageAppBar"] as ApplicationBar;
        }

        // Cannot load photo set photos
        private void OnPhotoStreamException(object sender, GetPhotoSetPhotosExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.PhotoSetId != PhotoSetSource.ResourceId)
                    return;

                SystemTray.ProgressIndicator.IsVisible = false;

                if (PhotoSetSource.Photos.Count == 0)
                {
                    StatusLabel.Text = AppResources.GenericPhotoLoadingErrorText;
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;
                }
            });
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
                    StatusLabel.Text = AppResources.GenericNoContentFound;
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StatusLabel.Visibility = Visibility.Collapsed;
                    PhotoStreamListView.Visibility = Visibility.Visible;

                    List<PhotoGroup> newGroups = null;
                    if (PhotoCollection.Count >= 1 && PhotoCollection[0].IsHeadline)
                    {
                        newGroups = rendererFactory.GeneratePhotoGroups(e.NewPhotos);
                    }
                    else
                    {
                        newGroups = rendererFactory.GeneratePhotoGroupsWithHeadline(e.NewPhotos);
                    }

                    foreach (var group in newGroups)
                    {
                        PhotoCollection.Add(group);
                    }
                }
            });
        }


        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            PhotoGroup photoGroup = e.Container.Content as PhotoGroup;
            if (photoGroup == null)
                return;

            int index = PhotoCollection.IndexOf(photoGroup);

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
            SystemTray.ProgressIndicator.Text = AppResources.GroupLoadingPhotosText;

            Anaconda.AnacondaCore.GetPhotoSetPhotosAsync(PhotoSetSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

        private PhotoSetAddPhotoView addPhotoView;
        private void AddPhotoButton_Click(object sender, EventArgs e)
        {
            addPhotoView = new PhotoSetAddPhotoView(PhotoSetSource);
            var addPhotoDialog = ModalPopup.Show(addPhotoView, AppResources.PhotoCollectionAddToSetText, new List<string> { "Done Adding Photos" });
        }

        private void OnPhotoAddedToSet(object sender, AddPhotoToSetCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.SetId != PhotoSetSource.ResourceId)
                    return;

                Photo newPhoto = Cinderella.CinderellaCore.PhotoCache[e.PhotoId];
                List<PhotoGroup> newGroups = rendererFactory.GeneratePhotoGroups(new List<Photo> { newPhoto });
                PhotoCollection.Insert(0, newGroups[0]);

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

                PhotoGroup groupToReflow = null;
                Photo photoToRemove = null;
                // Find the respective photogroup object
                foreach (var group in PhotoCollection)
                {
                    foreach (var photo in group.Photos)
                    {
                        if (groupToReflow != null)
                        {
                            break;
                        }

                        if (photo.ResourceId == e.PhotoId)
                        {
                            photoToRemove = photo;
                            groupToReflow = group;
                            break;
                        }
                    }
                }

                // Reflow or delete the group
                if (groupToReflow.Photos.Count == 1)
                {
                    PhotoCollection.Remove(groupToReflow);
                }
                else
                {
                    // Reflow the group
                    int oldIndex = PhotoCollection.IndexOf(groupToReflow);
                    PhotoCollection.Remove(groupToReflow);
                    PhotoCollection.Insert(oldIndex, groupToReflow);
                }

                if (PhotoSetSource.Photos.Count == 0)
                {
                    StatusLabel.Text = AppResources.GenericNoContentFound;
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;
                }
            });

        }

        
    }
}