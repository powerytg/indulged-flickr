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
using Indulged.Resources;
using Indulged.API.Avarice.Controls;
using Indulged.API.Avarice.Events;

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
                return "/Assets/Chrome/PreludeBackground.jpg";
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
            FeatureStreams.Add(new PreludeItemModel { Name = AppResources.PreludeVioletItemText });
            FeatureStreams.Add(new PreludeItemModel { Name = AppResources.PreludeSummersaltItemText });
            FeatureStreams.Add(new PreludeItemModel { Name = AppResources.PreludeMyPhotoStreamItemText });
            FeatureStreams.Add(new PreludeItemModel { Name = AppResources.PreludeDiscoveryItemText });
            FeatureStreams.Add(new PreludeItemModel { Name = AppResources.PreludeContactsItemText });
            FeatureStreams.Add(new PreludeItemModel { Name = AppResources.PreludeFavItemText, Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/Dashboard/Heart.png", UriKind.Relative)) });
            FeatureStreams.Add(new PreludeItemModel { Name = AppResources.PreludeSearchItemText });
            FeatureStreams.Add(new PreludeItemModel { Name = AppResources.PreludeUploadPhotoItemText });
            FeatureListView.ItemsSource = FeatureStreams;

            // Section titles
            FeaturedSectionView.Title = AppResources.PreludeFeaturedSectionText;
            StreamSectionView.Title = AppResources.PreludePhotoSetSectionText;
            GroupSectionView.Title = AppResources.PreludeCommunitySectionText;

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
                    StreamListStatusLabel.Text = AppResources.PreludeNoPhotoSetFoundText;
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
                StreamListStatusLabel.Text = AppResources.PreludeRetryText;
            });
        }

        private void OnGetGroupListException(object sender, GetGroupListExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;

                GroupListStatusLabel.IsHitTestVisible = true;
                GroupListStatusLabel.Text = AppResources.PreludeRetryText;
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
                    GroupListStatusLabel.Text = AppResources.PreludeNoGroupFoundText;
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
            if (entry.Name == AppResources.PreludeFavItemText)
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
            else if (entry.Name == AppResources.PreludeDiscoveryItemText)
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
            else if (entry.Name == AppResources.PreludeMyPhotoStreamItemText)
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
            else if (entry.Name == AppResources.PreludeVioletItemText)
            {
                DashboardNavigator.RequestVioletPage(this, null);
            }
            else if (entry.Name == AppResources.PreludeSummersaltItemText)
            {
                DashboardNavigator.RequestSummersaltPage(this, null);
            }
            else if (entry.Name == AppResources.PreludeContactsItemText)
            {
                Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
                currentPage.NavigationService.Navigate(new Uri("/Plugins/Profile/ContactPage.xaml", UriKind.Relative));
            }
            else if (entry.Name == AppResources.PreludeSearchItemText)
            {
                Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
                currentPage.NavigationService.Navigate(new Uri("/Plugins/Search/SearchPage.xaml", UriKind.Relative));
            }
            else if (entry.Name == AppResources.PreludeUploadPhotoItemText)
            {
                ShowUploadOptions();
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

        private void ShowUploadOptions()
        {
            var optionsView = new UploadOptionsView();
            var dialog = ModalPopup.Show(optionsView, "Upload From", new List<string> { AppResources.GenericConfirmText, AppResources.GenericCancelText });
            dialog.DismissWithButtonClick += (s, args) =>
            {
                ResetListSelection();

                int buttonIndex = (args as ModalPopupEventArgs).ButtonIndex;
                if (buttonIndex == 0)
                {
                    Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                    PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;

                    if (optionsView.CameraButton.IsChecked == true)
                    {
                        currentPage.NavigationService.Navigate(new Uri("/Plugins/ProCamera/ProCameraPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        currentPage.NavigationService.Navigate(new Uri("/Plugins/ProCamera/ProCameraPage.xaml?is_from_library=true", UriKind.Relative));
                    }
                }
            };

        }

        public void RefreshStreams()
        {
            GroupListStatusLabel.IsHitTestVisible = false;
            GroupListStatusLabel.Text = AppResources.PreludeRetrievingGroupsText;

            StreamListStatusLabel.IsHitTestVisible = false;
            StreamListStatusLabel.Text = AppResources.PreludeRetrievingPhotoSetsText;

            Anaconda.AnacondaCore.GetGroupListAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId);
            Anaconda.AnacondaCore.GetPhotoSetListAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId);
        }

        private void GroupListStatusLabel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GroupListStatusLabel.IsHitTestVisible = false;
            GroupListStatusLabel.Text = AppResources.PreludeRetrievingGroupsText;

            Anaconda.AnacondaCore.GetGroupListAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId);
        }

        private void StreamListStatusLabel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StreamListStatusLabel.IsHitTestVisible = false;
            StreamListStatusLabel.Text = AppResources.PreludeRetrievingPhotoSetsText;

            Anaconda.AnacondaCore.GetPhotoSetListAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId);

        }

        private void CameraButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
            currentPage.NavigationService.Navigate(new Uri("/Plugins/ProCamera/ProCameraPage.xaml", UriKind.Relative));
        }
    }
}
