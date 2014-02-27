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

namespace Indulged.Plugins.ProFX.Filters
{
    public partial class FXBlackWhiteFilter : FilterBase
    {
        private UInt32 smoothness = 2;
        private UInt32 threshold = 128;

        public FXBlackWhiteFilter()
        {
            InitializeComponent();

            DisplayName = "b+w";
            StatusBarName = "Black/White";
            Category = FilterCategory.Color;
        }

        public override void CreateFilter()
        {
            Filter = FilterFactory.CreateStampFilter(smoothness, threshold);
        }

        private void SmoothAmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SmoothAmountSlider == null)
                return;

            UInt32 intValue = (UInt32)SmoothAmountSlider.Value;
            smoothness = intValue;
            UpdatePreviewAsync();
        }

        private void ThresholdAmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ThresholdAmountSlider == null)
                return;

            UInt32 intValue = (UInt32)ThresholdAmountSlider.Value;
            threshold = intValue;
            UpdatePreviewAsync();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
