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

            DisplayName = "adjust color";
            redLevel = 0;
            greenLevel = 0;
            blueLevel = 0;
        }

        protected override void CreateFilter()
        {
            Filter = FilterFactory.CreateColorAdjustFilter(redLevel, greenLevel, blueLevel);
        }

        private void RedAmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (RedAmountSlider == null)
                return;
            
            redLevel = RedAmountSlider.Value;
            UpdatePreview();
        }

        private void GreenAmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GreenAmountSlider == null)
                return;

            greenLevel = GreenAmountSlider.Value;
            UpdatePreview();
        }

        private void BlueAmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (BlueAmountSlider == null)
                return;

            blueLevel = BlueAmountSlider.Value;
            UpdatePreview();
        }

    }
}
