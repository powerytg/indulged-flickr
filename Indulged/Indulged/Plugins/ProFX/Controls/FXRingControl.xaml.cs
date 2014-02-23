using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.ProFX.Controls
{
    public partial class FXRingControl : UserControl
    {
        // Constructor
        public FXRingControl()
        {
            InitializeComponent();

            Ring1Transform.CenterX = Width / 2;
            Ring1Transform.CenterY = Height / 2;

            Ring2Transform.CenterX = Width / 2;
            Ring2Transform.CenterY = Height / 2;

            Ring3Transform.CenterX = Width / 2;
            Ring3Transform.CenterY = Height / 2;

            Ring4Transform.CenterX = Width / 2;
            Ring4Transform.CenterY = Height / 2;

            RingAnimation.Begin();
        }

        public void Pause()
        {
            RingAnimation.Pause();
        }

        public void Stop()
        {
            RingAnimation.Stop();
        }

        public void Resume()
        {
            RingAnimation.Resume();
        }
    }
}
