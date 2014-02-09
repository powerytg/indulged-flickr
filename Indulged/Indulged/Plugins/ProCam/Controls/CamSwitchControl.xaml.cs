using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Media.Capture;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.ProCam.Controls
{
    public partial class CamSwitchControl : UserControl
    {
        // Events
        public EventHandler CameraChanged;

        private BitmapImage backCamIcon = new BitmapImage(new Uri("/Assets/ProCam/MainCamera.png", UriKind.Relative));
        private BitmapImage frontCamIcon = new BitmapImage(new Uri("/Assets/ProCam/FrontCamera.png", UriKind.Relative));

        private CameraSensorLocation _currentCamera = CameraSensorLocation.Back;
        public CameraSensorLocation CurrentCamera
        {
            get
            {
                return _currentCamera;
            }

            set
            {
                _currentCamera = value;

                if (_currentCamera == CameraSensorLocation.Back)
                {
                    Label.Text = "BACK CAMERA";
                    Icon.Source = backCamIcon;
                }
                else if (_currentCamera == CameraSensorLocation.Front)
                {
                    Label.Text = "FRONT CAMERA";
                    Icon.Source = frontCamIcon;
                }
            }
        }

        // Constructor
        public CamSwitchControl()
        {
            InitializeComponent();
        }

        private void SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCamera == CameraSensorLocation.Front)
            {
                CurrentCamera = CameraSensorLocation.Back;
            }
            else if (_currentCamera == CameraSensorLocation.Back)
            {
                CurrentCamera = CameraSensorLocation.Front;
            }

            if (CameraChanged != null)
            {
                CameraChanged(this, null);
            }
        }
    }
}
