using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Nokia.Graphics.Imaging;

using Indulged.Plugins.ProFX;
using Indulged.Plugins.ProFX.Events;

namespace Indulged.Plugins.ProFX.Filters
{
    public partial class FXCropFilter : FilterBase
    {
        // Crop area
        private Windows.Foundation.Rect cropRect = new Windows.Foundation.Rect(0, 0, 0, 0);

        // Constructor
        public FXCropFilter()
        {
            InitializeComponent();
            Category = FilterCategory.Transform;

            DisplayName = "crop";
        }

        public override bool hasEditorUI
        {
            get
            {
                return false;
            }
        }

        public override IFilter FinalOutputFilter
        {
            get
            {
                double xFactor = OriginalImage.PixelWidth / CurrentImage.PixelWidth;
                double yFactor = OriginalImage.PixelHeight / CurrentImage.PixelHeight;

                Windows.Foundation.Rect finalCropRect = new Windows.Foundation.Rect(cropRect.X * xFactor, cropRect.Y * yFactor, cropRect.Width * xFactor, cropRect.Height * yFactor);

                IFilter finalCropFilter = FilterFactory.CreateCropFilter(finalCropRect);
                return finalCropFilter;
            }
        }

        public override void CreateFilter()
        {
            if (cropRect.Width == 0 || cropRect.Height == 0)
            {
                cropRect.Width = OriginalPreviewImage.PixelWidth / 2;
                cropRect.Height = OriginalPreviewImage.PixelHeight / 2;
                
            }

            Filter = FilterFactory.CreateCropFilter(cropRect);
        }

        override protected void DeleteFilter()
        {
            cropRect = new Windows.Foundation.Rect(0, 0, 0, 0);
            base.DeleteFilter();
        }

        public void UpdateCropRect(double x, double y, double w, double h)
        {
            // Translate coordinations
            cropRect.X = x;
            cropRect.Y = y;
            cropRect.Width = w;
            cropRect.Height = h;
        }
    }
}
