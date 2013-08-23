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
        public FlickrGroup Group { get; set; }

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
        }

        // Photo stream updated
        private void OnTopicsUpdated(object sender, GroupTopicsUpdatedEventArgs e)
        {
            if (e.NewTopics.Count == 0 || e.GroupId != Group.ResourceId)
                return;

            Dispatcher.BeginInvoke(() => {
                foreach (var topic in e.NewTopics)
                {
                    TopicCollection.Add(topic);
                }
            });
        }

        private void OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            Topic topic = e.Container.Content as Topic;
            if (topic == null)
                return;

            int index = TopicCollection.IndexOf(topic);

            bool canLoad = (Group.Topics.Count < Group.TopicCount);
            if (TopicCollection.Count - index <= 2 && canLoad)
            {
                SystemTray.ProgressIndicator.Text = "loading topics";
                SystemTray.ProgressIndicator.IsVisible = true;

                int page = Group.Topics.Count / Anaconda.DefaultItemsPerPage + 1;
                Anaconda.AnacondaCore.GetGroupTopicsAsync(Group.ResourceId, new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
            }
        }

    }
}
