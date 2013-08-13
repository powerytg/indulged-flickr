﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Nokia.Graphics.Imaging;

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
        }

        public override bool hasEditorUI
        {
            get
            {
                return false;
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
