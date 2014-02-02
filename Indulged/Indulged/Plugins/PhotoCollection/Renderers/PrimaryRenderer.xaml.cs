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
using Indulged.Plugins.Common.PhotoGroupRenderers;
using Indulged.API.Cinderella;

namespace Indulged.Plugins.PhotoCollection.Renderers
{
    public partial class PrimaryRenderer : CommonPhotoGroupRendererBase
    {
        // Constructor
        public PrimaryRenderer()
            : base()
        {
            InitializeComponent();
        }

        protected override void OnPhotoGroupSourceChanged()
        {
            base.OnPhotoGroupSourceChanged();

            PhotoSet photoset = Cinderella.CinderellaCore.PhotoSetCache[PhotoGroupSource.context];
            TitleView.Text = photoset.Name;            
            DateView.Text = "Created at " + photoset.CreationDate.ToString("MMM dd, yyyy");

            if (photoset.Description.Length > 0)
            {
                DescView.Text = photoset.Description;
            }
            else
            {
                DescView.Text = "No description available. You can tap on the edit description button to add content.";
            }

            ImageView.Source = new BitmapImage { UriSource = new Uri(photoset.Photos[0].GetImageUrl()), DecodePixelWidth = 640 };
        }
    }
}
