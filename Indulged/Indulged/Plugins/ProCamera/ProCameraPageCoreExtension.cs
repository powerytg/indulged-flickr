using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Phone.Media.Capture;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Animation;   

namespace Indulged.Plugins.ProCamera
{
    public partial class ProCameraPage
    {
        // Camera
        private PhotoCaptureDevice cam;
        private CameraCaptureSequence seq;
        private MemoryStream capturedStream;

        private void DestroyCam()
        {
            if (cam != null)
            {
                cam.Dispose();
                seq = null;
            }
        }

        private async void CreateCam(CameraSensorLocation camType)
        {
            IReadOnlyList<Windows.Foundation.Size> SupportedResolutions = PhotoCaptureDevice.GetAvailableCaptureResolutions(camType);
            Windows.Foundation.Size res = SupportedResolutions[0];
            cam = await PhotoCaptureDevice.OpenAsync(camType, res);

            // Enable shutter sound
            cam.SetProperty(KnownCameraGeneralProperties.PlayShutterSoundOnCapture, true);

            // Create capture sequence
            seq = cam.CreateCaptureSequence(1);

            // Set video brush source
            ViewfinderBrush.SetSource(cam);
            CorrectViewfinderOrientation(Orientation);

            // Update UI chrome
            ShowUIChromeAnimated();

            // Events
            CameraButtons.ShutterKeyHalfPressed += OnShutterHalfPress;
            CameraButtons.ShutterKeyPressed += OnShutterFullPress;
            CameraButtons.ShutterKeyReleased += OnShutterReleased;
        }

        private async void CapturePhoto()
        {
            await cam.PrepareCaptureSequenceAsync(seq);
            capturedStream = new MemoryStream();
            seq.Frames[0].CaptureStream = capturedStream.AsOutputStream();

            // Capture
            await seq.StartCaptureAsync();
            capturedStream.Seek(0, SeekOrigin.Begin);

            // Post processing
            Dispatcher.BeginInvoke(() => {
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

            ProCameraPage.CapturedImage = new BitmapImage();
            CapturedImage.SetSource(ms);

            ImageBrush staticBrush = new ImageBrush();
            staticBrush.ImageSource = ProCameraPage.CapturedImage;
            Viewfinder.Background = staticBrush;
            Viewfinder.Opacity = 1;

            NavigationService.Navigate(new Uri("/Plugins/ProFX/ImageProcessingPage.xaml", UriKind.Relative));
 
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
            FocusAnimation.Begin();
            focusId++;
            PerformFocus(focusId);
        }

        private void OnFocusLocked(int fid)
        {
            if (fid != focusId)
                return;

            FocusAnimation.Stop();
            FocusLockedAnimation.Begin();
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
        }

        #endregion
    }
}
