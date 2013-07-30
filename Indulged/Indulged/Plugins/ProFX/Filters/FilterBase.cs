using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Windows.Storage.Streams;
using Indulged.Plugins.ProFX.Events;

namespace Indulged.Plugins.ProFX.Filters
{
    public class FilterBase : UserControl
    {
        // Buffer
        public IBuffer Buffer { get; set; }

        // Original Bitmap Image
        public WriteableBitmap OriginalImage { get; set; }

        // Current preview image
        public WriteableBitmap CurrentImage { get; set; }

        public string DisplayName { get; set; }

        // Filter reference
        public IFilter Filter { get; set; }

        protected virtual void CreateFilter()
        {
            // Do nothing
        }

        public virtual void OnFilterUIAdded()
        {
            UpdatePreviewAsync();
        }

        public void OnDeleteFilter(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        public async void DeleteFilterAsync()
        {
            using (EditingSession session = new EditingSession(Buffer))
            {
                // Add all previous filters
                foreach (FilterBase filterContainer in ImageProcessingPage.AppliedFilters)
                {
                    if (filterContainer != this)
                    {
                        session.AddFilter(filterContainer.Filter);
                    }
                }

                await session.RenderToWriteableBitmapAsync(CurrentImage, OutputOption.PreserveAspectRatio);
                CurrentImage.Invalidate();
            }

            var evt = new DeleteFilterEventArgs();
            evt.Filter = this;
            ImageProcessingPage.RequestDeleteFilter(this, evt);
        }

        protected async void UpdatePreviewAsync()
        {
            using (EditingSession session = new EditingSession(Buffer))
            {
                CreateFilter();

                // Add all previous filters
                foreach (FilterBase filterContainer in ImageProcessingPage.AppliedFilters)
                {
                    if (filterContainer.Filter != this.Filter)
                    {
                        session.AddFilter(filterContainer.Filter);
                    }
                }

                session.AddFilter(Filter);
                await session.RenderToWriteableBitmapAsync(CurrentImage, OutputOption.PreserveAspectRatio);
                CurrentImage.Invalidate();
            }


        }
    }
}
