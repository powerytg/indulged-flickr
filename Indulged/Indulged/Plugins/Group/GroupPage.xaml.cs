using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella;
using Indulged.API.Anaconda;

namespace Indulged.Plugins.Group
{
    public partial class GroupPage : PhoneApplicationPage
    {
        public static readonly DependencyProperty GroupSourceProperty = DependencyProperty.Register("GroupSource", typeof(FlickrGroup), typeof(GroupPage), new PropertyMetadata(OnGroupSourcePropertyChanged));

        public FlickrGroup GroupSource
        {
            get
            {
                return (FlickrGroup)GetValue(GroupSourceProperty);
            }
            set
            {
                SetValue(GroupSourceProperty, value);
            }
        }

        public static void OnGroupSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((GroupPage)sender).OnGroupSourceChanged();
        }

        protected virtual void OnGroupSourceChanged()
        {
            PhotoPageView.Group = GroupSource;
            TopicPageView.Group = GroupSource;

            // Show loading progress indicator
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "loading photos";

            // Get first page of photos
            Anaconda.AnacondaCore.GetGroupPhotosAsync(GroupSource.ResourceId, new Dictionary<string, string> { {"page" , "1"}, {"per_page" , Anaconda.DefaultItemsPerPage.ToString()} });

            // Get first page of topics
            Anaconda.AnacondaCore.GetGroupTopicsAsync(GroupSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

        // Constructor
        public GroupPage()
        {
            InitializeComponent();

            // Events
            Cinderella.CinderellaCore.GroupPhotoListUpdated += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (e.GroupId == GroupSource.ResourceId)
                        SystemTray.ProgressIndicator.IsVisible = false;
                });
            };

            Cinderella.CinderellaCore.GroupTopicsUpdated += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (e.GroupId == GroupSource.ResourceId)
                        SystemTray.ProgressIndicator.IsVisible = false;
                });
            };

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string groupId = NavigationContext.QueryString["group_id"];
            
            this.GroupSource = Cinderella.CinderellaCore.GroupCache[groupId];
            this.DataContext = GroupSource;

            // Config app bar
            ApplicationBar = Resources["PhotoPageAppBar"] as ApplicationBar;
        }

        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PanoramaView.SelectedIndex == 0)
            {
                ApplicationBar = Resources["PhotoPageAppBar"] as ApplicationBar;
            }
            else if (PanoramaView.SelectedIndex == 1)
            {
                ApplicationBar = Resources["TopicPageAppBar"] as ApplicationBar;
            }
        }

        private void RefreshPhotoListButton_Click(object sender, EventArgs e)
        {
            // Show progress bar
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "loading photos";

            // Refresh group photos
            Anaconda.AnacondaCore.GetGroupPhotosAsync(GroupSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

        private void RefreshTopicListButton_Click(object sender, EventArgs e)
        {
            // Show progress bar
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "loading topics";

            // Refresh group photos
            Anaconda.AnacondaCore.GetGroupTopicsAsync(GroupSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });

        }
    }
}