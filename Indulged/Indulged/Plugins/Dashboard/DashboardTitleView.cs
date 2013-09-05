using Indulged.Plugins.Chrome;
using Indulged.Plugins.Dashboard.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Indulged.Plugins.Dashboard
{
    public partial class DashboardTitleView : TitleView
    {
        private IDashboardPage selectedDashboardPage;

        // Constructor
        public DashboardTitleView()
            : base()
        {
            // Additional events
            DashboardNavigator.DashboardPageChanged += OnDashboardPageChanged;
        }

        private void OnDashboardPageChanged(object sender, DashboardPageEventArgs e)
        {
            selectedDashboardPage = e.SelectedPage;

            if (selectedDashboardPage.ShouldUseLightBackground && BackgroundImage.Source == lightBackgroundImage)
                return;

            if (!selectedDashboardPage.ShouldUseLightBackground && BackgroundImage.Source == darkBackgroundImage)
                return;


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
            if (selectedDashboardPage.ShouldUseLightBackground)
                BackgroundImage.Source = lightBackgroundImage;
            else
                BackgroundImage.Source = darkBackgroundImage;

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
