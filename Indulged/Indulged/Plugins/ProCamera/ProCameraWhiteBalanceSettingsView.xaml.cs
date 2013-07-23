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
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.ProCamera
{
    public partial class ProCameraWhiteBalanceSettingsView : ProCameraSettingsViewBase
    {
        // Events
        public static EventHandler WhiteBalanceModeChanged;

        // Icons
        public Image DayLightIcon { get; set; }
        public Image CloudyIcon { get; set; }
        public Image TungstenIcon { get; set; }
        public Image FluorescentIcon { get; set; }
        public Image FlashIcon { get; set; }

        private Image SelectedDayLightIcon;
        private Image SelectedCloudyIcon;
        private Image SelectedTungstenIcon;
        private Image SelectedFluorescentIcon;
        private Image SelectedFlashIcon;

        public ProCameraWhiteBalanceSettingsView()
        {
            InitializeComponent();

            DayLightIcon = new Image();
            DayLightIcon.Source = new BitmapImage(new Uri("/Assets/ProCamera/WB-Daylight.png", UriKind.Relative));

            SelectedDayLightIcon = new Image();
            SelectedDayLightIcon.Source = new BitmapImage(new Uri("/Assets/ProCamera/WB-Daylight-Selected.png", UriKind.Relative));

            CloudyIcon = new Image();
            CloudyIcon.Source = new BitmapImage(new Uri("/Assets/ProCamera/WB-Cloudy.png", UriKind.Relative));

            SelectedCloudyIcon = new Image();
            SelectedCloudyIcon.Source = new BitmapImage(new Uri("/Assets/ProCamera/WB-Cloudy-Selected.png", UriKind.Relative));

            TungstenIcon = new Image();
            TungstenIcon.Source = new BitmapImage(new Uri("/Assets/ProCamera/WB-Tungsten.png", UriKind.Relative));

            SelectedTungstenIcon = new Image();
            SelectedTungstenIcon.Source = new BitmapImage(new Uri("/Assets/ProCamera/WB-Tungsten-Selected.png", UriKind.Relative));

            FluorescentIcon = new Image();
            FluorescentIcon.Source = new BitmapImage(new Uri("/Assets/ProCamera/WB-Fluorescent.png", UriKind.Relative));

            SelectedFluorescentIcon = new Image();
            SelectedFluorescentIcon.Source = new BitmapImage(new Uri("/Assets/ProCamera/WB-Fluorescent-Selected.png", UriKind.Relative));

            FlashIcon = new Image();
            FlashIcon.Source = new BitmapImage(new Uri("/Assets/ProCamera/WB-Flash.png", UriKind.Relative));

            SelectedFlashIcon = new Image();
            SelectedFlashIcon.Source = new BitmapImage(new Uri("/Assets/ProCamera/WB-Flash-Selected.png", UriKind.Relative));

        }

        private IReadOnlyList<WhiteBalancePreset> GetAvailableWhiteBalancePresets(CameraSensorLocation cameraSensorLocation)
        {
            IReadOnlyList<object> rawValueList = PhotoCaptureDevice.GetSupportedPropertyValues(cameraSensorLocation, KnownCameraPhotoProperties.WhiteBalancePreset);
            List<WhiteBalancePreset> wbList = new List<WhiteBalancePreset>(rawValueList.Count);

            foreach (object rawValue in rawValueList)
            {
                wbList.Add((WhiteBalancePreset)(uint)rawValue);
            }

            return wbList.AsReadOnly();
        }

        protected override void OnCameraChanged()
        {
            base.OnCameraChanged();

            IReadOnlyList<WhiteBalancePreset> supportedWBPresets = GetAvailableWhiteBalancePresets(cam.SensorLocation);

            DayLightButton.IsEnabled = supportedWBPresets.Contains(WhiteBalancePreset.Daylight);
            CloudyButton.IsEnabled = supportedWBPresets.Contains(WhiteBalancePreset.Cloudy);
            TungstenButton.IsEnabled = supportedWBPresets.Contains(WhiteBalancePreset.Tungsten);
            FluorescentButton.IsEnabled = supportedWBPresets.Contains(WhiteBalancePreset.Fluorescent);
            FlashButton.IsEnabled = supportedWBPresets.Contains(WhiteBalancePreset.Flash);

            object rawWBMode = cam.GetProperty(KnownCameraPhotoProperties.WhiteBalancePreset);
            if (rawWBMode == null)
            {
                AutoButton.Foreground = SelectedForeground;
                DayLightButton.Content = DayLightIcon;
                CloudyButton.Content = CloudyIcon;
                TungstenButton.Content = TungstenIcon;
                FluorescentButton.Content = FluorescentIcon;
                FlashButton.Content = FlashIcon;

            }
            else
            {
                WhiteBalancePreset currentWB = (WhiteBalancePreset)(uint)rawWBMode;
                AutoButton.Foreground = UnselectedForeground;

                DayLightButton.Content = (currentWB == WhiteBalancePreset.Daylight) ? SelectedDayLightIcon : DayLightIcon;
                CloudyButton.Content = (currentWB == WhiteBalancePreset.Cloudy) ? SelectedCloudyIcon : CloudyIcon;
                TungstenButton.Content = (currentWB == WhiteBalancePreset.Tungsten) ? SelectedTungstenIcon : TungstenIcon;
                FluorescentButton.Content = (currentWB == WhiteBalancePreset.Fluorescent) ? SelectedFluorescentIcon : FluorescentIcon;
                FlashButton.Content = (currentWB == WhiteBalancePreset.Flash) ? SelectedFlashIcon : FlashIcon;
            }

        }

        private void AutoButton_Click(object sender, RoutedEventArgs e)
        {
            cam.SetProperty(KnownCameraPhotoProperties.WhiteBalancePreset, null);
            WhiteBalanceModeChanged(this, null);
        }

        private void DayLightButton_Click(object sender, RoutedEventArgs e)
        {
            cam.SetProperty(KnownCameraPhotoProperties.WhiteBalancePreset, WhiteBalancePreset.Daylight);
            WhiteBalanceModeChanged(this, null);
        }

        private void CloudyButton_Click(object sender, RoutedEventArgs e)
        {
            cam.SetProperty(KnownCameraPhotoProperties.WhiteBalancePreset, WhiteBalancePreset.Cloudy);
            WhiteBalanceModeChanged(this, null);
        }

        private void TungstenButton_Click(object sender, RoutedEventArgs e)
        {
            cam.SetProperty(KnownCameraPhotoProperties.WhiteBalancePreset, WhiteBalancePreset.Tungsten);
            WhiteBalanceModeChanged(this, null);
        }

        private void FluorescentButton_Click(object sender, RoutedEventArgs e)
        {
            cam.SetProperty(KnownCameraPhotoProperties.WhiteBalancePreset, WhiteBalancePreset.Fluorescent);
            WhiteBalanceModeChanged(this, null);
        }

        private void FlashButton_Click(object sender, RoutedEventArgs e)
        {
            cam.SetProperty(KnownCameraPhotoProperties.WhiteBalancePreset, WhiteBalancePreset.Flash);
            WhiteBalanceModeChanged(this, null);
        }

    }
}
