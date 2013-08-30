﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Events;

namespace Indulged.Plugins.Dashboard
{
    public partial class PreludePage : UserControl, IDashboardPage
    {
        public string BackgroundImageUrl
        {
            get
            {
                return "/Assets/Chrome/PreludeBackground.png";
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
            FeatureStreams.Add(new PreludeItemModel { Name = "Favourites" });
            FeatureStreams.Add(new PreludeItemModel { Name = "Violet" });
            FeatureStreams.Add(new PreludeItemModel { Name = "Summersalt" });
            FeatureListView.ItemsSource = FeatureStreams;

            // Events
            Cinderella.CinderellaCore.PhotoSetListUpdated += OnPhotoSetListUpdated;
            Cinderella.CinderellaCore.GroupListUpdated += OnGroupListUpdated;
            Cinderella.CinderellaCore.JoinGroupComplete += OnGroupJoined;
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
                
                PhotoSetList.Clear();
                foreach (PhotoSet photoset in Cinderella.CinderellaCore.PhotoSetList)
                {
                    PhotoSetList.Add(photoset);
                }
            });
        }

        // Group list updated
        private void OnGroupListUpdated(object sender, GroupListUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;
                
                GroupList.Clear();
                foreach (FlickrGroup group in e.Groups)
                {
                    GroupList.Add(group);
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
            PreludeItemModel entry = (PreludeItemModel)FeatureListView.SelectedItem;
            if (entry.Name == "Prelude")
            {
                // Ignore
            }
            else if (entry.Name == "Violet")
            {
                DashboardNavigator.RequestVioletPage(this, null);
            }
            else if (entry.Name == "Summersalt")
            {
                DashboardNavigator.RequestSummersaltPage(this, null);
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
    }
}
