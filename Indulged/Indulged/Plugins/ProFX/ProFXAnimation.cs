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

        private void ShowActiveFilterList()
        {
            ActiveFilterView.ShowLoadingView();

            TranslateTransform tf = (TranslateTransform)ActiveFilterView.RenderTransform;
            tf.Y = ActiveFilterView.Height;
            ActiveFilterView.Visibility = Visibility.Visible;

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
            Storyboard.SetTarget(yAnimation, ActiveFilterView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            sb.Children.Add(yAnimation);

            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                // Update droplets
                ActiveFilterView.UpdateFilterEntries();
                ActiveFilterView.HideLoadingView();
            };
        }

        private void DismissActiveFilterList(bool shouldRestoreToolbars = true, Action action = null)
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
            Storyboard.SetTarget(yAnimation, ActiveFilterView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            sb.Children.Add(yAnimation);

            sb.Begin();
            sb.Completed += (sender, e) =>
            {
                ActiveFilterView.Visibility = Visibility.Collapsed;

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

        private void ShowUploaderView()
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            CompositeTransform ct = (CompositeTransform)UploaderPage.RenderTransform;
            ct.TranslateX = w;

            UploaderPage.Opacity = 0;
            UploaderPage.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Processor page X animation
            DoubleAnimation processorXAnimation = new DoubleAnimation();
            processorXAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            processorXAnimation.To = -w;
            processorXAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(processorXAnimation, ProcessorPage);
            Storyboard.SetTargetProperty(processorXAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(processorXAnimation);

            // Processor alpha animation
            DoubleAnimation processorAlphaAnimation = new DoubleAnimation();
            processorAlphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            processorAlphaAnimation.To = 0.0;
            processorAlphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(processorAlphaAnimation, ProcessorPage);
            Storyboard.SetTargetProperty(processorAlphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(processorAlphaAnimation);

            // Uploader page X animation
            DoubleAnimation uploaderXAnimation = new DoubleAnimation();
            uploaderXAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            uploaderXAnimation.To = 0.0;
            uploaderXAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(uploaderXAnimation, UploaderPage);
            Storyboard.SetTargetProperty(uploaderXAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(uploaderXAnimation);

            // Uploader alpha animation
            DoubleAnimation uploaderAlphaAnimation = new DoubleAnimation();
            uploaderAlphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            uploaderAlphaAnimation.To = 1.0;
            uploaderAlphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(uploaderAlphaAnimation, UploaderPage);
            Storyboard.SetTargetProperty(uploaderAlphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(uploaderAlphaAnimation);

            animation.Completed += (sender, e) =>
            {
                ProcessorPage.Visibility = Visibility.Collapsed;

                UploaderPage.OriginalImage = originalImage;
                UploaderPage.UploadButton.Play();
            };

            animation.Begin();
        }

        private void DismissUploaderView()
        {
            UploaderPage.UploadButton.Stop();

            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            CompositeTransform ct = (CompositeTransform)ProcessorPage.RenderTransform;
            ct.TranslateX = -w;

            ProcessorPage.Opacity = 0;
            ProcessorPage.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Processor page X animation
            DoubleAnimation processorXAnimation = new DoubleAnimation();
            processorXAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            processorXAnimation.To = 0;
            processorXAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(processorXAnimation, ProcessorPage);
            Storyboard.SetTargetProperty(processorXAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(processorXAnimation);

            // Processor alpha animation
            DoubleAnimation processorAlphaAnimation = new DoubleAnimation();
            processorAlphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            processorAlphaAnimation.To = 1.0;
            processorAlphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(processorAlphaAnimation, ProcessorPage);
            Storyboard.SetTargetProperty(processorAlphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(processorAlphaAnimation);

            // Uploader page X animation
            DoubleAnimation uploaderXAnimation = new DoubleAnimation();
            uploaderXAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            uploaderXAnimation.To = w;
            uploaderXAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(uploaderXAnimation, UploaderPage);
            Storyboard.SetTargetProperty(uploaderXAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(uploaderXAnimation);

            // Uploader alpha animation
            DoubleAnimation uploaderAlphaAnimation = new DoubleAnimation();
            uploaderAlphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            uploaderAlphaAnimation.To = 0.0;
            uploaderAlphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(uploaderAlphaAnimation, UploaderPage);
            Storyboard.SetTargetProperty(uploaderAlphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(uploaderAlphaAnimation);

            animation.Completed += (sender, e) =>
            {
                UploaderPage.Visibility = Visibility.Collapsed;
            };

            animation.Begin();
        }
    }
}
