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

namespace Indulged.Plugins.Dashboard
{
    public partial class DashboardNavigator : UserControl
    {
        // Events
        public static EventHandler<DashboardPageEventArgs> DashboardPageChanged;

        // Constructor
        public DashboardNavigator()
        {
            InitializeComponent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var evt = new DashboardPageEventArgs();
            PivotItem selectedItem = MainPivot.SelectedItem as PivotItem;
            evt.SelectedPage = (IDashboardPage)selectedItem.Content;
            DashboardPageChanged.DispatchEvent(this, evt);
        }
    }
}
