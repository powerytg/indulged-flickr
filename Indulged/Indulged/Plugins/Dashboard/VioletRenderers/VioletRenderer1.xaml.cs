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
    public partial class VioletRenderer1 : VioletRendererBase
    {
        // Constructor
        public VioletRenderer1() : base()
        {
            InitializeComponent();
        }

        protected override void OnPhotoGroupChanged()
        {
            base.OnPhotoGroupChanged();
            Photo firstPhoto = PhotoGroup[0];
            ImageView.Source = new BitmapImage(new Uri(firstPhoto.GetImageUrl()));
        }
    }
}
