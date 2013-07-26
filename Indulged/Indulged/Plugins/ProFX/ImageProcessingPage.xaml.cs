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
            
            // Creat an in-memory editing session
            WriteableBitmap wb = new WriteableBitmap(originalImage);
            session = new EditingSession(wb.AsBitmap());

            // Generate a blurred background view
            ApplyFilterToBackgroundImageAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (session != null)
            {
                session.Dispose();
            }

            base.OnNavigatedFrom(e);
        }

        private async void ApplyFilterToBackgroundImageAsync()
        {
            WriteableBitmap bmp = new WriteableBitmap(originalImage);

            MemoryStream bitmapStream = new MemoryStream();
            bmp.SaveJpeg(bitmapStream, bmp.PixelWidth, bmp.PixelHeight, 0, 50);
            IBuffer bmpBuffer = bitmapStream.GetWindowsRuntimeBuffer();

            // Output buffer
            WriteableBitmap outputImage = new WriteableBitmap(bmp.PixelWidth, bmp.PixelHeight);

            using (EditingSession editsession = new EditingSession(bmpBuffer))
            {
                // First add an antique effect 
                editsession.AddFilter(FilterFactory.CreateBlurFilter(BlurLevel.Blur6));

                // Finally, execute the filtering and render to a bitmap
                await editsession.RenderToBitmapAsync(outputImage.AsBitmap());
                outputImage.Invalidate();
                FadeInNewImage(outputImage);
            }
        }

        private void FadeInNewImage(WriteableBitmap newImage)
        {
            // Fade in the new image
            BackgroundImage.Source = newImage;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation fadeInAnimation = new DoubleAnimation();
            animation.Children.Add(fadeInAnimation);
            fadeInAnimation.Duration = animation.Duration;
            fadeInAnimation.To = 1;
            Storyboard.SetTarget(fadeInAnimation, BackgroundImage);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
            animation.Begin();

        }

        private void AddFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ShowSeconderyViewWithContent(new FilterGalleryView());
        }

        private void OnRequestFilterListView(object sender, EventArgs e)
        {
            ShowFilterListView();
        }

    }
}