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
    public partial class VioletHeadlineRenderer : VioletRendererBase
    {
        // Constructor
        public VioletHeadlineRenderer()
            : base()
        {
            InitializeComponent();
        }

        protected override void OnPhotoGroupSourceChanged()
        {
            base.OnPhotoGroupSourceChanged();
            ImageView.PhotoSource = PhotoGroupSource.Photos[0];
        }
    }
}
