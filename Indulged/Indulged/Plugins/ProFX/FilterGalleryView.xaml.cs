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
using Indulged.Plugins.ProFX.Filters;
using Indulged.Plugins.ProFX.Events;

namespace Indulged.Plugins.ProFX
{
    public partial class FilterGalleryView : UserControl
    {
        // Filter class table
        private Dictionary<string, FilterBase> filterTable = new Dictionary<string, FilterBase>();

        // Binding source
        private ObservableCollection<string> filterList = new ObservableCollection<string>();

        // Constructor
        public FilterGalleryView()
        {
            InitializeComponent();

            // Generate the filter list
            PopulateFilterList();
        }

        // Populate the filter list
        private void PopulateFilterList()
        {
            foreach (FilterBase filter in ImageProcessingPage.AvailableFilters)
            {
                if (ImageProcessingPage.GetAppliedFilterByName(filter.DisplayName) != null)
                    continue;

                filterList.Add(filter.DisplayName);
                filterTable[filter.DisplayName] = filter;
            }

            GalleryListView.ItemsSource = filterList;

            if (filterList.Count == 0)
            {
                NoMoreFiltersLabel.Visibility = Visibility.Visible;
                GalleryListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoMoreFiltersLabel.Visibility = Visibility.Collapsed;
                GalleryListView.Visibility = Visibility.Visible;
            }
        }

        private void BackToEditorButton_Click(object sender, RoutedEventArgs e)
        {
            ImageProcessingPage.RequestFilterListView(this, null);
        }

        private void GalleryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filterName = (string)GalleryListView.SelectedItem;
            FilterBase selectedFilter = filterTable[filterName];

            var evt = new AddFilterEventArgs();
            evt.Filter = selectedFilter;
            ImageProcessingPage.RequestAddFilter(this, evt);
        }

    }
}
