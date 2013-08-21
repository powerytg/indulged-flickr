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

            DisplayName = "crop";

            // Events
            ImageProcessingPage.CropAreaChanged += OnCropAreaChanged;
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

        protected override void CreateFilter()
        {
            if (cropRect.Width == 0 || cropRect.Height == 0)
            {
                cropRect.Width = OriginalPreviewImage.PixelWidth / 2;
                cropRect.Height = OriginalPreviewImage.PixelHeight / 2;
                
            }

            Filter = FilterFactory.CreateCropFilter(cropRect);
        }

        public override async void DeleteFilterAsync()
        {
            cropRect = new Windows.Foundation.Rect(0, 0, 0, 0);
            base.DeleteFilterAsync();
        }

        public override void OnFilterUIAdded()
        {
            // Show clipping UI
            ImageProcessingPage.RequestCropView(this, null);
        }

        public override void OnFilterUIDismissed()
        {
            if(cropRect.Width != 0 && cropRect.Height != 0)
                UpdatePreviewAsync();

            ImageProcessingPage.RequestDismissCropView(this, null);

        }

        private void OnCropAreaChanged(object sender, CropAreaChangedEventArgs e)
        {
            // Translate coordinations
            cropRect.X = e.X;
            cropRect.Y = e.Y;
            cropRect.Width = e.Width;
            cropRect.Height = e.Height;
        }
    }
}
