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
    public partial class FilterBackground : UserControl
    {
        public FilterBackground()
        {
            InitializeComponent();
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
    }
}
