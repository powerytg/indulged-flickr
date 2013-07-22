using Microsoft.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Indulged.Plugins.ProCamera
{
    public partial class ProCameraPage
    {
        private ProCameraFlashSettingsView flashSettingsView = new ProCameraFlashSettingsView();
        private ProCameraISOSettingsView isoSettingsView = new ProCameraISOSettingsView();

        // Flash
        private void OnFlashButtonClick(object sender, RoutedEventArgs e)
        {
            flashSettingsView.cam = cam;
            ShowOrHideExtendedPanelWithView(flashSettingsView);
        }

        private void UpdateFlashModeButton()
        {
            if (cam.FlashMode == FlashMode.On)
            {
                FlashModeButton.DisplayValue = "on";
            }
            else if (cam.FlashMode == FlashMode.Off)
            {
                FlashModeButton.DisplayValue = "off";
            }
            else if (cam.FlashMode == FlashMode.Auto)
            {
                FlashModeButton.DisplayValue = "auto";
            }
            else
            {
                FlashModeButton.DisplayValue = "red eye";
            }
        }

        private void OnFlashModeChanged(object sender, EventArgs e)
        {
            UpdateFlashModeButton();
            HideExtendedPanel();
        }

        // ISO
        private void OnISOButtonClick(object sender, RoutedEventArgs e)
        {
            
            isoSettingsView.cam = cam;
            ShowOrHideExtendedPanelWithView(isoSettingsView);
        }
    }
}
