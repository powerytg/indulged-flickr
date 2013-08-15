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
    public partial class FXClarityFilter : FilterBase
    {
        private bool shouldUseAutoClarity = true;
        private int autoLevel = 8;
        private double gamma = 1.9;
        private double black = 0.625;
        private double white = 0.5;
        private double strength = 256;

        public FXClarityFilter()
        {
            InitializeComponent();

            DisplayName = "clarity";
            StatusBarName = "Clarity";
        }

        protected override void CreateFilter()
        {
            if (shouldUseAutoClarity)
            {
                Filter = FilterFactory.CreateLocalBoostFilter(autoLevel);
            }
            else
            {
                Filter = FilterFactory.CreateLocalBoostFilter(gamma, black, white, strength);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteFilterAsync();
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (AutoBoostSwitch == null)
                return;

            GammaSlider.IsEnabled = false;
            WhiteSlider.IsEnabled = false;
            BlackSlider.IsEnabled = false;
            StrengthSlider.IsEnabled = false;

            AutoLevelSlider.IsEnabled = true;

            shouldUseAutoClarity = true;
            UpdatePreviewAsync();
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (AutoBoostSwitch == null)
                return;

            GammaSlider.IsEnabled = true;
            WhiteSlider.IsEnabled = true;
            BlackSlider.IsEnabled = true;
            StrengthSlider.IsEnabled = true;

            AutoLevelSlider.IsEnabled = false;

            shouldUseAutoClarity = false;
            UpdatePreviewAsync();
        }

        private void AutoLevelSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AutoLevelSlider == null)
                return;

            autoLevel = (int)AutoLevelSlider.Value;
            UpdatePreviewAsync();
        }

        private void GammaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GammaSlider == null)
                return;

            gamma = GammaSlider.Value;
            UpdatePreviewAsync();
        }

        private void BlackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (BlackSlider == null)
                return;

            black = BlackSlider.Value;
            UpdatePreviewAsync();
        }

        private void WhiteSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WhiteSlider == null)
                return;

            white = WhiteSlider.Value;
            UpdatePreviewAsync();
        }

        private void StrengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (StrengthSlider == null)
                return;

            strength = StrengthSlider.Value;
            UpdatePreviewAsync();
        }

    }
}
