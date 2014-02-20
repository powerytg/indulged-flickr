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
    public partial class FXHueSaturationFilter : FilterBase
    {
        private uint hue = 128;
        private uint saturation = 128;

        public FXHueSaturationFilter()
        {
            InitializeComponent();
            Category = FilterCategory.Color;

            DisplayName = "hue/saturation";
            StatusBarName = "Hue/Saturation";
        }

        public override void CreateFilter()
        {
            Filter = FilterFactory.CreateHueSaturationFilter(hue, saturation);
        }

        private void HueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (HueSlider == null)
                return;

            hue = (uint)HueSlider.Value;
            UpdatePreviewAsync();
        }

        private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SaturationSlider == null)
                return;

            saturation = (uint)SaturationSlider.Value;
            UpdatePreviewAsync();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
