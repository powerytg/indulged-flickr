using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.Common.PhotoRenderers
{
    public partial class MediumPhotoRenderer : PhotoRendererBase
    {
        public MediumPhotoRenderer()
        {
            InitializeComponent();
        }

        protected override void OnPhotoSourceChanged()
        {
            base.OnPhotoSourceChanged();
            ImageView.Source = PhotoRendererDecoder.GetDecodedBitmapImage(PhotoSource, PhotoRendererDecoder.DecodeResolutions.Medium);
            if (PhotoSource.Title.Length > 0)
            {
                TitleLabel.Text = PhotoSource.Title;
                TitleLabel.Visibility = Visibility.Visible;
            }
            else
            {
                TitleLabel.Visibility = Visibility.Collapsed;
            }
                        
        }

    }
}
