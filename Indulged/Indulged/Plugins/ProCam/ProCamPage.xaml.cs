using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.Plugins.ProCam.HUD;
using System.Windows.Media.Imaging;
using Windows.Phone.Media.Capture;
using System.Windows.Media;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage : PhoneApplicationPage
    {
        private BitmapImage FlashIconAuto = new BitmapImage(new Uri("/Assets/ProCam/FlashAuto.png", UriKind.Relative));
        private BitmapImage FlashIconOn = new BitmapImage(new Uri("/Assets/ProCam/FlashOn.png", UriKind.Relative));
        private BitmapImage FlashIconOff = new BitmapImage(new Uri("/Assets/ProCam/FlashOff.png", UriKind.Relative));

        // Constructor
        public ProCamPage()
        {
            InitializeComponent();

            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Show loading view until camera is initialized
            ShowLoadingView();

            // Initialize camera
            DetectCameraSensorSupport();

            if (SupportedCameras.Contains(CameraSensorLocation.Back))
            {
                InitializeCameraAsync(CameraSensorLocation.Back);
            }
            else if (SupportedCameras.Contains(CameraSensorLocation.Front))
            {
                InitializeCameraAsync(CameraSensorLocation.Front);
            }

            // Events
            InitializeEventListeners();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            DestroyCam();
            Viewfinder.Background = new SolidColorBrush(Colors.Black);

            base.OnNavigatingFrom(e);
        }

    }
}