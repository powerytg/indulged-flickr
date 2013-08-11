using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;
using System.IO;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Phone.Tasks;
using Windows.Storage.Streams;
using Microsoft.Xna.Framework.Media;
using Indulged.Plugins.ProFX.Filters;
using Indulged.Plugins.ProFX.Events;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows; 

namespace Indulged.Plugins.ProFX
{
    public partial class ImageProcessingPage
    {
        // Original Photo
        private BitmapImage originalImage;

        // Sampled preview image
        private WriteableBitmap originalPreviewBitmap;

        // Preview image showed in the view finder
        private WriteableBitmap currentPreviewBitmap;

        // Buffer
        private MemoryStream previewStream;
        private IBuffer previewBuffer;

        // Available filters
        public static List<FilterBase> AvailableFilters = new List<FilterBase> {
            new FXVignetteFilter(),
            new FXBlackWhiteFilter(),
            new FXSolarizeFilter(),
            new FXSharpenFilter(),
            new FXSepiaFilter(),
            new FXPosterizeFilter(),
            new FXOilFilter(),
            new FXPaintingFilter(),
            new FXNegativeFilter(),
            new FXMonoColorFilter(),
            new FXLomoFilter(),
            new FXClarityFilter(),
            new FXHueSaturationFilter(),
            new FXGrayscaleFilter(),
            new FXAutoEnhanceFilter(),
            new FXAntiqueFilter(),
            new FXBlurFilter(),
            new FXColorAdjustmentFilter(),
            new FXLevelFilter(),
            new FXCartoonFilter(),
            new FXColorBoostFilter(),
            new FXColorizationmentFilter(),
            new FXExposureFilter()
        };

        // Applied filters
        public static List<FilterBase> AppliedFilters = new List<FilterBase>();

        private void OnRequestAddFilter(object sender, AddFilterEventArgs e)
        {
            AppliedFilters.Add(e.Filter);

            // Show the filter control view
            ShowFilterControlView(e.Filter);
        }

        private void OnRequestDeleteFilter(object sender, DeleteFilterEventArgs e)
        {
            AppliedFilters.Remove(e.Filter);
            
        }



        public static FilterBase GetAppliedFilterByName(string displayName)
        {
            foreach (FilterBase filter in AppliedFilters)
            {
                if (filter.DisplayName == displayName)
                    return filter;
            }

            return null;
        }

        // Sample the original image
        private void SampleOriginalImage()
        {            
            WriteableBitmap bmp = new WriteableBitmap(originalImage);
            double ratio = (double)bmp.PixelWidth / (double)bmp.PixelHeight;
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

            originalPreviewBitmap = bmp.Resize((int)previewWidth, (int)previewHeight, System.Windows.Media.Imaging.WriteableBitmapExtensions.Interpolation.Bilinear);
            currentPreviewBitmap = bmp.Resize((int)previewWidth, (int)previewHeight, System.Windows.Media.Imaging.WriteableBitmapExtensions.Interpolation.Bilinear);

            // Create buffer
            previewStream = new MemoryStream();
            currentPreviewBitmap.SaveJpeg(previewStream, originalPreviewBitmap.PixelWidth, originalPreviewBitmap.PixelHeight, 0, 75);
            previewBuffer = previewStream.GetWindowsRuntimeBuffer();
        }

    }
}
