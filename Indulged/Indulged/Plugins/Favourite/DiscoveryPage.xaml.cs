using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.API.Cinderella;
using System.Collections.ObjectModel;
using Indulged.API.Cinderella.Models;
using Indulged.API.Anaconda;
using Indulged.PolKit;
using Indulged.API.Cinderella.Events;
using Indulged.API.Avarice.Controls;

namespace Indulged.Plugins.Favourite
{
    public partial class DiscoveryPage : PhoneApplicationPage
    {
        private ObservableCollection<Photo> _photos = new ObservableCollection<Photo>();

        // Constructor
        public DiscoveryPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Show loading progress indicator
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = false;
            SystemTray.ProgressIndicator.Text = "retrieving photos";

            // Events
            Cinderella.CinderellaCore.DiscoveryStreamUpdated += OnDiscoveryStreamUpdated;
            Anaconda.AnacondaCore.DiscoveryStreamException += OnDiscoveryStreamException;

            if (Cinderella.CinderellaCore.DiscoveryList.Count > 0)
            {
                StatusLabel.Visibility = Visibility.Collapsed;
                ResultListView.Visibility = Visibility.Visible;
                _photos.Clear();
                foreach (var photo in Cinderella.CinderellaCore.DiscoveryList)
                {
                    _photos.Add(photo);
                }
            }
            else
            {
                StatusLabel.Visibility = Visibility.Visible;

                SystemTray.ProgressIndicator.IsVisible = true;
                var currentUser = Cinderella.CinderellaCore.CurrentUser;
                Anaconda.AnacondaCore.GetDiscoveryStreamAsync(new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
            }

            ResultListView.ItemsSource = _photos;
        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            Anaconda.AnacondaCore.DiscoveryStreamException -= OnDiscoveryStreamException;
            Cinderella.CinderellaCore.DiscoveryStreamUpdated -= OnDiscoveryStreamUpdated;

            ResultListView.ItemsSource = null;
            _photos.Clear();
            _photos = null;

            base.OnRemovedFromJournal(e);
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

        // Can't load discovery stream
        private void OnDiscoveryStreamException(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (_photos.Count == 0)
                {
                    StatusLabel.Visibility = Visibility.Visible;
                    StatusLabel.Text = "Cannot load discovery stream";
                    ResultListView.Visibility = Visibility.Collapsed;

                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;
                }
            });
        }

        // Discovery stream updated
        private void OnDiscoveryStreamUpdated(object sender, DiscoveryStreamUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (SystemTray.ProgressIndicator != null)
                    SystemTray.ProgressIndicator.IsVisible = false;

                if (Cinderella.CinderellaCore.DiscoveryList.Count == 0)
                {
                    StatusLabel.Visibility = Visibility.Visible;
                    StatusLabel.Text = "No content found";
                    ResultListView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StatusLabel.Visibility = Visibility.Collapsed;
                    ResultListView.Visibility = Visibility.Visible;
                }

                if (e.NewPhotos.Count == 0)
                    return;

                foreach (var photo in e.NewPhotos)
                {
                    if(!_photos.Contains(photo))
                        _photos.Add(photo);
                }

            });
        }

        private void ResultListView_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            Photo photo = e.Container.Content as Photo;
            if (photo == null)
                return;

            bool canLoad = canLoad = (!Anaconda.AnacondaCore.IsLoadingDiscoveryStream && Cinderella.CinderellaCore.DiscoveryList.Count < Cinderella.CinderellaCore.TotalDiscoveryPhotosCount && Cinderella.CinderellaCore.TotalDiscoveryPhotosCount != 0);
            int index = _photos.IndexOf(photo);

            if (_photos.Count - index <= 2 && canLoad)
            {
                int page = page = Cinderella.CinderellaCore.DiscoveryList.Count / PolicyKit.StreamItemsCountPerPage + 1;

                SystemTray.ProgressIndicator.IsVisible = true;
                Anaconda.AnacondaCore.GetDiscoveryStreamAsync(new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
            }
        }

        private void RefreshPhotoListButton_Click(object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator.IsVisible = true;
            Anaconda.AnacondaCore.GetDiscoveryStreamAsync(new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });

        }
    }
}