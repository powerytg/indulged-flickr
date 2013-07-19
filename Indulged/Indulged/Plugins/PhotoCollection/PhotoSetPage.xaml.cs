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

namespace Indulged.Plugins.PhotoCollection
{
    public partial class PhotoSetPage : PhoneApplicationPage
    {
        // Data source
        public PhotoSet PhotoSetSource { get; set; }

        private ObservableCollection<PhotoGroup> PhotoCollection;

        // Constructor
        public PhotoSetPage()
        {
            InitializeComponent();

            // Initialize data providers
            PhotoCollection = new ObservableCollection<PhotoGroup>();
            PhotoStreamListView.ItemsSource = PhotoCollection;

            // Events
            Cinderella.CinderellaCore.PhotoSetPhotosUpdated += OnPhotoStreamUpdated;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string setId = NavigationContext.QueryString["photoset_id"];
            PhotoSetSource = Cinderella.CinderellaCore.PhotoSetCache[setId];
            this.DataContext = PhotoSetSource;

            // Get first page of photo stream in the set
            Anaconda.AnacondaCore.GetPhotoSetPhotosAsync(PhotoSetSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });

        }

        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, PhotoSetPhotosUpdatedEventArgs e)
        {
            if (e.NewPhotos.Count == 0 || e.PhotoSetId != PhotoSetSource.ResourceId)
                return;

            List<PhotoGroup> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos, PhotoSetSource.ResourceId, "PhotoSet");
            foreach (var group in newGroups)
            {
                PhotoCollection.Add(group);
            }
        }


        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
        }
    }
}