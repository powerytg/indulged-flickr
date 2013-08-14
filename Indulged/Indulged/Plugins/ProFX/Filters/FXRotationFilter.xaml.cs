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
    public partial class FXRotationFilter : FilterBase
    {
        private double degree = 0.0;

        public FXRotationFilter()
        {
            InitializeComponent();

            DisplayName = "rotation";
        }

        protected override void CreateFilter()
        {
            Filter = FilterFactory.CreateFreeRotationFilter(degree, RotationResizeMode.FitInside);
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AmountSlider == null)
                return;

            degree = AmountSlider.Value;
            UpdatePreviewAsync();
        }

    }
}
