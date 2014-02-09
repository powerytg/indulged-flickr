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

            // Events
            InitializeEventListeners();
        }


    }
}