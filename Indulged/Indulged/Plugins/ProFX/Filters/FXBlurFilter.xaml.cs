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
    public partial class FXBlurFilter : FilterBase
    {
        private BlurLevel blurLevel;

        public FXBlurFilter()
        {
            InitializeComponent();

            DisplayName = "blur";
            StatusBarName = "Blur";

            blurLevel = BlurLevel.Blur3;
            Category = FilterCategory.Effect;
        }

        public override void CreateFilter()
        {
            Filter = FilterFactory.CreateBlurFilter(blurLevel);
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AmountSlider == null)
                return;

            int intValue = (int)AmountSlider.Value;

            if ((BlurLevel)intValue != blurLevel)
            {
                blurLevel = (BlurLevel)intValue;
                UpdatePreviewAsync();
            }
            
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
