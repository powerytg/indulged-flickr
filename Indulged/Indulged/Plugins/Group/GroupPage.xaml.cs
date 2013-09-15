using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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
            if (GroupSource == null)
                return;

            PhotoPageView.Group = GroupSource;
            TopicPageView.GroupSource = GroupSource;

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
            Cinderella.CinderellaCore.GroupPhotoListUpdated += OnPhotoListUpdated;
            Anaconda.AnacondaCore.GroupPhotoException += OnPhotoListException;

            Cinderella.CinderellaCore.GroupTopicsUpdated += OnTopicListUpdated;
            Anaconda.AnacondaCore.GroupTopicsException += OnTopicListException;

            // Events
            Anaconda.AnacondaCore.AddTopicException += OnAddTopicException;
            Cinderella.CinderellaCore.AddTopicCompleted += OnAddTopicComplete;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string groupId = NavigationContext.QueryString["group_id"];

            if (Cinderella.CinderellaCore.GroupCache.ContainsKey(groupId))
            {
                this.GroupSource = Cinderella.CinderellaCore.GroupCache[groupId];
                this.DataContext = GroupSource;

                // Config app bar
                ApplicationBar = Resources["PhotoPageAppBar"] as ApplicationBar;
            }

        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            Anaconda.AnacondaCore.AddTopicException -= OnAddTopicException;
            Cinderella.CinderellaCore.AddTopicCompleted -= OnAddTopicComplete;

            Cinderella.CinderellaCore.GroupPhotoListUpdated -= OnPhotoListUpdated;

            Cinderella.CinderellaCore.GroupTopicsUpdated -= OnTopicListUpdated;

            PhotoPageView.RemoveEventListeners();
            TopicPageView.RemoveEventListeners();


            GroupSource = null;

            base.OnRemovedFromJournal(e);
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

        private void OnPhotoListUpdated(object sender, GroupPhotoListUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.GroupId == GroupSource.ResourceId)
                    SystemTray.ProgressIndicator.IsVisible = false;
            });
        }

        private void OnPhotoListException(object sender, GetGroupPhotosExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.GroupId == GroupSource.ResourceId)
                    SystemTray.ProgressIndicator.IsVisible = false;

            });
        }

        private void OnTopicListUpdated(object sender, GroupTopicsUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.GroupId == GroupSource.ResourceId)
                    SystemTray.ProgressIndicator.IsVisible = false;
            });
        }

        private void OnTopicListException(object sender, GetGroupTopicsExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.GroupId == GroupSource.ResourceId)
                    SystemTray.ProgressIndicator.IsVisible = false;
            });
        }
    }
}