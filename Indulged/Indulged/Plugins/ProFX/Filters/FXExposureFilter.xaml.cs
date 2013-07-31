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
    public partial class FXExposureFilter : FilterBase
    {
        private ExposureMode expMode = ExposureMode.Natural;
        private double gain = 0;

        public FXExposureFilter()
        {
            InitializeComponent();

            DisplayName = "exposure";
        }

        protected override void CreateFilter()
        {
            Filter = FilterFactory.CreateExposureFilter(expMode, gain);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        private void NaturalModeButton_Checked(object sender, RoutedEventArgs e)
        {
            if (NaturalModeButton == null)
                return;

            expMode = ExposureMode.Natural;
            UpdatePreviewAsync();
        }

        private void GammaModeButton_Checked(object sender, RoutedEventArgs e)
        {
            if (GammaModeButton == null)
                return;

            expMode = ExposureMode.Gamma;
            UpdatePreviewAsync();
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AmountSlider == null)
                return;

            gain = AmountSlider.Value;
            UpdatePreviewAsync();
        }

        
    }
}
