using ExifLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.ProCamera
{
    public partial class ProCameraPage
    {
        int GetAngleFromExif(Stream imageStream)
        {
            var position = imageStream.Position;
            imageStream.Position = 0;
            ExifOrientation orientation = ExifReader.ReadJpeg(imageStream, String.Empty).Orientation;         
            imageStream.Position = position;
 
            switch (orientation) 
            {
                case ExifOrientation.TopRight:
                    return 90;
                case ExifOrientation.BottomRight:
                    return 180;
                case ExifOrientation.BottomLeft:
                    return 270;
                case ExifOrientation.TopLeft:
                case ExifOrientation.Undefined:
                default:
                    return 0;
            }
        }

        private Stream RotateStream(Stream stream, int angle)
        {
              stream.Position = 0;
              if (angle % 90 != 0 || angle < 0) throw new ArgumentException();
              if (angle % 360 == 0) return stream;
   
              BitmapImage bitmap = new BitmapImage();
              bitmap.SetSource(stream);
              WriteableBitmap wbSource = new WriteableBitmap(bitmap);
   
              WriteableBitmap wbTarget = null;
              if (angle % 180 == 0)
              {
                  wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
              }
              else
              {
                  wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
              }
   
              for (int x = 0; x < wbSource.PixelWidth; x++)
              {
                  for (int y = 0; y < wbSource.PixelHeight; y++)
                  {
                      switch (angle % 360)
                      {
                          case 90:
                              wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                              break;
                          case 180:
                              wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                               break;
                           case 270:
                               wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                               break;
                       }
                   }
               }
               MemoryStream targetStream = new MemoryStream();
               wbTarget.SaveJpeg(targetStream, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 100);
              return targetStream;
          }
     }
}
