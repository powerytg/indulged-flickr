using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Windows.Phone.Media.Capture;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage
    {
        // Events
        public EventHandler<PhotoResult> Complete;

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

            OSD.MainOSD.ResolutionChanged += OnResolutionChanged;

            HUDSwitchButton.HUDStateChanged += OnOSDStateChanged;
            CameraSwitchButton.CameraChanged += OnCameraChanged;
        }

        private void RemoveAllEventListeners()
        {
            EVDialer.DragBegin -= OnEVDialDragBegin;
            EVDialer.DragEnd -= OnEVDialDragEnd;
            EVDialer.ValueChanged -= OnEVDialValueChanged;

            ISODialer.DragBegin -= OnISODialDragBegin;
            ISODialer.DragEnd -= OnISODialDragEnd;
            ISODialer.ValueChanged -= OnISODialValueChanged;

            OSD.WhiteBalanceOSD.WhiteBalanceChanged -= OnWhiteBalanceChanged;
            OSD.MainOSD.SceneButton.Click -= OnSceneButtonClick;
            OSD.SceneOSD.SceneModeChanged -= OnSceneModeChanged;

            OSD.MainOSD.FocusAssistButton.Click -= OnFocusAssistButtonClick;
            OSD.FocusAssistOSD.FocusAssistModeChanged -= OnFocusAssistModeChanged;

            OSD.MainOSD.ResolutionChanged -= OnResolutionChanged;

            HUDSwitchButton.HUDStateChanged -= OnOSDStateChanged;
            CameraSwitchButton.CameraChanged -= OnCameraChanged;
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

            if (cam != null)
            {
                if (ISODialer.CurrentValue == ProCamConstraints.PROCAM_AUTO_ISO)
                {
                    cam.SetProperty(KnownCameraPhotoProperties.Iso, null);
                }
                else
                {
                    cam.SetProperty(KnownCameraPhotoProperties.Iso, ISODialer.CurrentValue);
                }
            }
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

        private void OnResolutionChanged(object sender, EventArgs e)
        {
            CurrentResolution = OSD.MainOSD.CurrentResolution;
            DestroyCam();
            InitializeCameraAsync(CameraSwitchButton.CurrentCamera);
        }

        private void OnCameraChanged(object sender, EventArgs e)
        {
            DismissOSD();
            ShowLoadingView();

            DestroyCam();
            InitializeCameraAsync(CameraSwitchButton.CurrentCamera);
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
            if (SupportedFlashModes.Count < 2)
            {
                return;
            }

            int currentIndex = SupportedFlashModes.IndexOf(CurrentFlashMode);
            if (currentIndex == SupportedFlashModes.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }

            CurrentFlashMode = SupportedFlashModes[currentIndex];
            if (CurrentFlashMode == FlashState.Auto)
            {
                FlashButton.Style = (Style)App.Current.Resources["CapsuleButtonStyle"];
                FlashIcon.Source = FlashIconAuto;
                FlashLabel.Text = "AUTO";

                cam.SetProperty(KnownCameraPhotoProperties.FlashMode, FlashState.Auto);
            }
            else if (CurrentFlashMode == FlashState.On)
            {
                FlashButton.Style = (Style)App.Current.Resources["CapsuleButtonActiveStyle"];
                FlashIcon.Source = FlashIconOn;
                FlashLabel.Text = "ON";

                cam.SetProperty(KnownCameraPhotoProperties.FlashMode, FlashState.On);
            }
            else
            {
                FlashButton.Style = (Style)App.Current.Resources["CapsuleButtonStyle"];
                FlashIcon.Source = FlashIconOff;
                FlashLabel.Text = "OFF";

                cam.SetProperty(KnownCameraPhotoProperties.FlashMode, FlashState.Off);
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

            if (cam != null)
            {
                if (OSD.WhiteBalanceOSD.CurrentWhiteBalanceIndex == 0)
                {
                    cam.SetProperty(KnownCameraPhotoProperties.WhiteBalancePreset, null);
                }
                else
                {
                    WhiteBalancePreset wb = (WhiteBalancePreset)OSD.WhiteBalanceOSD.SupportedWhiteBalances[OSD.WhiteBalanceOSD.CurrentWhiteBalanceIndex];
                    cam.SetProperty(KnownCameraPhotoProperties.WhiteBalancePreset, wb);
                }
                
            }
        }

        private void OnSceneModeChanged(object sender, EventArgs e)
        {
            OSD.MainOSD.SceneButton.Content = OSD.SceneOSD.SceneStrings[OSD.SceneOSD.CurrentIndex];
            ShowOSD(OSD.MainOSD);

            CameraSceneMode sceneMode = OSD.SceneOSD.SupportedSceneModes[OSD.SceneOSD.CurrentIndex];
            cam.SetProperty(KnownCameraPhotoProperties.SceneMode, sceneMode);
        }

        private void OnFocusAssistModeChanged(object sender, EventArgs e)
        {
            OSD.MainOSD.FocusAssistButton.Content = OSD.FocusAssistOSD.ModeStrings[OSD.FocusAssistOSD.CurrentIndex];
            ShowOSD(OSD.MainOSD);

            FocusIlluminationMode mode = OSD.FocusAssistOSD.SupportedModes[OSD.FocusAssistOSD.CurrentIndex];
            cam.SetProperty(KnownCameraPhotoProperties.FocusIlluminationMode, mode);
        }

        private void OnViewFinderTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (HUDSwitchButton.IsOn)
            {
                DismissOSD();
            }
            else
            {
                Point pt = e.GetPosition(Viewfinder);
                BeginAutoFocus(new Windows.Foundation.Point(pt.X, pt.Y));
            }
        }

        private void OnShutterButtonClick(object sender, RoutedEventArgs e)
        {
            CapturePhoto();
        }

        private void OnShutterHalfPress(object sender, EventArgs e)
        {
            BeginAutoFocus();
        }

        private void OnShutterFullPress(object sender, EventArgs e)
        {
            CapturePhoto();
        }

        private void OnShutterReleased(object sender, EventArgs e)
        {
            // Ignore
        }


    }
}
