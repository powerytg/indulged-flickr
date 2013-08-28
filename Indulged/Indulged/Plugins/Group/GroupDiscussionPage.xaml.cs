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
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.Group
{
    public partial class GroupDiscussionPage : PhoneApplicationPage
    {
        private FlickrGroup group;
        private Topic topic;

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

            User topicUser = topic.Author;
            AuthorAvatarView.Source = new BitmapImage(new Uri(topicUser.AvatarUrl));
            AuthorLabelView.Text = topicUser.Name;

            TopicView.TopicSource = topic;
        }
    }
}