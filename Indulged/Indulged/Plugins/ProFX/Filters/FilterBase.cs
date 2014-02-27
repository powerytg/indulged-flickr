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
    public enum FilterCategory 
    {
        Color,
        Transform,
        Effect,
        Enhancement
    };

    public class FilterBase : UserControl
    {
        // Events
        public EventHandler FilterWillBeRemoved;
        public EventHandler InvalidatePreview;

        private bool _isFilterEnabled = true;
        public bool IsFilterEnabled 
        {
            get
            {
                return _isFilterEnabled;
            }

            set
            {
                _isFilterEnabled = value;
            }
        }

        // Buffer
        public IBuffer Buffer { get; set; }

        // Original Bitmap Image
        public WriteableBitmap OriginalImage { get; set; }

        // Current preview image
        public WriteableBitmap CurrentImage { get; set; }
        public WriteableBitmap OriginalPreviewImage { get; set; }

        public string DisplayName { get; set; }
        public string StatusBarName { get; set; }

        public FilterCategory Category { get; set; }

        public virtual bool hasEditorUI 
        {
            get
            {
                return true;
            }
        }

        // Filter reference
        public IFilter Filter { get; set; }

        // Filter for final output
        public virtual IFilter FinalOutputFilter
        {
            get
            {
                return Filter;
            }
        }

        public virtual void CreateFilter()
        {
            // Do nothing
        }

        public virtual void OnFilterUIAdded()
        {
            UpdatePreviewAsync();
        }

        public virtual void OnFilterUIDismissed()
        {
            // Do nothing
        }

        public void OnDeleteFilter(object sender, RoutedEventArgs e)
        {
            DeleteFilter();
        }

        protected virtual void DeleteFilter()
        {
            if (FilterWillBeRemoved != null)
            {
                FilterWillBeRemoved(this, null);
            }
        }

        protected virtual void DeleteFilterAsync()
        {
            DeleteFilter();
        }

        public virtual void UpdatePreviewAsync()
        {
            CreateFilter();

            if (InvalidatePreview != null)
            {
                InvalidatePreview(this, null);
            }
        }

    }
}
