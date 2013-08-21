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

            // Get first page of photos
            Anaconda.AnacondaCore.GetGroupPhotosAsync(GroupSource.ResourceId, new Dictionary<string, string> { {"page" , "1"}, {"per_page" , Anaconda.DefaultItemsPerPage.ToString()} });

            // Get first page of topics
            Anaconda.AnacondaCore.GetGroupTopicsAsync(GroupSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

        // Constructor
        public GroupPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string groupId = NavigationContext.QueryString["group_id"];
            
            this.GroupSource = Cinderella.CinderellaCore.GroupCache[groupId];
            this.DataContext = GroupSource;
        }
    }
}