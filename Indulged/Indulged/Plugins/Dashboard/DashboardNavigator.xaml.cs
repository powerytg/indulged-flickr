using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Utils;
using Indulged.Plugins.Dashboard.Events;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Indulged.Plugins.Dashboard
{
    public partial class DashboardNavigator : UserControl
    {
        // Events
        public static EventHandler<DashboardPageEventArgs> DashboardPageChanged;

        public static EventHandler RequestPreludePage;
        public static EventHandler RequestVioletPage;
        public static EventHandler RequestSummersaltPage;


        // Constructor
        public DashboardNavigator()
        {
            InitializeComponent();

            // Events
            RequestVioletPage += OnRequestVioletPage;
            RequestSummersaltPage += OnRequestSummersaltPage;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var evt = new DashboardPageEventArgs();
            PivotItem selectedItem = MainPivot.SelectedItem as PivotItem;
            evt.SelectedPage = (IDashboardPage)selectedItem.Content;
            DashboardPageChanged.DispatchEvent(this, evt);
        }

        private void OnRequestVioletPage(object sender, EventArgs e)
        {
            MainPivot.SelectedIndex = 1;
        }

        private void OnRequestSummersaltPage(object sender, EventArgs e)
        {
            MainPivot.SelectedIndex = 2;
        }

        public void ResetListSelections()
        {
            PreludeView.ResetListSelection();
        }

        public void RefreshPreludeStreams()
        {
            PreludeView.RefreshStreams();
        }

        public void OnNavigatedFromPage()
        {
            LayoutRoot.Visibility = Visibility.Collapsed; 
            ResetListSelections();            
        }

        public void OnNavigatedToPage()
        {
            LayoutRoot.Visibility = Visibility.Visible;
        }

    }
}
