using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.Plugins.Dashboard;
using Indulged.Plugins.Dashboard.Events;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Indulged.Plugins.Chrome
{
    public partial class BackgroundView : UserControl
    {
        public BackgroundView()
        {
            InitializeComponent();

            // Events
            DashboardNavigator.DashboardPageChanged += DashboardPageChanged;
        }

        // Current selected navigator item
        private IDashboardPage selectedDashboardPage;

        // Navigator changed event
        private void DashboardPageChanged(object sender, DashboardPageEventArgs e)
        {
            selectedDashboardPage = e.SelectedPage;

            // Fade out the old image
            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation fadeOutAnimation = new DoubleAnimation();
            animation.Children.Add(fadeOutAnimation);
            fadeOutAnimation.Duration = animation.Duration;
            fadeOutAnimation.To = 0;
            Storyboard.SetTarget(fadeOutAnimation, BackgroundImage);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));

            animation.Completed += FadeOutAnimationCompleted;
            animation.Begin();
        }

        private void FadeOutAnimationCompleted(object sender, EventArgs e)
        {
            if (selectedDashboardPage.BackgroundImageUrl != null)
                BackgroundImage.Source = new BitmapImage(new Uri(selectedDashboardPage.BackgroundImageUrl, UriKind.Relative));
            else
                BackgroundImage.Source = null;

            // Change background color
            if (selectedDashboardPage.ShouldUseLightBackground)
                LayoutRoot.Background = new SolidColorBrush(Colors.White);
            else
                LayoutRoot.Background = new SolidColorBrush(Colors.Black);

            // Fade in the new image
            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation fadeInAnimation = new DoubleAnimation();
            animation.Children.Add(fadeInAnimation);
            fadeInAnimation.Duration = animation.Duration;
            fadeInAnimation.To = 1;
            Storyboard.SetTarget(fadeInAnimation, BackgroundImage);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
            animation.Begin();
        }
         
    }
}
