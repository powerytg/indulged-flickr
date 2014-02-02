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
    public partial class PhotoGroupRenderer1 : CommonPhotoGroupRendererBase
    {
        // Constructor
        public PhotoGroupRenderer1()
            : base()
        {
            InitializeComponent();
        }

        protected override void OnPhotoGroupSourceChanged()
        {
            base.OnPhotoGroupSourceChanged();
            Renderer.PhotoSource = PhotoGroupSource.Photos[0];
            Renderer.context = PhotoGroupSource.context;
            Renderer.contextType = PhotoGroupSource.contextType;

        }
    }
}
