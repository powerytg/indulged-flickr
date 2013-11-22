using ExifLib;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Phone.Media.Capture;
using System.Collections.Generic;
using System.Linq;
using Indulged.PolKit;
using Microsoft.Phone.Tasks;
using Indulged.API.Avarice.Controls;
using Indulged.Resources;

namespace Indulged.Plugins.ProCamera
{
    public partial class ProCameraPage : PhoneApplicationPage
    {
        // Currently captured image
        public static BitmapImage CapturedImage { get; set; }

        // Constructor
        public ProCameraPage()
        {
            InitializeComponent();

            // Events
            ProCameraFlashSettingsView.FlashModeChanged += OnFlashModeChanged;
            ProCameraISOSettingsView.ISOChanged += OnISOChanged;
            ProCameraWhiteBalanceSettingsView.WhiteBalanceModeChanged += OnWhiteBalanceChanged;
        }
        
        // Media library
        private MediaLibrary library = new MediaLibrary();

        // Capture task
        private CameraCaptureTask camTask;

        private bool executedOnce;

        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            if (executedOnce)
            {
                Dispatcher.BeginInvoke(() => {
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                        NavigationService.RemoveBackEntry();
                    }
                });

                return;
            }

            executedOnce = true;

            if (PolicyKit.ShouldUseProCamera)
            {
                LayoutRoot.Visibility = Visibility.Visible;
                LoadingView.Text = AppResources.ProCamInitText;
                HideAllUIChrome();

                if (PhotoCaptureDevice.AvailableSensorLocations.Contains(CameraSensorLocation.Back))
                {
                    CreateCam(CameraSensorLocation.Back);
                }
                else if (PhotoCaptureDevice.AvailableSensorLocations.Contains(CameraSensorLocation.Front))
                {
                    CreateCam(CameraSensorLocation.Front);
                }

                // Can switch camera?
                if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary) && PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing))
                    FlipButton.Visibility = Visibility.Visible;
                else
                    FlipButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                LayoutRoot.Visibility = Visibility.Collapsed;
                camTask = new CameraCaptureTask();
                camTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
                camTask.Show();
            }

        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            ProCameraFlashSettingsView.FlashModeChanged -= OnFlashModeChanged;
            ProCameraISOSettingsView.ISOChanged -= OnISOChanged;
            ProCameraWhiteBalanceSettingsView.WhiteBalanceModeChanged -= OnWhiteBalanceChanged;

            camTask = null;

            base.OnRemovedFromJournal(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (ModalPopup.HasPopupHistory())
            {
                e.Cancel = true;
                ModalPopup.RemoveLastPopup();
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }


        private void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                ProCameraPage.CapturedImage = new BitmapImage();
                ProCameraPage.CapturedImage.SetSource(e.ChosenPhoto);

                NavigationService.Navigate(new Uri("/Plugins/ProFX/ImageProcessingPage.xaml", UriKind.Relative));
                NavigationService.RemoveBackEntry();
            }
            else
            {
                Dispatcher.BeginInvoke(() => {
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                        NavigationService.RemoveBackEntry();
                    }

                });
            }
        }

        private void HideAllUIChrome()
        {
            LayoutRoot.IsHitTestVisible = false;
            TopToolbar.Opacity = 0;
            BottomShadowImage.Opacity = 0;
            ShutterButton.Opacity = 0;
            Viewfinder.Opacity = 0;
        }

        private void ShowUIChromeAnimated()
        {
            CorrectViewfinderOrientation(Orientation);
            Viewfinder.Opacity = 1;

            if (Viewfinder.Background != ViewfinderBrush)
                Viewfinder.Background = ViewfinderBrush;

            UpdateFlashModeButton();

            LoadingView.Visibility = Visibility.Collapsed;
            PerformUIChromeAppearanceAnimation();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            DestroyCam();
            Viewfinder.Background = new SolidColorBrush(Colors.Black);
        }

        // Ensure that the viewfinder is upright in LandscapeRight.
        private void CorrectViewfinderOrientation(PageOrientation orientation)
        {
            if (cam != null)
            {
                // LandscapeRight rotation when camera is on back of phone.
                int landscapeRightRotation = 180;
                int portraitRotation = 90;

                // Change LandscapeRight rotation for front-facing camera.
                if (cam.SensorLocation == CameraSensorLocation.Front)
                {
                    portraitRotation = -90;
                    landscapeRightRotation = -180;
                }

                // Rotate video brush from camera.
                if (orientation == PageOrientation.PortraitUp || orientation == PageOrientation.PortraitDown)
                {
                    ViewfinderBrush.RelativeTransform = new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = portraitRotation };
                }
                else if (orientation == PageOrientation.LandscapeRight)
                {
                    // Rotate for LandscapeRight orientation.
                    ViewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = landscapeRightRotation };
                }
                else
                {
                    // Rotate for standard landscape orientation.
                    ViewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 0 };
                }
            }
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            CorrectViewfinderOrientation(e.Orientation);
            base.OnOrientationChanged(e);
        }

        #region Toolbar buttons

        private void OnFlipButtonClick(object sender, RoutedEventArgs e)
        {
            HideAllUIChrome();

            CameraSensorLocation previousType = cam.SensorLocation;
            DestroyCam();

            if(previousType == CameraSensorLocation.Back)
                CreateCam(CameraSensorLocation.Front);
            else
                CreateCam(CameraSensorLocation.Back);
        }

        private void OnShutterButtonClick(object sender, RoutedEventArgs e)
        {
            // Play shutter sound
            PerformCaptureAnimation();

            // Capture into memory stream
            CapturePhoto();
        }

        #endregion

        
        private void OnViewFinderTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (extendedPanel != null && !isAnimatingExtendedPanel)
            {
                HideExtendedPanel();
            }
            else
            {
                // Perform focus
                Point pt = e.GetPosition(Viewfinder);
                CompositeTransform ct = (CompositeTransform)AutoFocusBrackets.RenderTransform;
                ct.TranslateX = pt.X - LayoutRoot.ActualWidth / 2;
                ct.TranslateY = pt.Y - LayoutRoot.ActualHeight / 2;
                
                BeginAutoFocus(new Windows.Foundation.Point(pt.X, pt.Y));
            }
        }

        
    }
}