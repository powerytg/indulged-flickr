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
        private void ShowSeconderyViewWithContent(FrameworkElement contentElement)
        {
            double w = LayoutRoot.ActualWidth;

            SeconderyContentView.Children.Clear();
            SeconderyContentView.Children.Add(contentElement);
            //contentElement.Width = SeconderyContentView.ActualWidth;
            //contentElement.Height = SeconderyContentView.ActualHeight;

            CompositeTransform ct = (CompositeTransform)SeconderyPage.RenderTransform;
            ct.TranslateX = w;
            SeconderyPage.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation editorAnimation = new DoubleAnimation();
            editorAnimation.Duration = animation.Duration;
            editorAnimation.To = -w;
            Storyboard.SetTarget(editorAnimation, EditorPage);
            Storyboard.SetTargetProperty(editorAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(editorAnimation);

            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = animation.Duration;
            galleryAnimation.To = 0;
            Storyboard.SetTarget(galleryAnimation, SeconderyPage);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(galleryAnimation);

            animation.Begin();
        }

        private void ShowFilterListView()
        {
            double w = LayoutRoot.ActualWidth;
            CompositeTransform ct = (CompositeTransform)EditorPage.RenderTransform;
            ct.TranslateX = -w;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation editorAnimation = new DoubleAnimation();
            editorAnimation.Duration = animation.Duration;
            editorAnimation.To = 0;
            Storyboard.SetTarget(editorAnimation, EditorPage);
            Storyboard.SetTargetProperty(editorAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(editorAnimation);

            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = animation.Duration;
            galleryAnimation.To = w;
            Storyboard.SetTarget(galleryAnimation, SeconderyPage);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(galleryAnimation);

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
