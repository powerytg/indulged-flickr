using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.Plugins.ProFX.Filters;
using Indulged.Plugins.ProFX.Controls;
using Indulged.Plugins.ProFX.Events;

namespace Indulged.Plugins.ProFX.OSD
{
    public partial class ActiveFiltersHUD : UserControl
    {
        // Events
        public EventHandler OnDismiss;
        public EventHandler<RequestFilterEventArgs> RequestFilter;
        public EventHandler RequestCropFilter;
        public EventHandler RequestRotationFilter;
        public EventHandler<DeleteFilterEventArgs> OnDeleteFilter;
        public EventHandler RequestFilterGallery;

        // Reference to the filter manager
        public FXFilterManager FilterManager { get; set; }

        // Constructor
        public ActiveFiltersHUD()
        {
            InitializeComponent();
        }

        public void ShowLoadingView()
        {
            LoadingView.Visibility = Visibility.Visible;
            FilterListView.Visibility = Visibility.Collapsed;
            NoFilterView.Visibility = Visibility.Collapsed;
        }

        public void HideLoadingView()
        {
            LoadingView.Visibility = Visibility.Collapsed;
            FilterListView.Visibility = Visibility.Visible;
        }

        public void UpdateFilterEntries()
        {
            FilterListView.Children.Clear();

            foreach (var filter in FilterManager.AppliedFilters)
            {
                FilterEntryControl entry = new FilterEntryControl();
                entry.Filter = filter;
                entry.Margin = new Thickness(5, 5, 5, 5);
                entry.RequestFilterView += OnDropletTap;
                entry.OnDelete += OnRequestDeleteFilter;
                entry.OnVisibilityChanged += OnFilterVisibilityChanged;
                FilterListView.Children.Add(entry);
            }

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnDismiss != null)
            {
                OnDismiss(this, null);
            }
        }

        private void OSDToggle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (OnDismiss != null)
            {
                OnDismiss(this, null);
            }
        }

        private void OnDropletTap(object sender, EventArgs e)
        {
            FilterEntryControl droplet = (FilterEntryControl)sender;
            if (droplet.Filter == FilterManager.CropFilter)
            {
                if (RequestCropFilter != null)
                {
                    RequestCropFilter(this, null);
                }
            }
            else if (droplet.Filter == FilterManager.RotationFilter)
            {
                if (RequestRotationFilter != null)
                {
                    RequestRotationFilter(this, null);
                }
            }
            else if (RequestFilter != null)
            {
                var evt = new RequestFilterEventArgs();
                evt.Filter = droplet.Filter;
                RequestFilter(this, evt);
            }
        }

        private void OnRequestDeleteFilter(object sender, EventArgs e)
        {
            FilterEntryControl entry = (FilterEntryControl)sender;
            FilterListView.Children.Remove(entry);
            if (entry.Filter == FilterManager.CropFilter)
            {
                FilterManager.DiscardCrop();
            }
            else if (entry.Filter == FilterManager.RotationFilter)
            {
                FilterManager.DiscardRotationFilter();
            }
            else if (FilterListView.Children.Contains(entry))
            {                
                FilterManager.DeleteFilter(entry.Filter);
            }

            if (FilterManager.AppliedFilters.Count == 0)
            {
                FilterListView.Visibility = Visibility.Collapsed;
                NoFilterView.Visibility = Visibility.Visible;
            }
        }

        private void OnFilterVisibilityChanged(object sender, EventArgs e)
        {
            FilterEntryControl entry = (FilterEntryControl)sender;
            FilterManager.PerformInvalidatePreview();
        }

        private void AddFilterLinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (RequestFilterGallery != null)
            {
                RequestFilterGallery(this, null);
            }
        }

    }
}
