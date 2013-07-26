using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.ProFX.Filters
{
    public partial class FXBlurFilter : FilterBase
    {
        public FXBlurFilter()
        {
            DisplayName = "Gaussian Blur";
            InitializeComponent();
        }

        private void BackToEditorButton_Click(object sender, RoutedEventArgs e)
        {
            ImageProcessingPage.RequestFilterListView(this, null);
        }
    }
}
