using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Phone.Media.Capture;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage
    {
        private void InitializeEventListeners()
        {
            EVDialer.DragBegin += OnEVDialDragBegin;
            EVDialer.DragEnd += OnEVDialDragEnd;
            EVDialer.ValueChanged += OnEVDialValueChanged;

            ISODialer.DragBegin += OnISODialDragBegin;
            ISODialer.DragEnd += OnISODialDragEnd;
            ISODialer.ValueChanged += OnISODialValueChanged;

            OSD.WhiteBalanceOSD.WhiteBalanceChanged += OnWhiteBalanceChanged;
            OSD.MainOSD.SceneButton.Click += OnSceneButtonClick;
            OSD.SceneOSD.SceneModeChanged += OnSceneModeChanged;
            
            OSD.MainOSD.FocusAssistButton.Click += OnFocusAssistButtonClick;
            OSD.FocusAssistOSD.FocusAssistModeChanged += OnFocusAssistModeChanged;

            HUDSwitchButton.HUDStateChanged += OnOSDStateChanged;
        }

        private void OnEVDialDragBegin(object sender, EventArgs e)
        {
            ShowEVHUD();
        }

        private void OnEVDialDragEnd(object sender, EventArgs e)
        {
            DismissEVHUD();
        }

        private void OnEVDialValueChanged(object sender, EventArgs e)
        {
            if (evHUDView == null)
            {
                return;
            }

            evHUDView.SelectedValue = EVDialer.CurrentValue;
        }

        private void OnISODialDragBegin(object sender, EventArgs e)
        {
            ShowISOHUD();
        }

        private void OnISODialDragEnd(object sender, EventArgs e)
        {
            DismissISOHUD();
        }

        private void OnISODialValueChanged(object sender, EventArgs e)
        {
            if (isoHUDView == null)
            {
                return;
            }

            isoHUDView.SelectedValue = ISODialer.CurrentValue;
        }

        private void OnOSDStateChanged(object sender, EventArgs e)
        {
            if (HUDSwitchButton.IsOn)
            {
                ShowOSD();
            }
            else
            {
                DismissOSD();
            }
        }

        private void OnWhiteBalanceButtonClick(object sender, RoutedEventArgs e)
        {
            if (OSD.Visibility == Visibility.Collapsed)
            {
                ShowOSD(OSD.WhiteBalanceOSD);
            }
            else
            {
                if (OSD.CurrentOSD == OSD.WhiteBalanceOSD)
                {
                    DismissOSD();
                }
                else
                {
                    ShowOSD(OSD.WhiteBalanceOSD);
                }
            }
        }

        private void OnFlashButtonClick(object sender, RoutedEventArgs e)
        {
            if (supportedFlashModes.Count < 2)
            {
                return;
            }

            int currentIndex = supportedFlashModes.IndexOf(CurrentFlashMode);
            if (currentIndex == supportedFlashModes.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }

            CurrentFlashMode = supportedFlashModes[currentIndex];
            if (CurrentFlashMode == FlashState.Auto)
            {
                FlashButton.Style = (Style)App.Current.Resources["CapsuleButtonStyle"];
                FlashIcon.Source = FlashIconAuto;
                FlashLabel.Text = "AUTO";                
            }
            else if (CurrentFlashMode == FlashState.On)
            {
                FlashButton.Style = (Style)App.Current.Resources["CapsuleButtonActiveStyle"];
                FlashIcon.Source = FlashIconOn;
                FlashLabel.Text = "ON";
            }
            else
            {
                FlashButton.Style = (Style)App.Current.Resources["CapsuleButtonStyle"];
                FlashIcon.Source = FlashIconOff;
                FlashLabel.Text = "OFF";
            }

        }

        private void OnSceneButtonClick(object sender, RoutedEventArgs e)
        {
            ShowOSD(OSD.SceneOSD);
        }

        private void OnFocusAssistButtonClick(object sender, RoutedEventArgs e)
        {
            ShowOSD(OSD.FocusAssistOSD);
        }

        private void OnWhiteBalanceChanged(object sender, EventArgs e)
        {
            DismissOSD();

            WBLabel.Text = OSD.WhiteBalanceOSD.WhiteBalanceStrings[OSD.WhiteBalanceOSD.CurrentWhiteBalanceIndex];
        }

        private void OnSceneModeChanged(object sender, EventArgs e)
        {
            OSD.MainOSD.SceneButton.Content = OSD.SceneOSD.SceneStrings[OSD.SceneOSD.CurrentIndex];
            ShowOSD(OSD.MainOSD);
        }

        private void OnFocusAssistModeChanged(object sender, EventArgs e)
        {
            OSD.MainOSD.FocusAssistButton.Content = OSD.FocusAssistOSD.ModeStrings[OSD.FocusAssistOSD.CurrentIndex];
            ShowOSD(OSD.MainOSD);
        }

    }
}
