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
    public partial class FXVignetteFilter : FilterBase
    {
        private Windows.UI.Color vinegetteColor = Windows.UI.Color.FromArgb(0xff, 0, 0, 0);
        private double radius = 0.6;

        public FXVignetteFilter()
        {
            InitializeComponent();

            DisplayName = "vignette";
            StatusBarName = "Vigenette";
        }

        protected override void CreateFilter()
        {
            Filter = FilterFactory.CreateVignettingFilter(radius, vinegetteColor);
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AmountSlider == null)
                return;

            radius = AmountSlider.Value;
            UpdatePreviewAsync();
        }

        private void OnSelectedColorChanged(object sender, EventArgs e)
        {
            vinegetteColor = Windows.UI.Color.FromArgb(0xff, PickerButton.SelectedColor.R, PickerButton.SelectedColor.G, PickerButton.SelectedColor.B);
            UpdatePreviewAsync();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
