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
    public partial class FXColorBoostFilter : FilterBase
    {
        private double gain = 0;

        public FXColorBoostFilter()
        {
            InitializeComponent();

            DisplayName = "vibrance";
            StatusBarName = "Vibrance";
        }

        protected override void CreateFilter()
        {
            Filter = FilterFactory.CreateColorBoostFilter(gain);
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AmountSlider == null)
                return;

            gain = AmountSlider.Value;
            UpdatePreviewAsync();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
