using Microsoft.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Phone.Media.Capture;

namespace Indulged.Plugins.ProCamera
{
    public partial class ProCameraPage
    {
        private ProCameraFlashSettingsView flashSettingsView = new ProCameraFlashSettingsView();
        private ProCameraISOSettingsView isoSettingsView = new ProCameraISOSettingsView();
        private ProCameraWhiteBalanceSettingsView whiteBalanceView = new ProCameraWhiteBalanceSettingsView();

        // Flash
        private void OnFlashButtonClick(object sender, RoutedEventArgs e)
        {
            flashSettingsView.cam = cam;
            ShowOrHideExtendedPanelWithView(flashSettingsView);
        }

        private void UpdateFlashModeButton()
        {
            FlashState flashMode = (FlashState)(uint)cam.GetProperty(KnownCameraPhotoProperties.FlashMode);
            if (flashMode == FlashState.On)
            {
                FlashModeButton.DisplayValue = "on";
            }
            else if (flashMode == FlashState.Off)
            {
                FlashModeButton.DisplayValue = "off";
            }
            else if (flashMode == FlashState.Auto)
            {
                FlashModeButton.DisplayValue = "auto";
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

        private void OnISOChanged(object sender, EventArgs e)
        {
            UpdateISOButton();
            HideExtendedPanel();
        }

        private void UpdateISOButton()
        {
            object rawISO = cam.GetProperty(KnownCameraPhotoProperties.Iso);
            if (rawISO == null)
            {
                ISOModeButton.DisplayValue = "auto";
            }
            else
            {
                UInt32 currentISO = (UInt32)rawISO;
                ISOModeButton.DisplayValue = currentISO.ToString();
            }
        }

        // White balance
        private void OnWBButtonClick(object sender, RoutedEventArgs e)
        {
            whiteBalanceView.cam = cam;
            ShowOrHideExtendedPanelWithView(whiteBalanceView);
        }

        private void OnWhiteBalanceChanged(object sender, EventArgs e)
        {
            UpdateWhiteBalanceButton();
            HideExtendedPanel();
        }

        private void UpdateWhiteBalanceButton()
        {
            object rawWB = cam.GetProperty(KnownCameraPhotoProperties.WhiteBalancePreset);
            if (rawWB == null)
            {
                WhiteBalanceButton.DisplayValue = "auto";
                WhiteBalanceButton.DisplayIcon = null;
            }
            else
            {
                WhiteBalancePreset wb = (WhiteBalancePreset)(uint)rawWB;
                switch (wb)
                {
                    case WhiteBalancePreset.Daylight:
                        WhiteBalanceButton.DisplayIcon = (BitmapImage)whiteBalanceView.DayLightIcon.Source;
                        break;
                    case WhiteBalancePreset.Cloudy:
                        WhiteBalanceButton.DisplayIcon = (BitmapImage)whiteBalanceView.CloudyIcon.Source;
                        break;
                    case WhiteBalancePreset.Tungsten:
                        WhiteBalanceButton.DisplayIcon = (BitmapImage)whiteBalanceView.TungstenIcon.Source;
                        break;
                    case WhiteBalancePreset.Fluorescent:
                        WhiteBalanceButton.DisplayIcon = (BitmapImage)whiteBalanceView.FluorescentIcon.Source;
                        break;
                    case WhiteBalancePreset.Flash:
                        WhiteBalanceButton.DisplayIcon = (BitmapImage)whiteBalanceView.FlashIcon.Source;
                        break;
                }

                WhiteBalanceButton.DisplayValue = null;
            }
        }

    }
}
