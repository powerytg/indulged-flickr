using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.Plugins.Dashboard;
using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.Plugins.Common.PhotoGroupRenderers;
using Indulged.Resources;

namespace Indulged.Plugins.Profile
{
    public partial class UserProfilePhotoPage : UserControl
    {
        private User _user;
        public User UserSource
        {
            get
            {
                return _user;
            }

            set
            {
                _user = value;

                if (_user == null)
                    return;

                if (_user.Photos.Count > 0)
                {
                    List<PhotoGroup> photoGroups = CommonPhotoGroupFactory.GeneratePhotoGroup(_user.Photos, _user.ResourceId, "UserPhotoStream");
                    foreach (var group in photoGroups)
                    {
                        PhotoCollection.Add(group);
                    }

                }
            }
        }

        // Photo data source
        public ObservableCollection<PhotoGroup> PhotoCollection { get; set; }

        // Constructor
        public UserProfilePhotoPage()
        {
            InitializeComponent();

            // Initialize data providers
            PhotoCollection = new ObservableCollection<PhotoGroup>();
            PhotoStreamListView.ItemsSource = PhotoCollection;

            // Events
            Cinderella.CinderellaCore.PhotoStreamUpdated += OnPhotoStreamUpdated;
            Anaconda.AnacondaCore.PhotoStreamException += OnPhotoStreamException;
        }

        private bool eventListenersRemoved = false;
        public void RemoveEventListeners()
        {
            if (eventListenersRemoved)
                return;

            eventListenersRemoved = true;

            Cinderella.CinderellaCore.PhotoStreamUpdated -= OnPhotoStreamUpdated;
            Anaconda.AnacondaCore.PhotoStreamException -= OnPhotoStreamException;
            UserSource = null;
        }

        // Cannot load photo stream
        private void OnPhotoStreamException(object sender, GetPhotoStreamExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.UserId != UserSource.ResourceId)
                    return;

                if(SystemTray.ProgressIndicator != null)
                    SystemTray.ProgressIndicator.IsVisible = false;

                StatusLabel.Text = AppResources.GenericPhotoLoadingErrorText;
                StatusLabel.Visibility = Visibility.Visible;
                PhotoStreamListView.Visibility = Visibility.Collapsed;
            });
        }

        // Photo stream updated
        private void OnPhotoStreamUpdated(object sender, PhotoStreamUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.UserId != UserSource.ResourceId)
                    return;

                if (SystemTray.ProgressIndicator != null)
                    SystemTray.ProgressIndicator.IsVisible = false;

                if (e.NewPhotos.Count == 0 && PhotoCollection.Count == 0)
                {
                    StatusLabel.Text = AppResources.GenericNoContentFound;
                    StatusLabel.Visibility = Visibility.Visible;
                    PhotoStreamListView.Visibility = Visibility.Collapsed;
                    return;
                }

                StatusLabel.Visibility = Visibility.Collapsed;
                PhotoStreamListView.Visibility = Visibility.Visible;

                if (e.NewPhotos.Count == 0)
                    return;

                if (e.Page == 1)
                    PhotoCollection.Clear();

                List<PhotoGroup> newGroups = CommonPhotoGroupFactory.GeneratePhotoGroup(e.NewPhotos, UserSource.ResourceId, "UserPhotoStream");
                foreach (var group in newGroups)
                {
                    PhotoCollection.Add(group);
                }
            });
        }

        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            PhotoGroup photoGroup = e.Container.Content as PhotoGroup;
            if (photoGroup == null)
                return;

            int index = PhotoCollection.IndexOf(photoGroup);

            bool canLoad = (UserSource.Photos.Count < UserSource.PhotoCount);
            if (PhotoCollection.Count - index <= 2 && canLoad)
            {
                // Show progress indicator
                if (SystemTray.ProgressIndicator != null)
                    SystemTray.ProgressIndicator.IsVisible = true;

                int page = UserSource.Photos.Count / Anaconda.DefaultItemsPerPage + 1;
                Anaconda.AnacondaCore.GetPhotoStreamAsync(UserSource.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
            }

        }
    }
}
