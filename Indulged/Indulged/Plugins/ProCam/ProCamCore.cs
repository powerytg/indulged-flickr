using Microsoft.Devices;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.Foundation;
using Windows.Phone.Media.Capture;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage
    {
        public List<CameraSensorLocation> SupportedCameras { get; set; }

        public List<Int32> SupportedEVValues { get; set; }
        public List<uint> SupportedISOValues { get; set; }
        private List<uint> _supportedISOFixtures = new List<uint> { 100, 125, 160, 200, 250, 320, 400, 500, 640, 800, 1000, 1250, 1600, 2000, 2500, 3200, 4000, 5000, 6400, 12800, 25600 };

        public List<Windows.Foundation.Size> SupportedResolutions { get; set; }
        public Windows.Foundation.Size CurrentResolution { get; set; }

        public List<FlashState> SupportedFlashModes { get; set; }
        public FlashState CurrentFlashMode = FlashState.Auto;

        public List<object> SupportedWhiteBalances { get; set; }
        public List<CameraSceneMode> SupportedSceneModes { get; set; }

        public List<FocusIlluminationMode> SupportedFocusAssistModes { get; set; }


        // Camera
        private PhotoCaptureDevice cam;
        private CameraCaptureSequence seq;
        private MemoryStream capturedStream;

        private void DestroyCam()
        {
            if (cam != null)
            {
                cam.Dispose();
                cam = null;
                seq = null;
            }

            CameraButtons.ShutterKeyHalfPressed -= OnShutterHalfPress;
            CameraButtons.ShutterKeyPressed -= OnShutterFullPress;
            CameraButtons.ShutterKeyReleased -= OnShutterReleased;
        }

        private void DetectCameraSensorSupport()
        {
            SupportedCameras = new List<CameraSensorLocation>();

            if (PhotoCaptureDevice.AvailableSensorLocations.Contains(CameraSensorLocation.Back))
            {
                SupportedCameras.Add(CameraSensorLocation.Back);
            }
            
            if (PhotoCaptureDevice.AvailableSensorLocations.Contains(CameraSensorLocation.Front))
            {
                SupportedCameras.Add(CameraSensorLocation.Front);
            }

            if (SupportedCameras.Count > 1)
            {
                CameraSwitchButton.Visibility = Visibility.Visible;
            }
            else
            {
                CameraSwitchButton.Visibility = Visibility.Collapsed;
            }
            
        }

        private async void InitializeCameraAsync(CameraSensorLocation _sensor = CameraSensorLocation.Back)
        {
            SupportedResolutions = PhotoCaptureDevice.GetAvailableCaptureResolutions(_sensor).ToList();
            CurrentResolution = SupportedResolutions[0];
            cam = await PhotoCaptureDevice.OpenAsync(_sensor, CurrentResolution);

            // Enable shutter sound
            cam.SetProperty(KnownCameraGeneralProperties.PlayShutterSoundOnCapture, true);

            // Create capture sequence
            seq = cam.CreateCaptureSequence(1);

            // White balance
            IReadOnlyList<object> wbList = PhotoCaptureDevice.GetSupportedPropertyValues(_sensor, KnownCameraPhotoProperties.WhiteBalancePreset);
            SupportedWhiteBalances = new List<object>();
            SupportedWhiteBalances.Add(ProCamConstraints.PROCAM_AUTO_WHITE_BALANCE);

            foreach (object rawValue in wbList)
            {
                SupportedWhiteBalances.Add((WhiteBalancePreset)(uint)rawValue);
            }

            OSD.WhiteBalanceOSD.SupportedWhiteBalances = SupportedWhiteBalances;
            OSD.WhiteBalanceOSD.CurrentWhiteBalanceIndex = 0;

            // EV
            SupportedEVValues = new List<int>();
            CameraCapturePropertyRange evRange = PhotoCaptureDevice.GetSupportedPropertyRange(_sensor, KnownCameraPhotoProperties.ExposureCompensation);
            int minEV = (int)evRange.Min;
            int maxEV = (int)evRange.Max;
            for (int i = minEV; i <= maxEV; i++)
            {
                SupportedEVValues.Add(i);
            }

            EVDialer.SupportedValues = SupportedEVValues;

            // ISO
            SupportedISOValues = new List<uint>();
            SupportedISOValues.Add(ProCamConstraints.PROCAM_AUTO_ISO);
            CameraCapturePropertyRange isoRange = PhotoCaptureDevice.GetSupportedPropertyRange(_sensor, KnownCameraPhotoProperties.Iso);
            var minISO = (uint)isoRange.Min;
            var maxISO = (uint)isoRange.Max;
            foreach(var fixture in _supportedISOFixtures)
            {
                if(fixture >= minISO && fixture <= maxISO)
                {
                    SupportedISOValues.Add(fixture);
                }
            }

            ISODialer.SupportedValues = SupportedISOValues;

            // Flash
            IReadOnlyList<object> flashList = PhotoCaptureDevice.GetSupportedPropertyValues(_sensor, KnownCameraPhotoProperties.FlashMode);
            SupportedFlashModes = new List<FlashState>();
            foreach (object rawValue in flashList)
            {
                SupportedFlashModes.Add((FlashState)(uint)rawValue);
            }

            // Resolution
            OSD.MainOSD.SupportedResolutions = SupportedResolutions;
            OSD.MainOSD.CurrentResolution = CurrentResolution;

            // Scene modes
            SupportedSceneModes = new List<CameraSceneMode>();
            IReadOnlyList<object> sceneList = PhotoCaptureDevice.GetSupportedPropertyValues(_sensor, KnownCameraPhotoProperties.SceneMode);
            foreach (object rawValue in sceneList)
            {
                SupportedSceneModes.Add((CameraSceneMode)(uint)rawValue);
            }

            OSD.SceneOSD.SupportedSceneModes = SupportedSceneModes;
            OSD.SceneOSD.CurrentIndex = 0;

            // Focus assist
            IReadOnlyList<object> focusList = PhotoCaptureDevice.GetSupportedPropertyValues(_sensor, KnownCameraPhotoProperties.FocusIlluminationMode);
            SupportedFocusAssistModes = new List<FocusIlluminationMode>();
            foreach (object rawValue in focusList)
            {
                SupportedFocusAssistModes.Add((FocusIlluminationMode)(uint)rawValue);
            }

            OSD.FocusAssistOSD.SupportedModes = SupportedFocusAssistModes;
            OSD.FocusAssistOSD.CurrentIndex = 0;

            // Enable shutter sound
            cam.SetProperty(KnownCameraGeneralProperties.PlayShutterSoundOnCapture, true);

            // Create capture sequence
            seq = cam.CreateCaptureSequence(1);

            // Set video brush source
            ViewfinderBrush.SetSource(cam);
            CorrectViewfinderOrientation(Orientation);

            // Show UI chrome
            HideLoadingView();

            // Events
            CameraButtons.ShutterKeyHalfPressed += OnShutterHalfPress;
            CameraButtons.ShutterKeyPressed += OnShutterFullPress;
            CameraButtons.ShutterKeyReleased += OnShutterReleased;
        }

        // Ensure that the viewfinder is upright in LandscapeRight.
        private void CorrectViewfinderOrientation(PageOrientation orientation)
        {
            if (cam != null)
            {
                
                double ang;

                // LandscapeRight rotation when camera is on back of phone.
                int landscapeRightRotation = 180;
                int portraitRotation = 90;

                // Change LandscapeRight rotation for front-facing camera.
                if (cam.SensorLocation == CameraSensorLocation.Front)
                {
                    portraitRotation = -90;
                    landscapeRightRotation = -180;
                }
                
                // Rotate video brush from camera.
                if (orientation == PageOrientation.PortraitUp || orientation == PageOrientation.PortraitDown)
                {
                    ang = portraitRotation;
                }
                else if (orientation == PageOrientation.LandscapeRight)
                {
                    ang = landscapeRightRotation;
                }
                else
                {
                    ang = 0;
                }
                
                var tf = new CompositeTransform() { Rotation = ang };
                var previewSize = tf.TransformBounds(new System.Windows.Rect(new System.Windows.Point(), new System.Windows.Size(cam.PreviewResolution.Width, cam.PreviewResolution.Height)));
                double s1 = Viewfinder.ActualWidth / (double)previewSize.Width;
                double s2 = Viewfinder.ActualHeight / (double)previewSize.Height;

                double scale = Math.Min(s1, s2);
                if (cam.SensorLocation == CameraSensorLocation.Back)
                {
                    ViewfinderBrush.Transform = new CompositeTransform()
                    {
                        Rotation = ang,
                        CenterX = Viewfinder.ActualWidth / 2,
                        CenterY = Viewfinder.ActualHeight / 2,
                        ScaleX = scale,
                        ScaleY = scale
                    };
                }
                else
                {
                    double scaleY = scale;
                    double scaleX = scale;
                    if ((orientation & PageOrientation.Portrait) == PageOrientation.Portrait)
                    {
                        scaleY = scale * -1;
                    }
                    else
                    {
                        scaleX = scale * -1;
                    }

                    ViewfinderBrush.Transform = new CompositeTransform()
                    {
                        Rotation = ang,
                        CenterX = Viewfinder.ActualWidth / 2,
                        CenterY = Viewfinder.ActualHeight / 2,
                        ScaleX = scaleX,
                        ScaleY = scaleY
                    };
                }
            }
        }

#region Shutter and focus

        private int focusId = 0;

        private async void PerformFocus(int fid, Windows.Foundation.Point? point = null)
        {
            if (point != null)
                cam.FocusRegion = new Windows.Foundation.Rect(point.Value.X, point.Value.Y, 0, 0);
            else
                cam.FocusRegion = null;

            await this.cam.FocusAsync();

            Dispatcher.BeginInvoke(() =>
            {
                OnFocusLocked(fid);
            });
        }

        private void BeginAutoFocus(Windows.Foundation.Point? point = null)
        {
            if (point != null)
            {
                PerformFocusAnimation(point);
            }

            focusId++;
            PerformFocus(focusId, point);
        }

        private void OnFocusLocked(int fid)
        {
            if (fid != focusId)
                return;

            if (focusBinkAnimation != null)
            {
                focusBinkAnimation.Stop();
                focusBinkAnimation = null;
            }

            PerformFocusLockedAnimation();
        }

#endregion

        private async void CapturePhoto()
        {
            PerformCaptureAnimation();

            await cam.PrepareCaptureSequenceAsync(seq);
            capturedStream = new MemoryStream();
            seq.Frames[0].CaptureStream = capturedStream.AsOutputStream();

            // Capture
            await seq.StartCaptureAsync();
            capturedStream.Seek(0, SeekOrigin.Begin);

            // Post processing
            Dispatcher.BeginInvoke(() =>
            {
                ProcessCapturedPhoto();
            });

        }

        private void ProcessCapturedPhoto()
        {
            string ramdomFileName = Guid.NewGuid().ToString().Replace("-", null);
            string fileName = ramdomFileName + ".jpg";

            int angle = 0;
            MemoryStream ms = null;
            if (Orientation == PageOrientation.PortraitUp || Orientation == PageOrientation.PortraitDown)
            {
                if (cam.SensorLocation == CameraSensorLocation.Back)
                {
                    angle = 90;
                }
                else
                {
                    angle = 270;
                }
            }

            if (angle == 0)
                ms = capturedStream;
            else
                ms = (MemoryStream)RotateStream(capturedStream, angle);

            BitmapImage capturedImage = new BitmapImage();
            capturedImage.SetSource(ms);
            ms.Close();
            PhoneApplicationService.Current.State["ChosenPhoto"] = capturedImage;

            Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/Plugins/ProFX/ImageProcessingPage.xaml", UriKind.Relative));
                NavigationService.RemoveBackEntry();
            });
        }

        private Stream RotateStream(Stream stream, int angle)
        {
            stream.Position = 0;
            if (angle % 90 != 0 || angle < 0) throw new ArgumentException();
            if (angle % 360 == 0) return stream;

            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            WriteableBitmap wbSource = new WriteableBitmap(bitmap);

            WriteableBitmap wbTarget = null;
            if (angle % 180 == 0)
            {
                wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
            }
            else
            {
                wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
            }

            for (int x = 0; x < wbSource.PixelWidth; x++)
            {
                for (int y = 0; y < wbSource.PixelHeight; y++)
                {
                    switch (angle % 360)
                    {
                        case 90:
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 180:
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 270:
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                    }
                }
            }
            MemoryStream targetStream = new MemoryStream();
            wbTarget.SaveJpeg(targetStream, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 100);
            return targetStream;
        }
    }
}
