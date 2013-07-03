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
        ObservableCollection<string> FeatureStreams;

        // Photo sets
        ObservableCollection<PhotoSet> PhotoSetList;

        // Constructor
        public PreludePage()
        {
            InitializeComponent();

            PhotoSetList = new ObservableCollection<PhotoSet>();
            StreamListView.ItemsSource = PhotoSetList;

            FeatureStreams = new ObservableCollection<string>();
            FeatureStreams.Add("Prelude");
            FeatureStreams.Add("Violet");
            FeatureStreams.Add("Summersalt");
            FeatureListView.ItemsSource = FeatureStreams;

            // Events
            Cinderella.CinderellaCore.PhotoSetListUpdated += OnPhotoSetListUpdated;
        }

        // Stream updated
        private void OnPhotoSetListUpdated(object sender, EventArgs args)
        {
            Dispatcher.BeginInvoke(() =>
            {
                PhotoSetList.Clear();
                foreach (PhotoSet photoset in Cinderella.CinderellaCore.PhotoSetList)
                {
                    PhotoSetList.Add(photoset);
                }
            });
        }
    }
}