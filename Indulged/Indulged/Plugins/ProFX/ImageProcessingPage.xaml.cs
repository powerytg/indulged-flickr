using Indulged.API.Anaconda;
using Indulged.API.Avarice.Controls;
using Indulged.API.Cinderella;
using Indulged.Plugins.Chrome;
using Indulged.Plugins.Chrome.Events;
using Indulged.Plugins.ProFX.Events;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Media;
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
        public static EventHandler RequestSettingsView;
        public static EventHandler RequestDismissSettingsView;

        public static EventHandler<AddFilterEventArgs> RequestAddFilter;
        public static EventHandler<RequestFilterViewEventArgs> RequestFilterView;
        public static EventHandler<DeleteFilterEventArgs> RequestDeleteFilter;
        public static EventHandler<DismissFilterEventArgs> RequestDismissFilterView;
        public static EventHandler RequestProcessorPage;
        public static EventHandler RequestDismissUploaderView;

        public static EventHandler RequestCropView;
        public static EventHandler RequestDismissCropView;
        public static EventHandler RequestResetCrop;
        public static EventHandler<CropAreaChangedEventArgs> CropAreaChanged;

        public static EventHandler RequestDismiss;

        // Filter gallery view
        private FilterGalleryView galleryView;

        // Settings view
        private FXSettingsView settingsView;

        // Constructor
        public ImageProcessingPage()
        {
            InitializeComponent();
            ApplyTheme();

            // Initialize gallery view
            galleryView = new FilterGalleryView();
            galleryView.VerticalAlignment = VerticalAlignment.Bottom;
            galleryView.Margin = new Thickness(0, 0, 0, BottomPanel.Height);
            galleryView.Visibility = Visibility.Collapsed;
            ProcessorPage.Children.Add(galleryView);

            // Initialize settings view
            settingsView = new FXSettingsView();
            settingsView.VerticalAlignment = VerticalAlignment.Bottom;
            settingsView.Margin = new Thickness(0, 0, 0, BottomPanel.Height);
            settingsView.Visibility = Visibility.Collapsed;
            ProcessorPage.Children.Add(settingsView);


            // Events
            ThemeManager.ThemeChanged += OnThemeChanged;

            RequestFilterListView += OnRequestFilterListView;
            RequestDismissFilterListView += OnRequestDismissFilterListView;

            RequestSettingsView += OnRequestSettingsView;
            RequestDismissSettingsView += OnRequestDismissSettingsView;

            RequestAddFilter += OnRequestAddFilter;
            RequestFilterView += OnRequestFilterView;
            RequestDeleteFilter += OnRequestDeleteFilter;
            RequestProcessorPage += OnRequestProcessorPage;
            RequestDismissFilterView += OnRequestDismissFilterView;
            
            RequestCropView += OnRequestCropView;
            RequestDismissCropView += OnRequestDismissCropView;
            RequestResetCrop += OnRequestResetCrop;

            RequestDismissUploaderView += OnRequestDismissUploaderView;
            RequestDismiss += OnRequestDismiss;
        }

        private void OnRequestDismissUploaderView(object sender, EventArgs e)
        {
            DismissUploaderView();
        }


        private void OnRequestDismiss(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
                NavigationService.RemoveBackEntry();
            }
            
        }

        private void ApplyTheme()
        {
            if (ThemeManager.CurrentTheme == Themes.Dark)
            {
                LayoutRoot.Background = new SolidColorBrush(Colors.Black);
                TitlePanel.Background = new SolidColorBrush(Color.FromArgb(216, 0, 0, 0));
            }
            else
            {
                LayoutRoot.Background = new SolidColorBrush(Colors.White);
                TitlePanel.Background = new SolidColorBrush(Color.FromArgb(216, 0xff, 0xff, 0xff));
            }
        }

        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            ApplyTheme();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (originalImage != null)
            {
                return;
            }

            originalImage = (BitmapImage)PhoneApplicationService.Current.State["ChosenPhoto"];
            PhoneApplicationService.Current.State.Remove("ChosenPhoto");
            originalImage.CreateOptions = BitmapCreateOptions.None;

            // Sampling
            if (originalImage != null && !double.IsNaN(originalImage.PixelWidth) && !double.IsNaN(originalImage.PixelHeight))
            {
                SampleOriginalImage();
                PhotoView.Source = currentPreviewBitmap;
            }
            else
            {
                PhotoView.SizeChanged += OnPhotoViewSizeChanged;
            }

        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (UploaderPage != null && UploaderPage.Visibility == Visibility.Visible)
            {
                e.Cancel = true;
                DismissUploaderView();
            }
            else if (settingsView != null && settingsView.Visibility == Visibility.Visible)
            {
                e.Cancel = true;
                RequestDismissSettingsView(this, null);
            }
            else if (galleryView != null && galleryView.Visibility == Visibility.Visible)
            {
                e.Cancel = true;
                RequestDismissFilterListView(this, null);
            }
            else if (BottomPanel.CurrentFilter != null)
            {
                e.Cancel = true;

                var evt = new DismissFilterEventArgs();
                evt.Filter = BottomPanel.CurrentFilter;
                RequestDismissFilterView(this, evt);
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            ThemeManager.ThemeChanged -= OnThemeChanged;

            RequestFilterListView -= OnRequestFilterListView;
            RequestDismissFilterListView -= OnRequestDismissFilterListView;

            RequestSettingsView -= OnRequestSettingsView;
            RequestDismissSettingsView -= OnRequestDismissSettingsView;

            RequestAddFilter -= OnRequestAddFilter;
            RequestFilterView -= OnRequestFilterView;
            RequestDeleteFilter -= OnRequestDeleteFilter;
            RequestProcessorPage -= OnRequestProcessorPage;
            RequestDismissFilterView -= OnRequestDismissFilterView;

            RequestCropView -= OnRequestCropView;
            RequestDismissCropView -= OnRequestDismissCropView;
            RequestResetCrop -= OnRequestResetCrop;

            RequestDismissUploaderView -= OnRequestDismissUploaderView;
            RequestDismiss -= OnRequestDismiss;

            // Clean up
            if (previewStream != null)
            {
                previewStream.Close();
                previewStream = null;
            }

            previewBuffer = null;

            AppliedFilters.Clear();

            originalBitmap = null;
            originalImage = null;
            originalPreviewBitmap = null;

            if (UploaderPage != null)
            {
                UploaderPage.RemoveEventListeners();
            }

            BottomPanel.RemoveEventListeners();

            if(galleryView != null)
                galleryView.RemoveEventListeners();

            if (settingsView != null)
                settingsView.RemoveEventListeners();

            base.OnRemovedFromJournal(e);
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

        private void OnRequestSettingsView(object sender, EventArgs e)
        {
            ShowSettingsView();
        }

        private void OnRequestDismissSettingsView(object sender, EventArgs e)
        {
            DismissSettingsView();
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
            ShowUploaderView();
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