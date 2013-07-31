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
    
    public partial class FXMonoColorFilter : FilterBase
    {
        private Windows.UI.Color preservedColor = Windows.UI.Color.FromArgb(0xff, 0xcc, 0x00, 0x00);
        private uint tolerance = 40;

        public FXMonoColorFilter()
        {
            InitializeComponent();

            DisplayName = "mono color";

        }

        protected override void CreateFilter()
        {
            Filter = FilterFactory.CreateMonoColorFilter(preservedColor, tolerance);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        private void ToleranceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ToleranceSlider == null)
                return;

            uint intValue = (uint)ToleranceSlider.Value;
            tolerance = intValue;
            UpdatePreviewAsync();
        }

        private void Picker_ColorChanged(object sender, Color color)
        {
            preservedColor = Windows.UI.Color.FromArgb(0xff, color.R, color.G, color.B);
            UpdatePreviewAsync();
        }

    }
}
