﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Avarice.Controls;

namespace Indulged.Plugins.Search
{
    public partial class SearchResultPage : PhoneApplicationPage
    {
        private bool executedOnce = false;

        // Constructor
        public SearchResultPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (executedOnce)
                return;

            executedOnce = true;

            string tags = null;
            string query = null;

            if (NavigationContext.QueryString.ContainsKey("tags"))
            {
                tags = NavigationContext.QueryString["tags"];
                PhotoResultView.Tags = tags;
                GroupResultView.Query = tags;
            }


            if (NavigationContext.QueryString.ContainsKey("query"))
            {
                query = NavigationContext.QueryString["query"];
                PhotoResultView.Query = query;
                GroupResultView.Query = query;
            }

            if (query == null)
                TitleLabel.Text = tags + " ...";
            else
                TitleLabel.Text = query + "...";

            // Perform a search
            PhotoResultView.PerformSearch();
            GroupResultView.PerformSearch();
        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            base.OnRemovedFromJournal(e);

            PhotoResultView.OnRemovedFromJournal();
            GroupResultView.OnRemovedFromJournal();
        }

    }
}