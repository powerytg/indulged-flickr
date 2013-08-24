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
            TopicCollection.Clear();
            foreach (var topic in GroupSource.Topics)
            {
                TopicCollection.Add(topic);
            }
        }

        // Photo data source
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
            Cinderella.CinderellaCore.AddTopicCompleted += OnAddTopicComplete;
        }

        // Photo stream updated
        private void OnTopicsUpdated(object sender, GroupTopicsUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.NewTopics.Count == 0 || e.GroupId != GroupSource.ResourceId)
                    return;

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
                SystemTray.ProgressIndicator.Text = "loading topics";
                SystemTray.ProgressIndicator.IsVisible = true;

                int page = GroupSource.Topics.Count / Anaconda.DefaultItemsPerPage + 1;
                Anaconda.AnacondaCore.GetGroupTopicsAsync(GroupSource.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
            }
        }

    }
}
