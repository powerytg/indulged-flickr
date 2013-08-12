using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.Plugins.ProFX.Filters;
using Indulged.Plugins.ProFX.Events;

namespace Indulged.Plugins.ProFX
{
    public partial class FXGalleryStatusView : UserControl
    {

        // Constructor
        public FXGalleryStatusView()
        {
            InitializeComponent();
        }

        private void TitleButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back
            ImageProcessingPage.RequestDismissFilterListView(this, null);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back
            ImageProcessingPage.RequestDismissFilterListView(this, null);
        }

    }
}
