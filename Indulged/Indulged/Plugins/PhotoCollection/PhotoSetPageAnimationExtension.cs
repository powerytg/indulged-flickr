using Indulged.API.Anaconda;
using Indulged.API.Cinderella.Models;
using Indulged.Plugins.PhotoCollection.Renderers;
using Indulged.Resources;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Indulged.Plugins.PhotoCollection
{
    public partial class PhotoSetPage
    {
        private void PerformAppearanceAnimation()
        {
            double w = System.Windows.Application.Current.Host.Content.ActualWidth;
            double h = System.Windows.Application.Current.Host.Content.ActualHeight;

            CompositeTransform ct = (CompositeTransform)LayoutRoot.RenderTransform;
            ct.TranslateY = h;

            LayoutRoot.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            galleryAnimation.To = 0.0;
            galleryAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, LayoutRoot);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);
            animation.Begin();
            animation.Completed += (sender, e) =>
            {
                OnPageAppeared();
            };
        }

        private void PerformDisappearanceAnimation()
        {
            double w = System.Windows.Application.Current.Host.Content.ActualWidth;
            double h = System.Windows.Application.Current.Host.Content.ActualHeight;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            yAnimation.To = h;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(yAnimation, LayoutRoot);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(yAnimation);
            animation.Begin();
        }
    }
}
