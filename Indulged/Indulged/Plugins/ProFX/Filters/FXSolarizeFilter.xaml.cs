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
    public partial class FXSolarizeFilter : FilterBase
    {
        private float level = 0.4f;

        public FXSolarizeFilter()
        {
            InitializeComponent();

            DisplayName = "solarize";
        }

        protected override void CreateFilter()
        {
            Filter = FilterFactory.CreateSolarizeFilter(level);
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AmountSlider == null)
                return;

            level = (float)AmountSlider.Value;
            UpdatePreviewAsync();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
