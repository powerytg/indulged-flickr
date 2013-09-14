using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;

namespace Indulged.Plugins.Search
{
    public partial class SearchPage : PhoneApplicationPage
    {
        // Constructor
        public SearchPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void OnSearchBox(object sender, RoutedEventArgs e)
        {
            SearchBox.Focus();
        }

        private void OnSearchBoxKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string trimmedQueryString = SearchBox.Text.Trim();
            if (e.Key == Key.Enter && trimmedQueryString.Length > 0)
            {
                NavigationService.Navigate(new Uri("/Plugins/Search/SearchResultPage.xaml?query=" + trimmedQueryString, UriKind.Relative));
                NavigationService.RemoveBackEntry();
            }
        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            TagListView.OnRemovedFromJournal();
            base.OnRemovedFromJournal(e);
        }

    }
}