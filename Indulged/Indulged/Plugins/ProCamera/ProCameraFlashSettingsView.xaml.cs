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

        protected override void OnCameraChanged()
        {
            base.OnCameraChanged();

            OnButton.IsEnabled = cam.IsFlashModeSupported(FlashMode.On);
            OffButton.IsEnabled = cam.IsFlashModeSupported(FlashMode.Off);
            AutoButton.IsEnabled = cam.IsFlashModeSupported(FlashMode.Auto);
            RedEyeButton.IsEnabled = cam.IsFlashModeSupported(FlashMode.RedEyeReduction);

            OnButton.Foreground = (cam.FlashMode == FlashMode.On) ? SelectedForeground : UnselectedForeground;
            OffButton.Foreground = (cam.FlashMode == FlashMode.Off) ? SelectedForeground : UnselectedForeground;
            AutoButton.Foreground = (cam.FlashMode == FlashMode.Auto) ? SelectedForeground : UnselectedForeground;
            RedEyeButton.Foreground = (cam.FlashMode == FlashMode.RedEyeReduction) ? SelectedForeground : UnselectedForeground;
        }

        private void AutoButton_Click(object sender, RoutedEventArgs e)
        {
            cam.FlashMode = FlashMode.Auto;
            FlashModeChanged(this, null);
        }

        private void OnButton_Click(object sender, RoutedEventArgs e)
        {
            cam.FlashMode = FlashMode.On;
            FlashModeChanged(this, null);
        }

        private void OffButton_Click(object sender, RoutedEventArgs e)
        {
            cam.FlashMode = FlashMode.Off;
            FlashModeChanged(this, null);
        }

        private void RedEyeButton_Click(object sender, RoutedEventArgs e)
        {
            cam.FlashMode = FlashMode.RedEyeReduction;
            FlashModeChanged(this, null);
        }
    }
}
