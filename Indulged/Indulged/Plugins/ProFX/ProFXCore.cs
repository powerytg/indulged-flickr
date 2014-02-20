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
using Indulged.Plugins.ProFX.Filters;
using Nokia.Graphics.Imaging;
using Indulged.API.Avarice.Controls;
using Indulged.Resources;
using Indulged.API.Avarice.Events;

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

        // Filter manager
        private FXFilterManager filterManager;

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

        private async void UpdatePreviewAsync()
        {
            using (EditingSession session = new EditingSession(previewBuffer))
            {
                // Add all previous filters
                foreach (FilterBase filterContainer in filterManager.AppliedFilters)
                {
                    session.AddFilter(filterContainer.Filter);
                }

                await session.RenderToWriteableBitmapAsync(currentPreviewBitmap, OutputOption.PreserveAspectRatio);
                currentPreviewBitmap.Invalidate();
            }
        }

        private void PerformAutoEnhance()
        {
            if (filterManager.HasAppliedFilterOtherThan(FilterCategory.Transform))
            {
                var dialog = ModalPopup.Show("This will clear all your previous applied filters. Do you wish to continue?",
                    "Auto Enhance", new List<string> { AppResources.GenericConfirmText, AppResources.GenericCancelText });
                dialog.DismissWithButtonClick += (s, args) =>
                {
                    int buttonIndex = (args as ModalPopupEventArgs).ButtonIndex;
                    if (buttonIndex == 0)
                    {
                        AddAutoEnhanceFilters();
                    }
                };
            }
            else
            {
                AddAutoEnhanceFilters();
            }
        }

        private void AddAutoEnhanceFilters()
        {
            filterManager.ClearAllFiltersOtherThan(FilterCategory.Transform);

            foreach (var filter in filterManager.AutoEnhanceFilters)
            {
                filter.OriginalImage = originalBitmap;
                filter.CurrentImage = currentPreviewBitmap;
                filter.OriginalPreviewImage = originalPreviewBitmap;
                filter.Buffer = previewBuffer;
            }

            filterManager.AutoEnhance();
        }

    }
}
