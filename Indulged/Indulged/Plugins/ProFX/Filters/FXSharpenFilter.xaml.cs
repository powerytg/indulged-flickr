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
    public partial class FXSharpenFilter : FilterBase
    {
        private SharpnessLevel level = SharpnessLevel.Level2;

        public FXSharpenFilter()
        {
            InitializeComponent();

            DisplayName = "sharpen";
        }

        protected override void CreateFilter()
        {
            Filter = FilterFactory.CreateSharpnessFilter(level);
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AmountSlider == null)
                return;

            int intValue = (int)AmountSlider.Value;

            if ((SharpnessLevel)intValue != level)
            {
                level = (SharpnessLevel)intValue;
                UpdatePreviewAsync();
            }
            
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
