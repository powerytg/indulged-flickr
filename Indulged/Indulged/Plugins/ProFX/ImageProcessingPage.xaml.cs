using Indulged.API.Anaconda;
using Indulged.Plugins.ProCamera;
using Indulged.Plugins.ProFX.Events;
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
        public static EventHandler RequestDismissFilterListView;
        public static EventHandler<AddFilterEventArgs> RequestAddFilter;
        public static EventHandler<RequestFilterViewEventArgs> RequestFilterView;
        public static EventHandler<DeleteFilterEventArgs> RequestDeleteFilter;
        public static EventHandler<DismissFilterEventArgs> RequestDismissFilterView;
        public static EventHandler RequestProcessorPage;

        public static EventHandler RequestCropView;
        public static EventHandler RequestDismissCropView;
        public static EventHandler RequestResetCrop;
        public static EventHandler<CropAreaChangedEventArgs> CropAreaChanged;

        // Filter gallery view
        private FilterGalleryView galleryView;

        // Constructor
        public ImageProcessingPage()
        {
            InitializeComponent();

            // Initialize gallery view
            galleryView = new FilterGalleryView();
            galleryView.VerticalAlignment = VerticalAlignment.Bottom;
            galleryView.Margin = new Thickness(0, 0, 0, BottomPanel.Height);
            galleryView.Visibility = Visibility.Collapsed;
            ProcessorPage.Children.Add(galleryView);

            // Events
            RequestFilterListView += OnRequestFilterListView;
            RequestDismissFilterListView += OnRequestDismissFilterListView;

            RequestAddFilter += OnRequestAddFilter;
            RequestFilterView += OnRequestFilterView;
            RequestDeleteFilter += OnRequestDeleteFilter;
            RequestProcessorPage += OnRequestProcessorPage;
            RequestDismissFilterView += OnRequestDismissFilterView;
            
            RequestCropView += OnRequestCropView;
            RequestDismissCropView += OnRequestDismissCropView;
            RequestResetCrop += OnRequestResetCrop;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //originalImage = ProCameraPage.CapturedImage;
            originalImage = (BitmapImage)PhotoView.Source;
            originalImage.CreateOptions = BitmapCreateOptions.None;

            // Sampling
            //if (originalImage != null && !double.IsNaN(originalImage.PixelWidth) && !double.IsNaN(originalImage.PixelHeight))
            //{
            //    SampleOriginalImage();
            //    PhotoView.Source = currentPreviewBitmap;
            //}
            //else
            //{
                PhotoView.SizeChanged += OnPhotoViewSizeChanged;
            //}

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void OnPhotoViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PhotoView.SizeChanged -= OnPhotoViewSizeChanged;
            SampleOriginalImage();
            PhotoView.Source = currentPreviewBitmap;
        }
        
        private void OnRequestFilterListView(object sender, EventArgs e)
        {
            ShowFilterListView();
        }

        private void OnRequestDismissFilterListView(object sender, EventArgs e)
        {
            DismissFilterListView();
        }

        private void OnRequestFilterView(object sender, RequestFilterViewEventArgs e)
        {
            ShowFilterControlView(e.Filter);
        }

        private void OnRequestProcessorPage(object sender, EventArgs e)
        {
            //ShowProcessorPage();
        }

        private void OnRequestDismissFilterView(object sender, DismissFilterEventArgs e)
        {
            DismissFilterControlView(e.Filter);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            //ShowUploaderPage();

            /*
            string sessionId = Guid.NewGuid().ToString().Replace("-", null);

            MemoryStream photoStream = new MemoryStream();
            WriteableBitmap originalBitmap = new WriteableBitmap(originalImage);
            originalBitmap.SaveJpeg(photoStream, originalBitmap.PixelWidth, originalBitmap.PixelHeight, 0, 85);
            photoStream.Seek(0, SeekOrigin.Begin);
            Anaconda.AnacondaCore.UploadPhoto(sessionId, "test.jpg", photoStream, null);
             * */
        }

        private void OnRequestCropView(object sender, EventArgs e)
        {
            PhotoView.ShowCropFinder();

            PhotoView.Source = originalPreviewBitmap;
        }

        private void OnRequestDismissCropView(object sender, EventArgs e)
        {
            PhotoView.DismissCropFinder();

            PhotoView.Source = currentPreviewBitmap;
        }

        private void OnRequestResetCrop(object sender, EventArgs e)
        {
            PhotoView.ResetCropArea();
        }

    }
}