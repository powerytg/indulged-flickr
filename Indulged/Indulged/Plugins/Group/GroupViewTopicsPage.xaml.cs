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
using Indulged.Resources;

namespace Indulged.Plugins.Group
{
    public partial class GroupViewTopicsPage : UserControl
    {
        public static readonly DependencyProperty GroupSourceProperty = DependencyProperty.Register("GroupSource", typeof(FlickrGroup), typeof(GroupViewTopicsPage), new PropertyMetadata(OnGroupSourcePropertyChanged));

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
            ((GroupViewTopicsPage)sender).OnGroupSourceChanged();
        }

        protected virtual void OnGroupSourceChanged()
        {
            if (GroupSource == null)
                return;

            TopicCollection.Clear();

            if (GroupSource.Topics.Count == 0)
            {
                StatusLabel.Text = AppResources.GroupNoActiveTopicsText;
                StatusLabel.Visibility = Visibility.Visible;
                TopicListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                foreach (var topic in GroupSource.Topics)
                {
                    TopicCollection.Add(topic);
                }

                StatusLabel.Visibility = Visibility.Collapsed;
                TopicListView.Visibility = Visibility.Visible;
            }
        }

        // Topic data source
        public ObservableCollection<Topic> TopicCollection { get; set; }

        // Constructor
        public GroupViewTopicsPage()
        {
            InitializeComponent();

            // Initialize data providers
            TopicCollection = new ObservableCollection<Topic>();
            TopicListView.ItemsSource = TopicCollection;

            // Events
            Cinderella.CinderellaCore.GroupTopicsUpdated += OnTopicsUpdated;
            Anaconda.AnacondaCore.GroupTopicsException += OnTopicsException;

            Cinderella.CinderellaCore.AddTopicCompleted += OnAddTopicComplete;
        }

        private bool eventListenersRemoved = false;
        public void RemoveEventListeners()
        {
            if (eventListenersRemoved)
                return;

            eventListenersRemoved = true;

            Cinderella.CinderellaCore.GroupTopicsUpdated -= OnTopicsUpdated;
            Anaconda.AnacondaCore.GroupTopicsException -= OnTopicsException;

            Cinderella.CinderellaCore.AddTopicCompleted -= OnAddTopicComplete;

            TopicListView.ItemsSource = null;
            TopicCollection.Clear();
            TopicCollection = null;

            GroupSource = null;
        }

        // Topic list cannot be loaded
        private void OnTopicsException(object sender, GetGroupTopicsExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if ( e.GroupId != GroupSource.ResourceId)
                    return;

                if (TopicCollection.Count == 0)
                {
                    StatusLabel.Text = "Cannot load topics";
                    StatusLabel.Visibility = Visibility.Visible;
                    TopicListView.Visibility = Visibility.Collapsed;
                }
            });
        }

        // Topic list updated
        private void OnTopicsUpdated(object sender, GroupTopicsUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.GroupId != GroupSource.ResourceId)
                    return;

                if (e.NewTopics.Count == 0 && TopicCollection.Count == 0)
                {
                    StatusLabel.Text = AppResources.GroupNoActiveTopicsText;
                    StatusLabel.Visibility = Visibility.Visible;
                    TopicListView.Visibility = Visibility.Collapsed;
                    return;
                }

                if (e.NewTopics.Count == 0)
                    return;

                StatusLabel.Visibility = Visibility.Collapsed;
                TopicListView.Visibility = Visibility.Visible;

                foreach (var topic in e.NewTopics)
                {
                    if(!TopicCollection.Contains(topic))
                        TopicCollection.Add(topic);
                }
            });
        }

        private void OnAddTopicComplete(object sender, AddTopicCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (GroupSource.ResourceId != e.GroupId)
                    return;

                StatusLabel.Visibility = Visibility.Collapsed;
                TopicListView.Visibility = Visibility.Visible;

                TopicCollection.Insert(0, e.newTopic);
            });
        }

        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            Topic topic = e.Container.Content as Topic;
            if (topic == null)
                return;

            int index = TopicCollection.IndexOf(topic);

            bool canLoad = (GroupSource.Topics.Count < GroupSource.TopicCount);
            if (TopicCollection.Count - index <= 2 && canLoad)
            {
                SystemTray.ProgressIndicator.Text = AppResources.GroupLoadingTopicsText;
                SystemTray.ProgressIndicator.IsVisible = true;

                int page = GroupSource.Topics.Count / Anaconda.DefaultItemsPerPage + 1;
                Anaconda.AnacondaCore.GetGroupTopicsAsync(GroupSource.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
            }
        }

        private void TopicListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TopicListView.SelectedItem == null)
                return;

            Topic selectedTopic =(Topic)TopicListView.SelectedItem;            
            string urlString = "/Plugins/Group/GroupDiscussionPage.xaml?group_id=" + GroupSource.ResourceId + "&topic_id=" + selectedTopic.ResourceId;

            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
            currentPage.NavigationService.Navigate(new Uri(urlString, UriKind.Relative));
            TopicListView.SelectedItem = null;
        }

    }
}
