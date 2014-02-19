using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Indulged.Plugins.ProFX
{
    public partial class ProFXPage
    {
        // Original Photo
        private BitmapImage originalImage;
        private WriteableBitmap originalBitmap;

        // Sampled preview image
        private WriteableBitmap originalPreviewBitmap;

        // Preview image showed in the view finder
        private WriteableBitmap currentPreviewBitmap;

        // Buffer
        private MemoryStream previewStream;
        private IBuffer previewBuffer;

        // Sample the original image
        private void SampleOriginalImage()
        {
            originalBitmap = new WriteableBitmap(originalImage);
            double ratio = (double)originalBitmap.PixelWidth / (double)originalBitmap.PixelHeight;
            double w = Application.Current.RootVisual.RenderSize.Width;
            double h = Application.Current.RootVisual.RenderSize.Height;
            double previewWidth;
            double previewHeight;

            if (w / ratio > h)
            {
                previewHeight = h;
                previewWidth = h * ratio;
            }
            else
            {
                previewWidth = w;
                previewHeight = w / ratio;
            }

            originalPreviewBitmap = originalBitmap.Resize((int)previewWidth, (int)previewHeight, System.Windows.Media.Imaging.WriteableBitmapExtensions.Interpolation.Bilinear);
            currentPreviewBitmap = originalBitmap.Resize((int)previewWidth, (int)previewHeight, System.Windows.Media.Imaging.WriteableBitmapExtensions.Interpolation.Bilinear);

            // Create buffer
            previewStream = new MemoryStream();
            currentPreviewBitmap.SaveJpeg(previewStream, originalPreviewBitmap.PixelWidth, originalPreviewBitmap.PixelHeight, 0, 75);
            previewBuffer = previewStream.GetWindowsRuntimeBuffer();
        }

    }
}
