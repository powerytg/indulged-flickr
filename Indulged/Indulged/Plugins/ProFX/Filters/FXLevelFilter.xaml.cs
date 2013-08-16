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
    public partial class FXLevelFilter : FilterBase
    {
        private bool shouldUseAutoLevel = true;
        private double midLevel = 0.5;

        public FXLevelFilter()
        {
            InitializeComponent();

            DisplayName = "levels";
            StatusBarName = "Levels";
        }

        protected override void CreateFilter()
        {
            if (shouldUseAutoLevel)
            {
                Filter = FilterFactory.CreateAutoLevelsFilter();
            }
            else
            {
                Filter = FilterFactory.CreateLevelsFilter(1.0, midLevel, 0.0);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (AutoLevelSwitch == null)
                return;

            MidToneSlider.IsEnabled = false;
            shouldUseAutoLevel = true;
            UpdatePreviewAsync();
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (MidToneSlider == null)
                return;

            MidToneSlider.IsEnabled = true;
            shouldUseAutoLevel = false;
            UpdatePreviewAsync();
        }

        private void MidToneSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MidToneSlider == null)
                return;

            midLevel = MidToneSlider.Value;
            UpdatePreviewAsync();
        }

        
    }
}
