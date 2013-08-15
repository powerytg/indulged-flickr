using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.Plugins.ProFX.Events;
using Indulged.Plugins.Chrome;
using System.Windows.Media;
using Indulged.Plugins.Chrome.Events;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.ProFX
{
    public partial class FXNormalStatusView : UserControl
    {
        private Image lightIcon = new Image();
        private Image darkIcon = new Image();

        public FXNormalStatusView()
        {
            InitializeComponent();

            lightIcon.Source = new BitmapImage(new Uri("/Assets/ProFX/FXArrowUpWhite.png", UriKind.Relative));
            darkIcon.Source = new BitmapImage(new Uri("/Assets/ProFX/FXArrowUp.png", UriKind.Relative));

            ApplyTheme();

            // Events
            ThemeManager.ThemeChanged += OnThemeChanged;

        }

        private void ApplyTheme()
        {
            if (ThemeManager.CurrentTheme == Themes.Dark)
            {
                TitleButton.Content = darkIcon;
            }
            else
            {
                TitleButton.Content = lightIcon;
            }
        }

        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            ApplyTheme();
        }

        private void TitleButton_Click(object sender, RoutedEventArgs e)
        {
            ImageProcessingPage.RequestFilterListView(this, null);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ImageProcessingPage.RequestFilterListView(this, null);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ImageProcessingPage.RequestSettingsView(this, null);
        }

    }
}
