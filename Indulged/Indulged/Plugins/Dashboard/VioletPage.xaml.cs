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

using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Events;
using Indulged.Plugins.Dashboard.Events;

namespace Indulged.Plugins.Dashboard
{
    public partial class VioletPage : UserControl, IDashboardPage
    {
        public string BackgroundImageUrl
        {
            get
            {
                return "/Assets/Chrome/VioletBackground.png";
            }
        }

        // Photo data source
        public ObservableCollection<List<Photo>> PhotoCollection { get; set; }

        // Constructor
        public VioletPage()
        {
            InitializeComponent();

            // Initialize data providers
            PhotoCollection = new ObservableCollection<List<Photo>>();
            PhotoStreamListView.ItemsSource = PhotoCollection;

            // Events
            Cinderella.CinderellaCore.PhotoStreamUpdated += OnPhotoStreamUpdated;
        }

        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, PhotoStreamUpdatedEventArgs e)
        {
            if (e.NewPhotos.Count == 0 || e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                return;

            List<List<Photo>> newGroups = VioletPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos);
            foreach (var group in newGroups)
            {
                PhotoCollection.Add(group);
            }
        }

        // Implementation of inifinite scrolling
        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            List<Photo> photoGroup = e.Container.Content as List<Photo>;
            if (photoGroup == null)
                return;

            int index = PhotoCollection.IndexOf(photoGroup);
            User currentUser = Cinderella.CinderellaCore.CurrentUser;

            if (PhotoCollection.Count - index <= 2 && !currentUser.IsLoadingPhotoStream && currentUser.Photos.Count < currentUser.PhotoCount)
            {
                int page = currentUser.Photos.Count / 100 + 1;
                System.Diagnostics.Debug.WriteLine("page=" + page.ToString());
                Anaconda.AnacondaCore.GetPhotoStreamAsync(currentUser.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", "100" } });
            }
        }

    }
}
