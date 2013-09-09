using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.PolKit;
using Indulged.API.Utils;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Events;
using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;

namespace Indulged.Plugins.Dashboard
{
    public partial class PreludePage : UserControl, IDashboardPage
    {
        public string PageName
        {
            get
            {
                return "PreludePage";
            }
        }

        public string BackgroundImageUrl
        {
            get
            {
                return "/Assets/Chrome/PreludeBackground.png";
            }
        }

        public bool ShouldUseLightBackground
        {
            get
            {
                return false;
            }
        }

        // "Special" streams
        ObservableCollection<PreludeItemModel> FeatureStreams;

        // Photo sets
        ObservableCollection<PhotoSet> PhotoSetList;

        // Group list
        ObservableCollection<FlickrGroup> GroupList;

        // Constructor
        public PreludePage()
        {
            InitializeComponent();

            PhotoSetList = new ObservableCollection<PhotoSet>();
            StreamListView.ItemsSource = PhotoSetList;

            GroupList = new ObservableCollection<FlickrGroup>();
            GroupListView.ItemsSource = GroupList;

            FeatureStreams = new ObservableCollection<PreludeItemModel>();
            FeatureStreams.Add(new PreludeItemModel { Name = "Violet" });
            FeatureStreams.Add(new PreludeItemModel { Name = "Summersalt" });
            FeatureStreams.Add(new PreludeItemModel { Name = "My Photo Stream" });
            FeatureStreams.Add(new PreludeItemModel { Name = "Discovery" });
            FeatureStreams.Add(new PreludeItemModel { Name = "Contacts" });
            FeatureStreams.Add(new PreludeItemModel { Name = "Favourites", Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/Dashboard/Heart.png", UriKind.Relative)) });
            FeatureStreams.Add(new PreludeItemModel { Name = "Search" });
            FeatureListView.ItemsSource = FeatureStreams;

            // Events
            Cinderella.CinderellaCore.PhotoSetListUpdated += OnPhotoSetListUpdated;
            Cinderella.CinderellaCore.GroupListUpdated += OnGroupListUpdated;
            Cinderella.CinderellaCore.JoinGroupComplete += OnGroupJoined;

            Anaconda.AnacondaCore.GetGroupListException += OnGetGroupListException;
            Anaconda.AnacondaCore.GetPhotoSetListException += OnGetPhotoSetListException;
        }

        private void OnGroupJoined(object sender, GroupJoinedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                FlickrGroup group = Cinderella.CinderellaCore.GroupCache[e.GroupId];
                if (group == null || GroupList.Contains(group))
                    return;

                GroupList.Add(group);
            });
        }

        // Stream updated
        private void OnPhotoSetListUpdated(object sender, PhotoSetListUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;

                if (Cinderella.CinderellaCore.PhotoSetList.Count == 0)
                {
                    StreamListStatusLabel.Text = "No photo sets found";
                    StreamListStatusLabel.Visibility = Visibility.Visible;
                    StreamListView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StreamListStatusLabel.Visibility = Visibility.Collapsed;
                    StreamListView.Visibility = Visibility.Visible;

                    PhotoSetList.Clear();
                    foreach (PhotoSet photoset in Cinderella.CinderellaCore.PhotoSetList)
                    {
                        PhotoSetList.Add(photoset);
                    }
                }

            });
        }

        private void OnGetPhotoSetListException(object sender, GetPhotoSetListExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;

                StreamListStatusLabel.IsHitTestVisible = true;
                StreamListStatusLabel.Text = "Tap to retry";
            });
        }

        private void OnGetGroupListException(object sender, GetGroupListExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;

                GroupListStatusLabel.IsHitTestVisible = true;
                GroupListStatusLabel.Text = "Tap to retry";
            });
        }

        // Group list updated
        private void OnGroupListUpdated(object sender, GroupListUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;

                if (Cinderella.CinderellaCore.GroupCache.Count == 0)
                {
                    GroupListStatusLabel.Text = "No groups found";
                    GroupListStatusLabel.Visibility = Visibility.Visible;
                    GroupListView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    GroupListStatusLabel.Visibility = Visibility.Collapsed;
                    GroupListView.Visibility = Visibility.Visible;

                    GroupList.Clear();
                    foreach (FlickrGroup group in e.Groups)
                    {
                        GroupList.Add(group);
                    }
                }

            });
        }

        private void OnGroupListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupListView.SelectedItem == null)
                return;

            FlickrGroup group = (FlickrGroup)GroupListView.SelectedItem;

            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
            currentPage.NavigationService.Navigate(new Uri("/Plugins/Group/GroupPage.xaml?group_id=" + group.ResourceId, UriKind.Relative));
        }

        private void OnFeatureListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FeatureListView.SelectedItem == null)
                return;

            PreludeItemModel entry = (PreludeItemModel)FeatureListView.SelectedItem;
            if (entry.Name == "Favourites")
            {
                if (PolicyKit.VioletPageSubscription == PolicyKit.FavouriteStream)
                    DashboardNavigator.RequestVioletPage(this, null);
                else
                {
                    Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                    PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
                    currentPage.NavigationService.Navigate(new Uri("/Plugins/Favourite/FavouritePage.xaml", UriKind.Relative));
                }

            }
            else if (entry.Name == "Discovery")
            {
                if (PolicyKit.VioletPageSubscription == PolicyKit.DiscoveryStream)
                    DashboardNavigator.RequestVioletPage(this, null);
                else
                {
                    Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                    PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
                    currentPage.NavigationService.Navigate(new Uri("/Plugins/Favourite/DiscoveryPage.xaml", UriKind.Relative));
                }

            }
            else if (entry.Name == "My Photo Stream")
            {
                if (PolicyKit.VioletPageSubscription == PolicyKit.MyStream)
                    DashboardNavigator.RequestVioletPage(this, null);
                else
                {
                    Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                    PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
                    currentPage.NavigationService.Navigate(new Uri("/Plugins/Profile/UserProfilePage.xaml?user_id=" + Cinderella.CinderellaCore.CurrentUser.ResourceId, UriKind.Relative));
                }

            }
            else if (entry.Name == "Violet")
            {
                DashboardNavigator.RequestVioletPage(this, null);
            }
            else if (entry.Name == "Summersalt")
            {
                DashboardNavigator.RequestSummersaltPage(this, null);
            }
            else if (entry.Name == "Contacts")
            {
                Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
                currentPage.NavigationService.Navigate(new Uri("/Plugins/Profile/ContactPage.xaml", UriKind.Relative));
            }
            else if (entry.Name == "Search")
            {
                Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
                currentPage.NavigationService.Navigate(new Uri("/Plugins/Search/SearchPage.xaml", UriKind.Relative));
            }
        }

        private void OnStreamListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StreamListView.SelectedItem == null)
                return;

            PhotoSet pset = (PhotoSet)StreamListView.SelectedItem;

            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
            currentPage.NavigationService.Navigate(new Uri("/Plugins/PhotoCollection/PhotoSetPage.xaml?photoset_id=" + pset.ResourceId, UriKind.Relative));

        }

        public void ResetListSelection()
        {
            FeatureListView.SelectedItem = null;
            StreamListView.SelectedItem = null;
            GroupListView.SelectedItem = null;
        }

        public void RefreshStreams()
        {
            GroupListStatusLabel.IsHitTestVisible = false;
            GroupListStatusLabel.Text = "Retrieving group list...";

            StreamListStatusLabel.IsHitTestVisible = false;
            StreamListStatusLabel.Text = "Retrieving photo sets...";

            Anaconda.AnacondaCore.GetGroupListAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId);
            Anaconda.AnacondaCore.GetPhotoSetListAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId);
        }

        private void GroupListStatusLabel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GroupListStatusLabel.IsHitTestVisible = false;
            GroupListStatusLabel.Text = "Retrieving group list...";

            Anaconda.AnacondaCore.GetGroupListAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId);
        }

        private void StreamListStatusLabel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StreamListStatusLabel.IsHitTestVisible = false;
            StreamListStatusLabel.Text = "Retrieving photo sets...";

            Anaconda.AnacondaCore.GetPhotoSetListAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId);

        }
    }
}
