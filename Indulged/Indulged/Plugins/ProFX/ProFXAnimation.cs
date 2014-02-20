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
    public partial class ProFXPage
    {
        private void ShowFilterGallery()
        {
            // Update droplets
            FilterGalleryView.UpdateFilterDroplets();

            TranslateTransform tf = (TranslateTransform)FilterGalleryView.RenderTransform;
            tf.Y = FilterGalleryView.Height;
            FilterGalleryView.Visibility = Visibility.Visible;

            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation topAnimation = new DoubleAnimation();
            topAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            topAnimation.To = 0;
            topAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(topAnimation, CropToolbar);
            Storyboard.SetTargetProperty(topAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(topAnimation);

            DoubleAnimation bottomAnimation = new DoubleAnimation();
            bottomAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            bottomAnimation.To = 0;
            bottomAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(bottomAnimation, BottomToolbar);
            Storyboard.SetTargetProperty(bottomAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(bottomAnimation);

            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = sb.Duration;
            yAnimation.To = 0;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, FilterGalleryView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            sb.Children.Add(yAnimation);

            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                
            };
        }

        private void DismissFilterGallery(bool shouldRestoreToolbars = true, Action action = null)
        {
            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            if (shouldRestoreToolbars)
            {
                DoubleAnimation topAnimation = new DoubleAnimation();
                topAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                topAnimation.To = 1;
                topAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                Storyboard.SetTarget(topAnimation, CropToolbar);
                Storyboard.SetTargetProperty(topAnimation, new PropertyPath("UIElement.Opacity"));
                sb.Children.Add(topAnimation);
            }

            if (shouldRestoreToolbars)
            {
                DoubleAnimation bottomAnimation = new DoubleAnimation();
                bottomAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                bottomAnimation.To = 1;
                bottomAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                Storyboard.SetTarget(bottomAnimation, BottomToolbar);
                Storyboard.SetTargetProperty(bottomAnimation, new PropertyPath("UIElement.Opacity"));
                sb.Children.Add(bottomAnimation);
            }

            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = sb.Duration;
            yAnimation.To = FilterGalleryView.Height;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, FilterGalleryView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            sb.Children.Add(yAnimation);

            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                FilterGalleryView.Visibility = Visibility.Collapsed;

                if (action != null)
                {
                    action();
                }
            };
        }

        private void ShowFilterOSD(FilterBase filter)
        {
            FilterContainerView.Filter = filter;

            TranslateTransform tf = (TranslateTransform)FilterGalleryView.RenderTransform;
            tf.Y = FilterContainerView.Height;
            FilterContainerView.Visibility = Visibility.Visible;

            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = sb.Duration;
            yAnimation.To = 0;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, FilterContainerView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            sb.Children.Add(yAnimation);

            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                filter.OnFilterUIAdded();
            };
        }

        private void DismissFilterOSD(Action action = null)
        {
            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation topAnimation = new DoubleAnimation();
            topAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            topAnimation.To = 1;
            topAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(topAnimation, CropToolbar);
            Storyboard.SetTargetProperty(topAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(topAnimation);

            DoubleAnimation bottomAnimation = new DoubleAnimation();
            bottomAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            bottomAnimation.To = 1;
            bottomAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(bottomAnimation, BottomToolbar);
            Storyboard.SetTargetProperty(bottomAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(bottomAnimation);

            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = sb.Duration;
            yAnimation.To = FilterContainerView.Height;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, FilterContainerView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            sb.Children.Add(yAnimation);

            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                FilterContainerView.Visibility = Visibility.Collapsed;

                if (action != null)
                {
                    action();
                }
            };
        }

        private void ShowCropOSD(Action action = null)
        {
            TranslateTransform tf = (TranslateTransform)CropView.RenderTransform;
            tf.Y = CropView.Height;
            CropView.Visibility = Visibility.Visible;

            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation topAnimation = new DoubleAnimation();
            topAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            topAnimation.To = 0;
            topAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(topAnimation, CropToolbar);
            Storyboard.SetTargetProperty(topAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(topAnimation);

            DoubleAnimation bottomAnimation = new DoubleAnimation();
            bottomAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            bottomAnimation.To = 0;
            bottomAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(bottomAnimation, BottomToolbar);
            Storyboard.SetTargetProperty(bottomAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(bottomAnimation);

            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = sb.Duration;
            yAnimation.To = 0;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, CropView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            sb.Children.Add(yAnimation);

            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                if (action != null)
                {
                    action();
                }
            };
        }

        private void DismissCropOSD(Action action = null)
        {
            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation topAnimation = new DoubleAnimation();
            topAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            topAnimation.To = 1;
            topAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(topAnimation, CropToolbar);
            Storyboard.SetTargetProperty(topAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(topAnimation);

            DoubleAnimation bottomAnimation = new DoubleAnimation();
            bottomAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            bottomAnimation.To = 1;
            bottomAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(bottomAnimation, BottomToolbar);
            Storyboard.SetTargetProperty(bottomAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(bottomAnimation);

            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = sb.Duration;
            yAnimation.To = CropView.Height;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, CropView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            sb.Children.Add(yAnimation);

            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                CropView.Visibility = Visibility.Collapsed;

                if (action != null)
                {
                    action();
                }
            };
        }

        private void ShowRotationOSD(Action action = null)
        {
            TranslateTransform tf = (TranslateTransform)RotationView.RenderTransform;
            tf.Y = RotationView.Height;
            RotationView.Visibility = Visibility.Visible;

            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation topAnimation = new DoubleAnimation();
            topAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            topAnimation.To = 0;
            topAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(topAnimation, CropToolbar);
            Storyboard.SetTargetProperty(topAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(topAnimation);

            DoubleAnimation bottomAnimation = new DoubleAnimation();
            bottomAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            bottomAnimation.To = 0;
            bottomAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(bottomAnimation, BottomToolbar);
            Storyboard.SetTargetProperty(bottomAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(bottomAnimation);

            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = sb.Duration;
            yAnimation.To = 0;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, RotationView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            sb.Children.Add(yAnimation);

            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                if (action != null)
                {
                    action();
                }
            };
        }

        private void DismissRotationOSD(Action action = null)
        {
            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation topAnimation = new DoubleAnimation();
            topAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            topAnimation.To = 1;
            topAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(topAnimation, CropToolbar);
            Storyboard.SetTargetProperty(topAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(topAnimation);

            DoubleAnimation bottomAnimation = new DoubleAnimation();
            bottomAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            bottomAnimation.To = 1;
            bottomAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(bottomAnimation, BottomToolbar);
            Storyboard.SetTargetProperty(bottomAnimation, new PropertyPath("UIElement.Opacity"));
            sb.Children.Add(bottomAnimation);

            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = sb.Duration;
            yAnimation.To = RotationView.Height;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, RotationView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            sb.Children.Add(yAnimation);

            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                RotationView.Visibility = Visibility.Collapsed;

                if (action != null)
                {
                    action();
                }
            };
        }
    }
}
