using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Indulged.Plugins.Common.PhotoGroupRenderers
{
    public class CommonPhotoGroupRendererBase : UserControl
    {
        public static readonly DependencyProperty PhotoGroupSourceProperty = DependencyProperty.Register("PhotoGroupSource", typeof(PhotoGroup), typeof(CommonPhotoGroupRendererBase), new PropertyMetadata(OnPhotoGroupSourcePropertyChanged));

        public PhotoGroup PhotoGroupSource
        {
            get 
            { 
                return (PhotoGroup)GetValue(PhotoGroupSourceProperty); 
            }
            set 
            { 
                SetValue(PhotoGroupSourceProperty, value);
            }
        }

        public static void OnPhotoGroupSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((CommonPhotoGroupRendererBase)sender).OnPhotoGroupSourceChanged();
        }

        protected virtual void OnPhotoGroupSourceChanged()
        {
        }

        protected bool IsPortraitAspectRatio(Photo photo)
        {
            int w = (photo.Width == 0) ? photo.Width : photo.MediumWidth;
            int h = (photo.Height == 0) ? photo.Height : photo.MediumHeight;

            if (w == 0 || h == 0)
            {
                return false;
            }
            else
            {
                return (w < h);
            }
        }

        // Constructor
        public CommonPhotoGroupRendererBase()
            : base()
        {
        }
    }
}
