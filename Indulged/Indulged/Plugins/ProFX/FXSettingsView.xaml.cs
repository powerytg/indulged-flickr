using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.Plugins.Chrome;
using System.Windows.Media;
using Indulged.Plugins.Chrome.Events;

namespace Indulged.Plugins.ProFX
{
    public partial class FXSettingsView : UserControl
    {
        public FXSettingsView()
        {
            InitializeComponent();

            if (ThemeManager.CurrentTheme == Themes.Dark)
            {
                BackgroundPicker.SelectedIndex = 0;
            }
            else
            {
                BackgroundPicker.SelectedIndex = 1;
            }

            ApplyTheme();

            // Events
            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void ApplyTheme()
        {
            if (ThemeManager.CurrentTheme == Themes.Dark)
            {
                LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(216, 0, 0, 0));
            }
            else
            {
                LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(216, 0xff, 0xff, 0xff));
            }
        }

        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            ApplyTheme();
        }

        private void FitRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (FitRadioButton == null)
                return;

            PhotoPreviewer.RequestChangeAspectRatioToFit(this, null);
        }

        private void FillRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (FillRadioButton == null)
                return;

            PhotoPreviewer.RequestChangeAspectRatioToFill(this, null);
        }

        private void BackgroundPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BackgroundPicker == null)
                return;

            if (BackgroundPicker.SelectedIndex == 0)
                ThemeManager.CurrentTheme = Themes.Dark;
            else
                ThemeManager.CurrentTheme = Themes.Light;
        }
    }
}
