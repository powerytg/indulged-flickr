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
    public partial class FXColorAdjustmentFilter : FilterBase
    {
        private double redLevel;
        private double greenLevel;
        private double blueLevel;

        public FXColorAdjustmentFilter()
        {
            InitializeComponent();

            DisplayName = "rgb";
            StatusBarName = "Adjust Color";
            Category = FilterCategory.Color;

            redLevel = 0;
            greenLevel = 0;
            blueLevel = 0;
        }

        public override void CreateFilter()
        {
            Filter = FilterFactory.CreateColorAdjustFilter(redLevel, greenLevel, blueLevel);
        }

        private void RedAmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (RedAmountSlider == null)
                return;
            
            redLevel = RedAmountSlider.Value;
            UpdatePreviewAsync();
        }

        private void GreenAmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GreenAmountSlider == null)
                return;

            greenLevel = GreenAmountSlider.Value;
            UpdatePreviewAsync();
        }

        private void BlueAmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (BlueAmountSlider == null)
                return;

            blueLevel = BlueAmountSlider.Value;
            UpdatePreviewAsync();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

    }
}
