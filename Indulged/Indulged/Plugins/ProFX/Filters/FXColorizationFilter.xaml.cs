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
using System.Windows.Media;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Indulged.Plugins.ProFX.Filters
{
    
    public partial class FXColorizationmentFilter : FilterBase
    {
        private Color toneColor = Color.FromArgb(0xff, 0xab, 0xf3, 0x50);
        private short luminance = 0;
        private short chrominance = 100;

        public FXColorizationmentFilter()
        {
            InitializeComponent();
            Category = FilterCategory.Color;

            DisplayName = "colorization";
            StatusBarName = "Colorization";
        }

        public override void CreateFilter()
        {
            Filter = FilterFactory.CreateColorizationFilter(toneColor.R, toneColor.G, toneColor.B, luminance, chrominance);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        private void LuminanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(LuminanceSlider == null)
                return;

            short intValue = (short)LuminanceSlider.Value;
            luminance = intValue;
            UpdatePreviewAsync();
        }

        private void ChrominanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(ChrominanceSlider == null)
                return;

            short intValue = (short)ChrominanceSlider.Value;
            chrominance = intValue;
            UpdatePreviewAsync();

        }

        private void OnSelectedColorChanged(object sender, EventArgs e)
        {
            toneColor = PickerButton.SelectedColor;
            UpdatePreviewAsync();
        }

    }
}
