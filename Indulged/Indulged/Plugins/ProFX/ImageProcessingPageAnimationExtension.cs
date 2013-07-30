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

            ct = (CompositeTransform)SeconderyPage.RenderTransform;
            ct.TranslateY = h;

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

            DoubleAnimationUsingKeyFrames editorAnimation = new DoubleAnimationUsingKeyFrames();
            editorAnimation.Duration = animation.Duration;
            editorAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.4), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            Storyboard.SetTarget(editorAnimation, EditorPage);
            Storyboard.SetTargetProperty(editorAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(editorAnimation);

            animation.Begin();
        }

        private void SwitchSeconderyViewWithContent(FrameworkElement newContentView)
        {
            double w = LayoutRoot.ActualWidth;

            FrameworkElement oldContentView = (FrameworkElement)SeconderyContentView.Children[0];

            newContentView.Opacity = 0;
            SeconderyContentView.Children.Add(newContentView);
            //contentElement.Width = SeconderyContentView.ActualWidth;
            //contentElement.Height = SeconderyContentView.ActualHeight;

            CompositeTransform ct = (CompositeTransform)newContentView.RenderTransform;
            ct.TranslateX = w;
            newContentView.Opacity = 1;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation oldAnimation = new DoubleAnimation();
            oldAnimation.Duration = animation.Duration;
            oldAnimation.To = -w;
            Storyboard.SetTarget(oldAnimation, oldContentView);
            Storyboard.SetTargetProperty(oldAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
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
    }
}
