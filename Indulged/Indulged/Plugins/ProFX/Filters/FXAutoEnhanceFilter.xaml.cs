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
    public partial class FXAutoEnhanceFilter : FilterBase
    {
        private bool isAutoBrightnessContrastOn = true;
        private bool isAutoClarityOn = true;
        private AutoEnhanceConfiguration config = new AutoEnhanceConfiguration();

        public FXAutoEnhanceFilter()
        {
            InitializeComponent();

            DisplayName = "auto enhance";
            StatusBarName = "Auto Enhance";
        }

        protected override void CreateFilter()
        {
            if (isAutoBrightnessContrastOn)
                config.ApplyAutomaticContrastAndBrightness();
            else
                config.ApplyContrastAndBrightnessOff();

            if (isAutoClarityOn)
                config.ApplyAutomaticLocalBoost();
            else
                config.ApplyLocalBoostOff();

            Filter = FilterFactory.CreateAutoEnhanceFilter(config);
        }

        private void OnBrightnessToggleChecked(object sender, RoutedEventArgs e)
        {
            if (BrightnessToggle == null)
                return;

            isAutoBrightnessContrastOn = true;
            UpdatePreviewAsync();
        }

        private void OnBrightnessToggleUnchecked(object sender, RoutedEventArgs e)
        {
            if (BrightnessToggle == null)
                return;
            
            isAutoBrightnessContrastOn = false;
            UpdatePreviewAsync();
        }

        private void OnClarityToggleChecked(object sender, RoutedEventArgs e)
        {
            if (ClarityToggle == null)
                return;
            
            isAutoClarityOn = true;
            UpdatePreviewAsync();
        }

        private void OnClarityToggleUnchecked(object sender, RoutedEventArgs e)
        {
            if (ClarityToggle == null)
                return;

            isAutoClarityOn = false;
            UpdatePreviewAsync();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        
    }
}
