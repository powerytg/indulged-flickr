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
            new FXBlurFilter(),
            new FXColorAdjustFilter()
        };

        // Applied filters
        public static List<FilterBase> AppliedFilters = new List<FilterBase>();

        private void OnRequestAddFilter(object sender, AddFilterEventArgs e)
        {
            AppliedFilters.Add(e.Filter);

            // Show the filter control view
            SwitchSeconderyViewWithContent(e.Filter);
            e.Filter.OriginalImage = originalPreviewBitmap;
            e.Filter.Buffer = previewBuffer;
            e.Filter.OnFilterUIAdded();

            // Update the UI
            FilterButton filterButton = new FilterButton();
            filterButton.Filter = e.Filter;
            filterButton.HorizontalContentAlignment = HorizontalAlignment.Left;
            filterButton.FontWeight = FontWeights.Light;
            filterButton.FontSize = 36;
            filterButton.Content = e.Filter.DisplayName;
            filterButton.BorderThickness = new Thickness(0);
            filterButton.Background = new SolidColorBrush(Colors.Transparent);
            filterButton.Margin = new Thickness(0);
            FilterListView.Children.Insert(0, filterButton);
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
            originalPreviewBitmap = bmp.Resize((int)PhotoView.RenderSize.Width, (int)PhotoView.RenderSize.Height, System.Windows.Media.Imaging.WriteableBitmapExtensions.Interpolation.Bilinear);

            // Create buffer
            previewStream = new MemoryStream();
            originalPreviewBitmap.SaveJpeg(previewStream, originalPreviewBitmap.PixelWidth, originalPreviewBitmap.PixelHeight, 0, 75);
            previewBuffer = previewStream.GetWindowsRuntimeBuffer();

        }

        private void UpdateCurrentPreviewImageExcluveFilter(FilterBase filter)
        {
            
        }

    }
}
