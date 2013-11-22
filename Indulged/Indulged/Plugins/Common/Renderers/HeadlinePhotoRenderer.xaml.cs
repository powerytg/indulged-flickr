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
    public partial class HeadlinePhotoRenderer : PhotoRendererBase
    {
        public HeadlinePhotoRenderer()
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
            ImageView.Source = new BitmapImage { UriSource = new Uri(PhotoSource.GetImageUrl()), DecodePixelWidth = 640 };

            string lowerCaseTitle = PhotoSource.Title.ToLower();
            if (lowerCaseTitle.Contains(".jpg") || lowerCaseTitle.Contains(".jpeg") || lowerCaseTitle.Contains(".png"))
                TitleLabel.Text = "Cover Photo";
            else
                TitleLabel.Text = "Cover Photo: " + PhotoSource.Title;

            DescriptionLabel.Text = PhotoSource.Description;
        }
    }
}
