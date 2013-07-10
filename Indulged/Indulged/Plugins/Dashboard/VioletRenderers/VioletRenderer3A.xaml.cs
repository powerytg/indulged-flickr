using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

using Indulged.API.Cinderella.Models;

namespace Indulged.Plugins.Dashboard.VioletRenderers
{
    public partial class VioletRenderer3A : VioletRendererBase
    {
        // Constructor
        public VioletRenderer3A() : base()
        {
            InitializeComponent();
        }

        protected override void OnPhotoGroupSourceChanged()
        {
            base.OnPhotoGroupSourceChanged();
            ImageView1.PhotoSource = PhotoGroupSource.Photos[0];
            ImageView2.PhotoSource = PhotoGroupSource.Photos[1];
            ImageView3.PhotoSource = PhotoGroupSource.Photos[2];
        }
    }
}
