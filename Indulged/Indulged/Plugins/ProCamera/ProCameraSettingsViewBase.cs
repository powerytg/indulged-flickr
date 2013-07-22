using Microsoft.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Indulged.Plugins.ProCamera
{
    public class ProCameraSettingsViewBase : UserControl
    {
        public static SolidColorBrush SelectedForeground = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xae, 0xef));
        public static SolidColorBrush UnselectedForeground = new SolidColorBrush(Color.FromArgb(0xff, 0x6b, 0xa1, 0xb5));

        // Reference to the camera
        private PhotoCamera _cam;
        public PhotoCamera cam
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
