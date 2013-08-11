using Indulged.Plugins.ProFX.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Indulged.Plugins.ProFX
{
    public partial class ImageProcessingPage
    {
        
        private void ShowFilterListView()
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            CompositeTransform ct = (CompositeTransform)galleryView.RenderTransform;
            ct.TranslateY = galleryView.Height;

            galleryView.Opacity = 0;
            galleryView.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            galleryAnimation.To = 0.0;
            galleryAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, galleryView);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);

            // Alpha animation
            DoubleAnimation alphaAnimation = new DoubleAnimation();
            alphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            alphaAnimation.To = 1.0;
            alphaAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(alphaAnimation, galleryView);
            Storyboard.SetTargetProperty(alphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(alphaAnimation);


            animation.Begin();
        }

        private void ShowFilterControlView(FilterBase filter)
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            // Add the request filter view to screen
            filter.VerticalAlignment = VerticalAlignment.Bottom;
            filter.Margin = new Thickness(0, 0, 0, BottomPanel.Height);
            filter.Visibility = Visibility.Collapsed;
            filter.CurrentImage = currentPreviewBitmap;
            ProcessorPage.Children.Add(filter);

            CompositeTransform ct = (CompositeTransform)filter.RenderTransform;
            ct.TranslateY = filter.Height;

            filter.Opacity = 0;
            filter.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.6));

            // Gallery Y animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            galleryAnimation.To = galleryView.Height;
            galleryAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, galleryView);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);

            // Gallery Alpha animation
            DoubleAnimation alphaAnimation = new DoubleAnimation();
            alphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            alphaAnimation.To = 0.0;
            alphaAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(alphaAnimation, galleryView);
            Storyboard.SetTargetProperty(alphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(alphaAnimation);

            // Filter view Y animation
            DoubleAnimationUsingKeyFrames filterYAnimation = new DoubleAnimationUsingKeyFrames();
            filterYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.3), Value = filter.Height, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } });
            filterYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.6), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } });
            Storyboard.SetTarget(filterYAnimation, filter);
            Storyboard.SetTargetProperty(filterYAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(filterYAnimation);

            // Filter view alpha animation
            DoubleAnimationUsingKeyFrames filterAlphaAnimation = new DoubleAnimationUsingKeyFrames();
            filterAlphaAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.3), Value = 0.0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } });
            filterAlphaAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.6), Value = 1.0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } });
            Storyboard.SetTarget(filterAlphaAnimation, filter);
            Storyboard.SetTargetProperty(filterAlphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(filterAlphaAnimation);

            animation.Completed += (sender, evt) => {
                galleryView.Visibility = Visibility.Collapsed;
                filter.OnFilterUIAdded();
            };

            animation.Begin();
        }

    }
}
