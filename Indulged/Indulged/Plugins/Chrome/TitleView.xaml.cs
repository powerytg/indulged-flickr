using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.Chrome
{
    public partial class TitleView : UserControl
    {
        public static readonly DependencyProperty TitlelessProperty = DependencyProperty.Register("Titleless", typeof(bool), typeof(TitleView), new PropertyMetadata(OnTitlelessPropertyChanged));

        public bool Titleless
        {
            get
            {
                return (bool)GetValue(TitlelessProperty);
            }
            set
            {
                SetValue(TitlelessProperty, value);
            }
        }

        public static void OnTitlelessPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TitleView)sender).OnTitlelessChanged();
        }

        protected virtual void OnTitlelessChanged()
        {            
            BackgroundImage.Source = new BitmapImage(new Uri("/Assets/Chrome/BlankTitleView.png", UriKind.RelativeOrAbsolute));
        }

        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register("Theme", typeof(Themes), typeof(TitleView), new PropertyMetadata(OnThemePropertyChanged));
        
        public Themes Theme
        {
            get
            {
                return (Themes)GetValue(ThemeProperty);
            }
            set
            {
                SetValue(ThemeProperty, value);
            }
        }

        public static void OnThemePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TitleView)sender).OnThemeChanged();
        }

        protected virtual void OnThemeChanged()
        {
            if (Titleless)
            {
                BackgroundImage.Source = new BitmapImage(new Uri("/Assets/Chrome/BlankTitleView.png", UriKind.RelativeOrAbsolute));
            }
            else if (Theme == Themes.Dark)
            {
                BackgroundImage.Source = new BitmapImage(new Uri("/Assets/Chrome/DarkTitleView.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                BackgroundImage.Source = new BitmapImage(new Uri("/Assets/Chrome/LightTitleView.png", UriKind.RelativeOrAbsolute));
            }
        }

        // Constructor
        public TitleView()
        {
            InitializeComponent();
        }

    }
}
