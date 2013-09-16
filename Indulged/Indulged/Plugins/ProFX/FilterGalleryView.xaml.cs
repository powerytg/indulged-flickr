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
using Indulged.Plugins.Chrome.Events;
using System.Windows.Media;
using Indulged.Plugins.Chrome;

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
            
            ApplyTheme();

            // Generate the filter list
            PopulateFilterList();

            // Events
            ThemeManager.ThemeChanged += OnThemeChanged;
            ImageProcessingPage.RequestDeleteFilter += OnRequestDeleteFilter;
        }

        private bool eventListenersRemoved = false;
        public void RemoveEventListeners()
        {
            if (eventListenersRemoved)
                return;

            eventListenersRemoved = true;

            ThemeManager.ThemeChanged -= OnThemeChanged;
            ImageProcessingPage.RequestDeleteFilter -= OnRequestDeleteFilter;

            filterButtonTable.Clear();
            filterButtonTable = null;
        }

        private void ApplyTheme()
        {
            if (ThemeManager.CurrentTheme == Themes.Dark)
            {
                LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(216, 0, 0, 0));
            }
            else
            {
                LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(216, 0xff, 0xff, 0xff));
            }
        }

        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            ApplyTheme();
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

            if (ImageProcessingPage.AppliedFilters.Contains(button.Filter))
            {
                var evt = new RequestFilterViewEventArgs();
                evt.Filter = button.Filter;
                ImageProcessingPage.RequestFilterView(this, evt);
            }
            else
            {
                var evt = new AddFilterEventArgs();
                evt.Filter = button.Filter;
                ImageProcessingPage.RequestAddFilter(this, evt);
            }

        }

        private void OnRequestDeleteFilter(object sender, DeleteFilterEventArgs e)
        {
            // Reset filter button
            FilterButton button = filterButtonTable[e.Filter.DisplayName];
            button.Selected = false;
        }

    }
}
