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
    public partial class FXLomoFilter : FilterBase
    {
        private LomoStyle style = LomoStyle.Neutral;
        private LomoVignetting vignetting = LomoVignetting.Medium;
        private double brightness = 0.5;
        private double saturation = 0;

        public FXLomoFilter()
        {
            InitializeComponent();
            Category = FilterCategory.Effect;

            DisplayName = "lomography";
            StatusBarName = "Lomography";
        }

        public override void CreateFilter()
        {
            Filter = FilterFactory.CreateLomoFilter(brightness, saturation, vignetting, style);
        }

       

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        private void TintPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TintPicker == null)
                return;

            style = (LomoStyle)TintPicker.SelectedIndex;
            UpdatePreviewAsync();
        }

        private void StrengthPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StrengthPicker == null)
                return;

            vignetting = (LomoVignetting)StrengthPicker.SelectedIndex;
            UpdatePreviewAsync();
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (BrightnessSlider == null)
                return;

            brightness = BrightnessSlider.Value;
            UpdatePreviewAsync();
        }

        private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SaturationSlider == null)
                return;

            saturation = SaturationSlider.Value;
            UpdatePreviewAsync();
        }
        
    }
}
