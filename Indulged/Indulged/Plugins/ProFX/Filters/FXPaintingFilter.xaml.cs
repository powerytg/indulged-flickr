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
    public partial class FXPaintingFilter : FilterBase
    {
        private uint level = 2;

        public FXPaintingFilter()
        {
            InitializeComponent();

            DisplayName = "painting";
            StatusBarName = "Painting Effect";
        }

        protected override void CreateFilter()
        {
            Filter = FilterFactory.CreatePaintFilter(level);
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AmountSlider == null)
                return;

            uint intValue = (uint)AmountSlider.Value;
            level = intValue;
            UpdatePreviewAsync();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
