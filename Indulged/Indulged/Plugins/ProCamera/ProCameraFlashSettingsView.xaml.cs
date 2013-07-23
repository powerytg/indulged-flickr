using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;
using Windows.Phone.Media.Capture;

namespace Indulged.Plugins.ProCamera
{
    public partial class ProCameraFlashSettingsView : ProCameraSettingsViewBase
    {
        // Events
        public static EventHandler FlashModeChanged;

        public ProCameraFlashSettingsView()
        {
            InitializeComponent();
        }

        private IReadOnlyList<FlashState> GetAvailableFlashStates(CameraSensorLocation cameraSensorLocation)
        {
            IReadOnlyList<object> rawValueList = PhotoCaptureDevice.GetSupportedPropertyValues(cameraSensorLocation, KnownCameraPhotoProperties.FlashMode);
            List<FlashState> flashStates = new List<FlashState>(rawValueList.Count);

            foreach (object rawValue in rawValueList)
            {
                flashStates.Add((FlashState)(uint)rawValue);
            }

            return flashStates.AsReadOnly();
        }

        protected override void OnCameraChanged()
        {
            base.OnCameraChanged();

            IReadOnlyList<FlashState> supportedFlashMode = GetAvailableFlashStates(cam.SensorLocation);

            OnButton.IsEnabled = supportedFlashMode.Contains(FlashState.On);
            OffButton.IsEnabled = supportedFlashMode.Contains(FlashState.Off);
            AutoButton.IsEnabled = supportedFlashMode.Contains(FlashState.Auto);

            FlashState currentFlashState = (FlashState)(uint)cam.GetProperty(KnownCameraPhotoProperties.FlashMode);

            OnButton.Foreground = (currentFlashState == FlashState.On) ? SelectedForeground : UnselectedForeground;
            OffButton.Foreground = (currentFlashState == FlashState.Off) ? SelectedForeground : UnselectedForeground;
            AutoButton.Foreground = (currentFlashState == FlashState.Auto) ? SelectedForeground : UnselectedForeground;
        }

        private void AutoButton_Click(object sender, RoutedEventArgs e)
        {
            cam.SetProperty(KnownCameraPhotoProperties.FlashMode, FlashState.Auto);
            FlashModeChanged(this, null);
        }

        private void OnButton_Click(object sender, RoutedEventArgs e)
        {
            cam.SetProperty(KnownCameraPhotoProperties.FlashMode, FlashState.On);
            FlashModeChanged(this, null);
        }

        private void OffButton_Click(object sender, RoutedEventArgs e)
        {
            cam.SetProperty(KnownCameraPhotoProperties.FlashMode, FlashState.Off);
            FlashModeChanged(this, null);
        }

    }
}
