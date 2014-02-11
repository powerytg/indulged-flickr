using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.Plugins.ProCam.HUD;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage : PhoneApplicationPage
    {
        private BitmapImage FlashIconAuto = new BitmapImage(new Uri("/Assets/ProCam/FlashAuto.png", UriKind.Relative));
        private BitmapImage FlashIconOn = new BitmapImage(new Uri("/Assets/ProCam/FlashOn.png", UriKind.Relative));
        private BitmapImage FlashIconOff = new BitmapImage(new Uri("/Assets/ProCam/FlashOff.png", UriKind.Relative));

        // Constructor
        public ProCamPage()
        {
            InitializeComponent();

            EVDialer.SupportedValues = supportedEVValues;
            ISODialer.SupportedValues = supportedISOValues;

            OSD.MainOSD.SupportedResolutions = supportedResolutions;
            OSD.MainOSD.CurrentResolution = supportedResolutions[0];

            OSD.WhiteBalanceOSD.SupportedWhiteBalances = supportedWhiteBalances;
            OSD.WhiteBalanceOSD.CurrentWhiteBalanceIndex = 0;

            OSD.SceneOSD.SupportedSceneModes = supportedSceneModes;
            OSD.SceneOSD.CurrentIndex = 0;

            OSD.FocusAssistOSD.SupportedModes = supportedFocusAssistModes;
            OSD.FocusAssistOSD.CurrentIndex = 0;

            // Events
            InitializeEventListeners();
        }

       



    }
}