using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Indulged.Plugins.Group
{
    public partial class GroupDiscussionPage : PhoneApplicationPage
    {
        private FlickrGroup group;
        private Topic topic;

        // Reply data source
        public ObservableCollection<ModelBase> ReplyCollection { get; set; }

        // Constructor
        public GroupDiscussionPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string groupId = NavigationContext.QueryString["group_id"];
            group = Cinderella.CinderellaCore.GroupCache[groupId];

            string topicId = NavigationContext.QueryString["topic_id"];
            topic = group.TopicCache[topicId];

            // Reply list
            ReplyCollection = new ObservableCollection<ModelBase>();
            ReplyListView.ItemsSource = ReplyCollection;

            // Add topic as first item
            ReplyCollection.Add(topic);

            foreach (var reply in topic.Replies)
            {
                ReplyCollection.Add(reply);
            }

            // Config app bar
            ApplicationBar = Resources["ReplyListAppBar"] as ApplicationBar;

            // Events
            Cinderella.CinderellaCore.TopicRepliesUpdated += OnRepliesUpdated;
            Anaconda.AnacondaCore.AddTopicReplyException += OnAddReplyException;
            Cinderella.CinderellaCore.AddTopicReplyCompleted += OnAddReplyComplete;

            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.Text = "retrieving replies";
            SystemTray.ProgressIndicator.IsVisible = true;

            // Refresh reply list
            Anaconda.AnacondaCore.GetTopicRepliesAsync(topicId, groupId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

        private void OnRepliesUpdated(object sender, TopicRepliesUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.TopicId != topic.ResourceId)
                    return;

                foreach (var reply in e.NewReplies)
                {
                    if (!ReplyCollection.Contains(reply))
                        ReplyCollection.Add(reply);
                }

                SystemTray.ProgressIndicator.IsVisible = false;

            });
        }

        private void ReplyListView_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            TopicReply reply = e.Container.Content as TopicReply;
            if (reply == null)
                return;

            int index = ReplyCollection.IndexOf(reply);

            bool canLoad = (topic.Replies.Count < topic.ReplyCount);
            if (ReplyCollection.Count - index <= 2 && canLoad)
            {
                SystemTray.ProgressIndicator.Text = "retrieving replies";
                SystemTray.ProgressIndicator.IsVisible = true;

                int page = topic.Replies.Count / Anaconda.DefaultItemsPerPage + 1;
                Anaconda.AnacondaCore.GetTopicRepliesAsync(topic.ResourceId, group.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
            }
        }

        private void RefreshReplyListButton_Click(object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator.Text = "retrieving replies";
            SystemTray.ProgressIndicator.IsVisible = true;

            Anaconda.AnacondaCore.GetTopicRepliesAsync(topic.ResourceId, group.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }
        
    }
}