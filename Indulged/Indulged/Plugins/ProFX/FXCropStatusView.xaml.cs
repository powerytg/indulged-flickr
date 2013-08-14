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
    public partial class FXCropStatusView : UserControl
    {
        public FilterBase SelectedFilter { get; set;}

        // Constructor
        public FXCropStatusView()
        {
            InitializeComponent();
        }

        private void TitleButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back
            var evt = new DismissFilterEventArgs();
            evt.Filter = SelectedFilter;
            ImageProcessingPage.RequestDismissFilterView(this, evt);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back
            var evt = new DismissFilterEventArgs();
            evt.Filter = SelectedFilter;
            ImageProcessingPage.RequestDismissFilterView(this, evt);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back
            ImageProcessingPage.RequestResetCrop(this, null);
            SelectedFilter.DeleteFilterAsync();
        }

    }
}
