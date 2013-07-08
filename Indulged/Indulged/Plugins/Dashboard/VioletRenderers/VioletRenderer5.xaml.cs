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

        protected override void OnPhotoGroupChanged()
        {
            base.OnPhotoGroupChanged();
            ImageView1.PhotoSource = PhotoGroup[0];
            ImageView2.PhotoSource = PhotoGroup[1];
            ImageView3.PhotoSource = PhotoGroup[2];
            ImageView4.PhotoSource = PhotoGroup[3];
            ImageView5.PhotoSource = PhotoGroup[4];
        }
    }
}
