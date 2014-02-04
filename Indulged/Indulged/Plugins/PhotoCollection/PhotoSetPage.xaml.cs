using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Models;
using Indulged.Plugins.Common.PhotoGroupRenderers;
using Indulged.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Navigation;

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
        }

        private bool executedOnce;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (executedOnce)
                return;

            executedOnce = true;

            // Events
            InitializeEventListeners();

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
            RemoveEventListeners();

            base.OnRemovedFromJournal(e);
        }

        private void OnPageAppeared()
        {
            this.DataContext = PhotoSetSource;

            // Initial items
            if (PhotoSetSource.Photos.Count > 0)
            {
                var exsitsingGroups = rendererFactory.GeneratePhotoGroupsWithHeadline(PhotoSetSource.Photos);
                foreach (var group in exsitsingGroups)
                {
                    PhotoCollection.Add(group);
                }

            }
            else
            {
                StatusLabel.Visibility = Visibility.Visible;
                PhotoStreamListView.Visibility = Visibility.Collapsed;
            }

            // Show loading progress indicator
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


        
    }
}