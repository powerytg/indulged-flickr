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
        private Dictionary<string, FilterButton> filterButtonTable = new Dictionary<string, FilterButton>();

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

                FilterButton button = new FilterButton();
                button.Content = filter.DisplayName;
                button.Filter = filter;
                button.Selected = false;
                button.Click += OnFilterButtonClicked;

                FilterListView.Children.Add(button);
                filterButtonTable[filter.DisplayName] = button;
            }
        }

        private void OnFilterButtonClicked(object sender, RoutedEventArgs e)
        {
            FilterButton button = (FilterButton)sender;
            button.Selected = true;

            var evt = new AddFilterEventArgs();
            evt.Filter = button.Filter;
            ImageProcessingPage.RequestAddFilter(this, evt);
        }

    }
}
