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
        public double Degree { get; set; }

        public FXRotationFilter()
        {
            InitializeComponent();
            Degree = 0.0;
            Category = FilterCategory.Transform;

            DisplayName = "rotation";
            StatusBarName = "Rotate Image";
        }

        public override void CreateFilter()
        {
            Filter = FilterFactory.CreateFreeRotationFilter(Degree, RotationResizeMode.FitInside);
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AmountSlider == null)
                return;

            Degree = AmountSlider.Value;
            UpdatePreviewAsync();
        }

    }
}
