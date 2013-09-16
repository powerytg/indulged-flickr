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
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Anaconda;
using Indulged.API.Cinderella.Models;

namespace Indulged.Plugins.Search
{
    public partial class PhotoSearchResultView : UserControl
    {
        public string SearchSessionId { get; set; }

        public string Query { get; set; }
        public string Tags { get; set; }

        public int Page { get; set; }
        private int perPage = 20;
        public int TotalCount { get; set; }

        private ObservableCollection<Photo> _photos = new ObservableCollection<Photo>();


        // Constructor
        public PhotoSearchResultView()
        {
            InitializeComponent();

            NoResultLabel.Visibility = Visibility.Collapsed;
            ResultListView.Visibility = Visibility.Collapsed;
            LoadingView.Visibility = Visibility.Visible;

            ResultListView.ItemsSource = _photos;

            // Events
            Cinderella.CinderellaCore.PhotoSearchCompleted += OnPhotoSearchResult;
        }

        public void PerformSearch()
        {
            SearchSessionId = Guid.NewGuid().ToString().Replace("-", null);
            Anaconda.AnacondaCore.SearchPhotoAsync(SearchSessionId, Query, Tags, new Dictionary<string, string> { { "page", "1" }, { "per_page", perPage.ToString() } });
        }

        private void OnPhotoSearchResult(object sender, PhotoSearchResultEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.SearchSessionId != SearchSessionId)
                    return;

                if (_photos.Count == 0 && e.Photos.Count == 0)
                {
                    NoResultLabel.Visibility = Visibility.Visible;
                    ResultListView.Visibility = Visibility.Collapsed;
                    LoadingView.Visibility = Visibility.Collapsed;

                    return;
                }

                // Add new photos
                foreach (Photo photo in e.Photos)
                {
                    if (!_photos.Contains(photo))
                        _photos.Add(photo);
                }

                NoResultLabel.Visibility = Visibility.Collapsed;
                ResultListView.Visibility = Visibility.Visible;
                LoadingView.Visibility = Visibility.Collapsed;

                TotalCount = e.TotalCount;
            });
        }

        public void OnRemovedFromJournal()
        {
            ResultListView.ItemsSource = null;
            _photos.Clear();
            _photos = null;

            Cinderella.CinderellaCore.PhotoSearchCompleted -= OnPhotoSearchResult;
        }
    }
}
