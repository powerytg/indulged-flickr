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

        public List<Int32> supportedEVValues = new List<Int32> { -3, -2, -1, 0, 1, 2, 3 };

        // Constructor
        public ProCamPage()
        {
            InitializeComponent();

            EVDialer.SupportedValues = supportedEVValues;
            EVHudView.SupportedValues = supportedEVValues;

            // Events
            EVDialer.DragBegin += OnEVDialDragBegin;
            EVDialer.DragEnd += OnEVDialDragEnd;
            EVDialer.ValueChanged += OnEVDialValueChanged;
        }

        private void OnEVDialDragBegin(object sender, EventArgs e)
        {
        }

        private void OnEVDialDragEnd(object sender, EventArgs e)
        {
        }

        private void OnEVDialValueChanged(object sender, EventArgs e)
        {
            EVHudView.SelectedValue = EVDialer.CurrentValue;
        }

    }
}