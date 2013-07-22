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

            ProCameraFlashSettingsView.FlashModeChanged += OnFlashModeChanged;
        }
        
        // Camera
        private PhotoCamera cam;

        // Media library
        private MediaLibrary library = new MediaLibrary();

        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            LoadingView.Text = "Initializing Camera ...";
            HideAllUIChrome();

            if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true)
            {
                CreateCam(CameraType.Primary);
            }
            else if(PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true)
            {
                CreateCam(CameraType.FrontFacing);                
            }

            ViewfinderBrush.SetSource(cam);

            // Can switch camera?
            if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary) && PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing))
                FlipButton.Visibility = Visibility.Visible;
            else
                FlipButton.Visibility = Visibility.Collapsed;
        }

        private void HideAllUIChrome()
        {
            TopToolbar.Opacity = 0;
            BottomShadowImage.Opacity = 0;
            ShutterButton.Opacity = 0;
            Viewfinder.Opacity = 0;
        }

        private void DestroyCam()
        {
            if (cam != null)
            {
                cam.Dispose();
                cam.CaptureImageAvailable -= OnCameraImageAvailable;
                cam.Initialized -= OnCameraInitialized;
            }
        }

        private void CreateCam(CameraType camType)
        {
            cam = new PhotoCamera(camType);
            cam.CaptureImageAvailable += OnCameraImageAvailable;
            cam.Initialized += OnCameraInitialized;
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            DestroyCam();
            Viewfinder.Background = new SolidColorBrush(Colors.Black);
        }

        private void OnCameraInitialized(object sender, CameraOperationCompletedEventArgs e)
        {
            // Show the UI chrome
            Dispatcher.BeginInvoke(() =>
                {
                    CorrectViewfinderOrientation(Orientation);
                    Viewfinder.Opacity = 1;

                    if(Viewfinder.Background != ViewfinderBrush)
                        Viewfinder.Background = ViewfinderBrush;

                    UpdateFlashModeButton();

                    LoadingView.Visibility = Visibility.Collapsed;
                    PerformUIChromeAppearanceAnimation();
                });
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
                if (cam.CameraType == CameraType.FrontFacing)
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

            CameraType previousType = cam.CameraType;
            DestroyCam();

            if(previousType == CameraType.Primary)
                CreateCam(CameraType.FrontFacing);
            else
                CreateCam(CameraType.Primary);

            ViewfinderBrush.SetSource(cam);
            CorrectViewfinderOrientation(Orientation);
        }

        private void OnShutterButtonClick(object sender, RoutedEventArgs e)
        {
            // Play shutter sound
            PerformCaptureAnimation();

            if (cam != null)
            {
                try
                {
                    // Start image capture.
                    cam.CaptureImage();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        #endregion

        private void OnCameraImageAvailable(object sender, ContentReadyEventArgs e)
        {
            string ramdomFileName = Guid.NewGuid().ToString().Replace("-", null);
            string fileName = ramdomFileName + ".jpg";

            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    Stream ms = null;
                    int angle = 0;
                    if (Orientation == PageOrientation.PortraitUp || Orientation == PageOrientation.PortraitDown)
                    {
                        if (cam.CameraType == CameraType.Primary)
                        {
                            angle = 90;
                        }
                        else
                        {
                            angle = 270;
                        }
                    }

                    if (angle == 0)
                        ms = e.ImageStream;
                    else
                        ms = RotateStream(e.ImageStream, angle);

                    ProCameraPage.CapturedImage = new BitmapImage();
                    CapturedImage.SetSource(ms);

                    ImageBrush staticBrush = new ImageBrush();
                    staticBrush.ImageSource = ProCameraPage.CapturedImage;
                    Viewfinder.Background = staticBrush;
                    Viewfinder.Opacity = 1;

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    CapturedImage = null;
                }
                finally
                {
                    // Close image stream
                    e.ImageStream.Close();

                    // Go to processing page
                    if (CapturedImage != null)
                    {
                        NavigationService.Navigate(new Uri("/Plugins/ProCamera/ImageProcessingPage.xaml", UriKind.Relative));
                    }

                }
            });
        }

        private void OnViewFinderTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (extendedPanel != null && !isAnimatingExtendedPanel)
            {
                HideExtendedPanel();
            }
        }

        
    }
}