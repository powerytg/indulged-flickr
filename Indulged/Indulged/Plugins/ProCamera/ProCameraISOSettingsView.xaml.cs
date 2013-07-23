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

namespace Indulged.Plugins.ProCamera
{
    public partial class ProCameraISOSettingsView : ProCameraSettingsViewBase
    {
        // Events
        public static EventHandler ISOChanged;

        // Constructor
        public ProCameraISOSettingsView()
        {
            InitializeComponent();
        }

        // Common ISO values
        private static List<string> commonISOValues = new List<string> {"AUTO", "100", "200", "400", "800", "1600", "3200" };

        private List<string> GetAvailableISOValues(CameraSensorLocation cameraSensorLocation)
        {
            List<string> isoValues = new List<string>();
            CameraCapturePropertyRange isoRange = PhotoCaptureDevice.GetSupportedPropertyRange(cameraSensorLocation, KnownCameraPhotoProperties.Iso);
            UInt32 minISO = (UInt32)isoRange.Min;
            UInt32 maxISO = (UInt32)isoRange.Max;

            // Auto value
            isoValues.Add("AUTO");

            foreach (var isoString in commonISOValues)
            {
                if (isoString == "AUTO")
                    continue;

                UInt32 isoValue = UInt32.Parse(isoString);
                if (isoValue >= minISO && isoValue <= maxISO)
                {
                    isoValues.Add(isoString);
                }
            }

            return isoValues;
        }

        protected override void OnCameraChanged()
        {
            base.OnCameraChanged();

            // Construct ISO buttons
            List<string> supportedISOModes = GetAvailableISOValues(cam.SensorLocation);

            object rawISOValue = cam.GetProperty(KnownCameraPhotoProperties.Iso);
            UInt32 currentISO;
            string currentISOString = "";
            if (rawISOValue == null)
            {
                // Auto mode
            }
            else
            {
                currentISO = (UInt32)cam.GetProperty(KnownCameraPhotoProperties.Iso);
                currentISOString = currentISO.ToString();
            }
            
            ButtonPanel.Children.Clear();

            Style buttonStyle = Application.Current.Resources["CameraSubSettingsButtonStyle"] as Style;
            foreach (string isoString in supportedISOModes)
            {
                Button button = new Button();
                button.Content = isoString.ToString();
                button.Style = buttonStyle;
                button.Margin = new Thickness(20, 20, 20, 20);

                if (rawISOValue == null && isoString == "AUTO")
                {
                    button.Foreground = SelectedForeground;
                }
                else
                {
                    button.Foreground = (currentISOString == isoString) ? SelectedForeground : UnselectedForeground;
                }

                
                button.Click += OnISOButtonClick;

                ButtonPanel.Children.Add(button);
            }

        }

        private void OnISOButtonClick(object sender, RoutedEventArgs e)
        {
            Button isoButton = (Button)sender;
            if (isoButton.Content.ToString() == "AUTO")
                cam.SetProperty(KnownCameraPhotoProperties.Iso, null);
            else
            {
                UInt32 isoValue = UInt32.Parse(isoButton.Content.ToString());
                cam.SetProperty(KnownCameraPhotoProperties.Iso, isoValue);

            }

            ISOChanged(this, null);
        }

    }
}
