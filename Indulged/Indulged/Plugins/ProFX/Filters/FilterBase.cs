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

namespace Indulged.Plugins.ProFX.Filters
{
    public class FilterBase : UserControl
    {
        // Buffer
        public IBuffer Buffer { get; set; }

        // Original Bitmap Image
        public WriteableBitmap OriginalImage { get; set; }

        public string DisplayName { get; set; }

        // Filter reference
        public IFilter Filter { get; set; }

        protected virtual void CreateFilter()
        {
            // Do nothing
        }

        public virtual void OnFilterUIAdded()
        {
            UpdatePreview();
        }

        protected async void UpdatePreview()
        {
            using (EditingSession session = new EditingSession(Buffer))
            {
                CreateFilter();

                session.AddFilter(Filter);
                await session.RenderToWriteableBitmapAsync(OriginalImage, OutputOption.PreserveAspectRatio);
                OriginalImage.Invalidate();
            }


        }
    }
}
