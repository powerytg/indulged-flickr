using System;
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

namespace Indulged.Plugins.Search
{
    public partial class SearchResultPage : PhoneApplicationPage
    {
        // Constructor
        public SearchResultPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string tags = null;
            string query = null;

            if (NavigationContext.QueryString.ContainsKey("tags"))
            {
                tags = NavigationContext.QueryString["tags"];
                PhotoResultView.Tags = tags;
            }


            if (NavigationContext.QueryString.ContainsKey("query"))
            {
                query = NavigationContext.QueryString["query"];
                PhotoResultView.Query = query;
            }

            if (query == null)
                TitleLabel.Text = tags + " ...";
            else
                TitleLabel.Text = query + "...";

            // Perform a search
            PhotoResultView.PerformSearch();
        }

    }
}