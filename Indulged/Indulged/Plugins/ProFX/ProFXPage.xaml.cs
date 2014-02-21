using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.ProFX
{
    public partial class ProFXPage : PhoneApplicationPage
    {
        // Constructor
        public ProFXPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            originalImage = new BitmapImage(new Uri("/Assets/ProCam/TestImage.jpg", UriKind.Relative));
            originalImage.CreateOptions = BitmapCreateOptions.None;

            // Prepare for sampling
            ViewFinder.SizeChanged += OnViewFinderSizeChanged;

            // Filters
            filterManager = new Filters.FXFilterManager();
            FilterGalleryView.FilterManager = filterManager;
            FilterGalleryView.InitializeFilterDroplets();

            ActiveFilterView.FilterManager = filterManager;

            // Events
            InitializeEventListeneres();
        }

        private void OnViewFinderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewFinder.SizeChanged -= OnViewFinderSizeChanged;

            SampleOriginalImage();
            ViewFinder.Source = currentPreviewBitmap;
            ViewFinder.OriginalBitmap = originalImage;
        }

    }
}