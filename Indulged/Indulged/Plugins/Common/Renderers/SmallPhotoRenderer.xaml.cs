using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.Common.Renderers
{
    public partial class SmallPhotoRenderer : PhotoRendererBase
    {
        public SmallPhotoRenderer()
        {
            InitializeComponent();
        }

        protected override Image GetImagePresenter()
        {
            return ImageView;
        }

        protected override void OnPhotoSourceChanged()
        {
            base.OnPhotoSourceChanged();
            ImageView.Source = new BitmapImage { UriSource = new Uri(PhotoSource.GetImageUrl()), DecodePixelWidth = 300 };

            if (PhotoSource.ViewCount == 0)
            {
                StatView.Visibility = Visibility.Collapsed;
            }
            else
            {
                StatView.Visibility = Visibility.Visible;
                StatView.PhotoSource = PhotoSource;
            }

        }
    }
}
