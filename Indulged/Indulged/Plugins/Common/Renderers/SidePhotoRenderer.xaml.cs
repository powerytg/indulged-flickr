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
    public partial class SidePhotoRenderer : PhotoRendererBase
    {
        public SidePhotoRenderer()
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
            ImageView.Source = new BitmapImage { UriSource = new Uri(PhotoSource.GetImageUrl()), DecodePixelWidth = 480 };

            if (PhotoSource.Description.Length > 0)
            {
                DescriptionView.PhotoSource = PhotoSource;
                DescriptionView.Visibility = Visibility.Visible;
            }
            else
            {
                DescriptionView.Visibility = Visibility.Collapsed;
            }

        }
    }
}
