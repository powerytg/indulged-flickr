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

namespace Indulged.Plugins.Common.PhotoGroupRenderers
{
    public partial class CommonPhotoGroupRenderer2 : CommonPhotoGroupRendererBase
    {
        // Constructor
        public CommonPhotoGroupRenderer2()
            : base()
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
        }
    }
}
