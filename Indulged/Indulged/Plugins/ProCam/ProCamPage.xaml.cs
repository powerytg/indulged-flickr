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

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage : PhoneApplicationPage
    {


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

            HUDSwitchButton.HUDStateChanged += OnOSDStateChanged;

            // Events
            InitializeEventListeners();
        }

    }
}