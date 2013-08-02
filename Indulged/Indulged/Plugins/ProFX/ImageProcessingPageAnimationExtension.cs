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
        private void ShowSeconderyViewWithContent(FrameworkElement contentElement, double height)
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            SeconderyContentView.Children.Clear();
            SeconderyContentView.Children.Add(contentElement);

            CompositeTransform ct = (CompositeTransform)SeconderyPage.RenderTransform;
            ct.TranslateY = h;
            SeconderyPage.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.6));

            // Page container animation
            DoubleAnimation containerAnimation = new DoubleAnimation();
            containerAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            containerAnimation.To = height;
            containerAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(containerAnimation, PageContainer);
            Storyboard.SetTargetProperty(containerAnimation, new PropertyPath("(UIElement.Height)"));
            animation.Children.Add(containerAnimation);

            DoubleAnimationUsingKeyFrames editorAnimation = new DoubleAnimationUsingKeyFrames();
            editorAnimation.Duration = animation.Duration;
            editorAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.2), Value = -w, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            Storyboard.SetTarget(editorAnimation, EditorPage);
            Storyboard.SetTargetProperty(editorAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(editorAnimation);

            DoubleAnimationUsingKeyFrames galleryAnimation = new DoubleAnimationUsingKeyFrames();
            galleryAnimation.Duration = animation.Duration;
            galleryAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.4), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } });
            Storyboard.SetTarget(galleryAnimation, SeconderyPage);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);

            animation.Begin();
        }

        private void ShowFilterListView()
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            CompositeTransform ct = (CompositeTransform)EditorPage.RenderTransform;
            ct.TranslateX = -w;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            // Page container animation
            DoubleAnimation containerAnimation = new DoubleAnimation();
            containerAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            containerAnimation.To = (ImageProcessingPage.AppliedFilters.Count + 1) * AddFilterButton.ActualHeight;
            containerAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(containerAnimation, PageContainer);
            Storyboard.SetTargetProperty(containerAnimation, new PropertyPath("(UIElement.Height)"));
            animation.Children.Add(containerAnimation);

            DoubleAnimation seconderyAnimation = new DoubleAnimation();
            seconderyAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            seconderyAnimation.To = h;
            Storyboard.SetTarget(seconderyAnimation, SeconderyPage);
            Storyboard.SetTargetProperty(seconderyAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(seconderyAnimation);

            DoubleAnimationUsingKeyFrames editorAnimation = new DoubleAnimationUsingKeyFrames();
            editorAnimation.Duration = animation.Duration;
            editorAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.4), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            Storyboard.SetTarget(editorAnimation, EditorPage);
            Storyboard.SetTargetProperty(editorAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(editorAnimation);

            animation.Begin();
        }

        private void SwitchSeconderyViewWithContent(FrameworkElement newContentView, double height)
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            FrameworkElement oldContentView = (FrameworkElement)SeconderyContentView.Children[0];

            newContentView.Opacity = 0;
            SeconderyContentView.Children.Add(newContentView);

            CompositeTransform ct = (CompositeTransform)newContentView.RenderTransform;
            ct.TranslateX = w;
            newContentView.Opacity = 1;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Page container animation
            DoubleAnimation containerAnimation = new DoubleAnimation();
            containerAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            containerAnimation.To = height;
            containerAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(containerAnimation, PageContainer);
            Storyboard.SetTargetProperty(containerAnimation, new PropertyPath("(UIElement.Height)"));
            animation.Children.Add(containerAnimation);

            DoubleAnimation oldAnimation = new DoubleAnimation();
            oldAnimation.Duration = animation.Duration;
            oldAnimation.To = height;
            Storyboard.SetTarget(oldAnimation, oldContentView);
            Storyboard.SetTargetProperty(oldAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(oldAnimation);

            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = animation.Duration;
            galleryAnimation.To = 0;
            Storyboard.SetTarget(galleryAnimation, newContentView);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(galleryAnimation);
            animation.Completed += (sender, e) => {
                SeconderyContentView.Children.Remove(oldContentView);
            };

            animation.Begin();
        }

        private void ShowUploaderPage()
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            UploaderPage.Opacity = 0;

            CompositeTransform ct = (CompositeTransform)UploaderPage.RenderTransform;
            ct.TranslateX = w;
            UploaderPage.Opacity = 1;
            UploaderPage.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Processor page animation
            DoubleAnimation processorPageAnimation = new DoubleAnimation();
            processorPageAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            processorPageAnimation.To = -w;
            processorPageAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(processorPageAnimation, ProcessorPage);
            Storyboard.SetTargetProperty(processorPageAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(processorPageAnimation);

            DoubleAnimation uploaderPageAnimation = new DoubleAnimation();
            uploaderPageAnimation.Duration = animation.Duration;
            uploaderPageAnimation.To = 0;
            Storyboard.SetTarget(uploaderPageAnimation, UploaderPage);
            Storyboard.SetTargetProperty(uploaderPageAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(uploaderPageAnimation);

            animation.Completed += (sender, e) => {
                ProcessorPage.Visibility = Visibility.Collapsed;
            };

            animation.Begin();
        }

        private void ShowProcessorPage()
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            ProcessorPage.Opacity = 0;

            CompositeTransform ct = (CompositeTransform)ProcessorPage.RenderTransform;
            ct.TranslateX = -w;
            ProcessorPage.Opacity = 1;
            ProcessorPage.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Processor page animation
            DoubleAnimation processorPageAnimation = new DoubleAnimation();
            processorPageAnimation.Duration = animation.Duration;
            processorPageAnimation.To = 0;
            processorPageAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(processorPageAnimation, ProcessorPage);
            Storyboard.SetTargetProperty(processorPageAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(processorPageAnimation);

            DoubleAnimation uploaderPageAnimation = new DoubleAnimation();
            uploaderPageAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            uploaderPageAnimation.To = w;
            Storyboard.SetTarget(uploaderPageAnimation, UploaderPage);
            Storyboard.SetTargetProperty(uploaderPageAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(uploaderPageAnimation);

            animation.Completed += (sender, e) =>
            {
                UploaderPage.Visibility = Visibility.Collapsed;
            };

            animation.Begin();
        }

    }
}
