using Indulged.API.Cinderella.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.Common.PhotoRenderers
{
    public class PhotoRendererDecoder
    {
        public enum DecodeResolutions
        {
            High,
            Medium,
            Small,
            Tiny
        };

        public static BitmapImage GetDecodedBitmapImage(Photo photo, DecodeResolutions res)
        {
            if (res == DecodeResolutions.High)
            {
                return new BitmapImage { UriSource = new Uri(photo.GetImageUrl()), DecodePixelWidth = 640 };
            }
            else if (res == DecodeResolutions.Medium)
            {
                return new BitmapImage { UriSource = new Uri(photo.GetImageUrl()), DecodePixelWidth = 480 };
            }
            else
            {
                return new BitmapImage { UriSource = new Uri(photo.GetImageUrl()), DecodePixelWidth = 240 };
            }
        }
    }
}
