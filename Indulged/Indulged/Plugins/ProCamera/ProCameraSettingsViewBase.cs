using System.Windows.Controls;
using System.Windows.Media;
using Windows.Phone.Media.Capture;

namespace Indulged.Plugins.ProCamera
{
    public class ProCameraSettingsViewBase : UserControl
    {
        public static SolidColorBrush SelectedForeground = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xae, 0xef));
        public static SolidColorBrush UnselectedForeground = new SolidColorBrush(Color.FromArgb(0xff, 0x6b, 0xa1, 0xb5));

        // Reference to the camera
        private PhotoCaptureDevice _cam;
        public PhotoCaptureDevice cam
        {
            get
            {
                return _cam;
            }

            set
            {
                _cam = value;
                OnCameraChanged();
            }
        }

        protected virtual void OnCameraChanged()
        {
        }
    }
}
