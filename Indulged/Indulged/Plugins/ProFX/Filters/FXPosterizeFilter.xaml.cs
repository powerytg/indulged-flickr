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
    public partial class FXPosterizeFilter : FilterBase
    {
        private ushort level = 5;

        public FXPosterizeFilter()
        {
            InitializeComponent();
            Category = FilterCategory.Effect;

            DisplayName = "posterize";
            StatusBarName = "Posterize";
        }

        public override void CreateFilter()
        {
            Filter = FilterFactory.CreatePosterizeFilter(level);
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AmountSlider == null)
                return;

            ushort intValue = (ushort)AmountSlider.Value;
            level = intValue;
            UpdatePreviewAsync();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
