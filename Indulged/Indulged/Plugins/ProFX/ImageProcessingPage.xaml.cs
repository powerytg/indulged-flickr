﻿using Indulged.Plugins.ProFX.Events;
using Microsoft.Phone.Controls;
using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Storage.Streams; 


namespace Indulged.Plugins.ProFX
{
    public partial class ImageProcessingPage : PhoneApplicationPage
    {
        // Events
        public static EventHandler RequestFilterListView;
        public static EventHandler<AddFilterEventArgs> RequestAddFilter;

        // Constructor
        public ImageProcessingPage()
        {
            InitializeComponent();

            // Events
            RequestFilterListView += OnRequestFilterListView;
            RequestAddFilter += OnRequestAddFilter;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //originalImage = ProCameraPage.CapturedImage;
            originalImage = (BitmapImage)PhotoView.Source;
            originalImage.CreateOptions = BitmapCreateOptions.None;
            //PhotoView.Source = originalImage;
            
            // Sampling
            PhotoView.SizeChanged += OnPhotoViewSizeChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void OnPhotoViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PhotoView.SizeChanged -= OnPhotoViewSizeChanged;
            SampleOriginalImage();
            PhotoView.Source = originalPreviewBitmap;
        }
        
        private void AddFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ShowSeconderyViewWithContent(new FilterGalleryView());
        }

        private void BackToEditorButton_Click(object sender, RoutedEventArgs e)
        {
            ShowFilterListView();
        }

        private void OnRequestFilterListView(object sender, EventArgs e)
        {
            ShowFilterListView();
        }

    }
}