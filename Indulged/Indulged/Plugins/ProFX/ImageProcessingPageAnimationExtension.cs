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
            galleryAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, galleryView);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);

            // Alpha animation
            DoubleAnimation alphaAnimation = new DoubleAnimation();
            alphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            alphaAnimation.To = 1.0;
            alphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(alphaAnimation, galleryView);
            Storyboard.SetTargetProperty(alphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(alphaAnimation);


            animation.Begin();
        }

        private void DismissFilterListView()
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            galleryAnimation.To = galleryView.Height;
            galleryAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, galleryView);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);

            // Alpha animation
            DoubleAnimation alphaAnimation = new DoubleAnimation();
            alphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            alphaAnimation.To = 0.0;
            alphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(alphaAnimation, galleryView);
            Storyboard.SetTargetProperty(alphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(alphaAnimation);

            animation.Completed += (sender, evt) =>
            {
                galleryView.Visibility = Visibility.Collapsed;
            };

            animation.Begin();
        }

        private void ShowFilterControlView(FilterBase filter)
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            // Add the request filter view to screen
            if (filter.hasEditorUI)
            {
                filter.VerticalAlignment = VerticalAlignment.Bottom;
                filter.Margin = new Thickness(0, 0, 0, BottomPanel.Height);
                filter.Visibility = Visibility.Collapsed;
            }

            filter.OriginalImage = originalBitmap;
            filter.CurrentImage = currentPreviewBitmap;
            filter.OriginalPreviewImage = originalPreviewBitmap;
            filter.Buffer = previewBuffer;

            if (filter.hasEditorUI)
            {
                ProcessorPage.Children.Add(filter);

                CompositeTransform ct = (CompositeTransform)filter.RenderTransform;
                ct.TranslateY = filter.Height;

                filter.Opacity = 0;
                filter.Visibility = Visibility.Visible;
            }
            
            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.6));

            // Gallery Y animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            galleryAnimation.To = galleryView.Height;
            galleryAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, galleryView);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);

            // Gallery Alpha animation
            DoubleAnimation alphaAnimation = new DoubleAnimation();
            alphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            alphaAnimation.To = 0.0;
            alphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(alphaAnimation, galleryView);
            Storyboard.SetTargetProperty(alphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(alphaAnimation);

            // Filter view Y animation
            if (filter.hasEditorUI)
            {
                DoubleAnimationUsingKeyFrames filterYAnimation = new DoubleAnimationUsingKeyFrames();
                filterYAnimation.Duration = animation.Duration;
                filterYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.3), Value = filter.Height, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                filterYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.6), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                Storyboard.SetTarget(filterYAnimation, filter);
                Storyboard.SetTargetProperty(filterYAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
                animation.Children.Add(filterYAnimation);

                // Filter view alpha animation
                DoubleAnimationUsingKeyFrames filterAlphaAnimation = new DoubleAnimationUsingKeyFrames();
                filterAlphaAnimation.Duration = animation.Duration;
                filterAlphaAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.3), Value = 0.0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                filterAlphaAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.6), Value = 1.0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
                Storyboard.SetTarget(filterAlphaAnimation, filter);
                Storyboard.SetTargetProperty(filterAlphaAnimation, new PropertyPath("Opacity"));
                animation.Children.Add(filterAlphaAnimation);
            }
            

            animation.Completed += (sender, evt) => {
                galleryView.Visibility = Visibility.Collapsed;
                filter.OnFilterUIAdded();
            };

            animation.Begin();
        }

        private void DismissFilterControlView(FilterBase filter)
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            if (!filter.hasEditorUI)
            {
                filter.OnFilterUIDismissed();
                return;
            }

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.6));

            // Filter Y animation
            DoubleAnimation filterYAnimation = new DoubleAnimation();
            filterYAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            filterYAnimation.To = galleryView.Height;
            filterYAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(filterYAnimation, filter);
            Storyboard.SetTargetProperty(filterYAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(filterYAnimation);

            // Gallery Alpha animation
            DoubleAnimation alphaAnimation = new DoubleAnimation();
            alphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            alphaAnimation.To = 0.0;
            alphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(alphaAnimation, filter);
            Storyboard.SetTargetProperty(alphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(alphaAnimation);
            
            animation.Completed += (sender, evt) =>
            {
                filter.OnFilterUIDismissed();
                ProcessorPage.Children.Remove(filter);
            };

            animation.Begin();
        }

        private void ShowSettingsView()
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            CompositeTransform ct = (CompositeTransform)settingsView.RenderTransform;
            ct.TranslateY = settingsView.Height;

            settingsView.Opacity = 0;
            settingsView.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            yAnimation.To = 0.0;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, settingsView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(yAnimation);

            // Alpha animation
            DoubleAnimation alphaAnimation = new DoubleAnimation();
            alphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            alphaAnimation.To = 1.0;
            alphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(alphaAnimation, settingsView);
            Storyboard.SetTargetProperty(alphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(alphaAnimation);


            animation.Begin();
        }

        private void DismissSettingsView()
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            yAnimation.To = settingsView.Height;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, settingsView);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(yAnimation);

            // Alpha animation
            DoubleAnimation alphaAnimation = new DoubleAnimation();
            alphaAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            alphaAnimation.To = 0.0;
            alphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(alphaAnimation, settingsView);
            Storyboard.SetTargetProperty(alphaAnimation, new PropertyPath("Opacity"));
            animation.Children.Add(alphaAnimation);

            animation.Completed += (sender, evt) =>
            {
                settingsView.Visibility = Visibility.Collapsed;
            };

            animation.Begin();
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

            animation.Completed += (sender, e) => {
                ProcessorPage.Visibility = Visibility.Collapsed;

                UploaderPage.OriginalImage = originalImage;
            };

            animation.Begin();
        }

        private void DismissUploaderView()
        {
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
