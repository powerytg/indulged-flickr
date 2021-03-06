﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Models;
using Indulged.Plugins.Common.Renderers;
using Indulged.API.Anaconda;

namespace Indulged.Plugins.Search
{
    public partial class SearchTagListView : UserControl
    {
        // Constructor
        public SearchTagListView()
        {
            InitializeComponent();

            LoadingView.Visibility = Visibility.Visible;
            TagListView.Visibility = Visibility.Collapsed;

            // Events
            Cinderella.CinderellaCore.PopularTagsUpdated += OnPopularTagsReturned;
        }

        private void OnPopularTagsReturned(object sender, PopularTagListUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                TagListView.Children.Clear();
                foreach (PhotoTag tag in e.Tags)
                {
                    WeightedTagButton button = new WeightedTagButton();
                    button.TagSource = tag;
                    TagListView.Children.Add(button);
                }

                LoadingView.Visibility = Visibility.Collapsed;
                TagListView.Visibility = Visibility.Visible;
            });

        }

        public void OnRemovedFromJournal()
        {
            Cinderella.CinderellaCore.PopularTagsUpdated -= OnPopularTagsReturned;
        }

    }
}
