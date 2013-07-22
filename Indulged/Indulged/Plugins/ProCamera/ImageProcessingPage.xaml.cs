using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.ProCamera
{
    public partial class ImageProcessingPage : PhoneApplicationPage
    {
        // Constructor
        public ImageProcessingPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            PhotoView.Source = ProCameraPage.CapturedImage;
        }
    }
}