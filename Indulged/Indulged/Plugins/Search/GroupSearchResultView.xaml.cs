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
    public partial class GroupSearchResultView : UserControl
    {
        public string SearchSessionId { get; set; }

        public string Query { get; set; }

        public int Page { get; set; }
        private int perPage = 20;
        public int TotalCount { get; set; }

        private ObservableCollection<FlickrGroup> _groups = new ObservableCollection<FlickrGroup>();


        // Constructor
        public GroupSearchResultView()
        {
            InitializeComponent();

            NoResultLabel.Visibility = Visibility.Collapsed;
            ResultListView.Visibility = Visibility.Collapsed;
            LoadingView.Visibility = Visibility.Visible;

            ResultListView.ItemsSource = _groups;

            // Events
            Cinderella.CinderellaCore.GroupSearchCompleted += OnGroupSearchResult;
        }

        public void PerformSearch()
        {
            SearchSessionId = Guid.NewGuid().ToString().Replace("-", null);
            Anaconda.AnacondaCore.SearchGroupsAsync(SearchSessionId, Query, new Dictionary<string, string> { { "page", "1" }, { "per_page", perPage.ToString() } });
        }

        private void OnGroupSearchResult(object sender, GroupSearchResultEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.SearchSessionId != SearchSessionId)
                    return;

                if (_groups.Count == 0 && e.Groups.Count == 0)
                {
                    NoResultLabel.Visibility = Visibility.Visible;
                    ResultListView.Visibility = Visibility.Collapsed;
                    LoadingView.Visibility = Visibility.Collapsed;

                    return;
                }

                // Add new photos
                foreach (FlickrGroup group in e.Groups)
                {
                    if (!_groups.Contains(group))
                        _groups.Add(group);
                }

                NoResultLabel.Visibility = Visibility.Collapsed;
                ResultListView.Visibility = Visibility.Visible;
                LoadingView.Visibility = Visibility.Collapsed;

                int page = _groups.Count / perPage + 1;
                TotalCount = e.TotalCount;
            });
        }

        // Implementation of inifinite scrolling
        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            FlickrGroup groupItem = e.Container.Content as FlickrGroup;
            if (groupItem == null)
                return;

            int index = _groups.IndexOf(groupItem);
            bool canLoad = (_groups.Count < TotalCount);
            if (!canLoad)
                return;

            if (_groups.Count - index <= 2 && canLoad)
            {
                int page = _groups.Count / perPage + 1;
                Anaconda.AnacondaCore.SearchGroupsAsync(SearchSessionId, Query, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", perPage.ToString() } });
            }
        }
    }
}
