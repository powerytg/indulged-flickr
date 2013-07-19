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
    public partial class VioletRenderer5 : VioletRendererBase
    {
        // Constructor
        public VioletRenderer5() : base()
        {
            InitializeComponent();
        }

        protected override void OnPhotoGroupSourceChanged()
        {
            base.OnPhotoGroupSourceChanged();
            ImageView1.PhotoSource = PhotoGroupSource.Photos[0];
            ImageView1.context = PhotoGroupSource.context;
            ImageView1.contextType = PhotoGroupSource.contextType;

            ImageView2.PhotoSource = PhotoGroupSource.Photos[1];
            ImageView2.context = PhotoGroupSource.context;
            ImageView2.contextType = PhotoGroupSource.contextType;

            ImageView3.PhotoSource = PhotoGroupSource.Photos[2];
            ImageView3.context = PhotoGroupSource.context;
            ImageView3.contextType = PhotoGroupSource.contextType;

            ImageView4.PhotoSource = PhotoGroupSource.Photos[3];
            ImageView4.context = PhotoGroupSource.context;
            ImageView4.contextType = PhotoGroupSource.contextType;

            ImageView5.PhotoSource = PhotoGroupSource.Photos[4];
            ImageView5.context = PhotoGroupSource.context;
            ImageView5.contextType = PhotoGroupSource.contextType;

        }
    }
}
