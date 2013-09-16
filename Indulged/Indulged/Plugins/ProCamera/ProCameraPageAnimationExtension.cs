using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Indulged.Plugins.ProCamera
{
    public partial class ProCameraPage
    {
        private void PerformUIChromeAppearanceAnimation()
        {
            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.Duration = duration;

            DoubleAnimation bottomShadowAnimation = new DoubleAnimation();
            animation.Children.Add(bottomShadowAnimation);
            bottomShadowAnimation.Duration = duration;
            bottomShadowAnimation.To = 1;
            Storyboard.SetTarget(bottomShadowAnimation, BottomShadowImage);
            Storyboard.SetTargetProperty(bottomShadowAnimation, new PropertyPath("Opacity"));

            DoubleAnimation shutterButtonAnimation = new DoubleAnimation();
            animation.Children.Add(shutterButtonAnimation);
            shutterButtonAnimation.Duration = duration;
            shutterButtonAnimation.To = 1;
            Storyboard.SetTarget(shutterButtonAnimation, ShutterButton);
            Storyboard.SetTargetProperty(shutterButtonAnimation, new PropertyPath("Opacity"));

            DoubleAnimation toolbarAnimation = new DoubleAnimation();
            animation.Children.Add(toolbarAnimation);
            toolbarAnimation.Duration = duration;
            toolbarAnimation.To = 1;
            Storyboard.SetTarget(toolbarAnimation, TopToolbar);
            Storyboard.SetTargetProperty(toolbarAnimation, new PropertyPath("Opacity"));

            animation.Begin();
            animation.Completed += (sender, e) => {
                LayoutRoot.IsHitTestVisible = true;
            };
        }

        private Rectangle curtain;

        private void PerformCaptureAnimation()
        {
            Viewfinder.Opacity = 0;
            TopToolbar.Opacity = 0;
            BottomShadowImage.Opacity = 0;
            ShutterButton.Opacity = 0;

            LoadingView.Text = "Processing ...";
            LoadingView.Visibility = Visibility.Visible;

            curtain = new Rectangle();
            curtain.Fill = new SolidColorBrush(Colors.White);
            LayoutRoot.Children.Add(curtain);

            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.Duration = duration;

            DoubleAnimation curtainAnimation = new DoubleAnimation();
            animation.Children.Add(curtainAnimation);
            curtainAnimation.Duration = duration;
            curtainAnimation.To = 0;
            curtainAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(curtainAnimation, curtain);
            Storyboard.SetTargetProperty(curtainAnimation, new PropertyPath("Opacity"));

            animation.Completed += OnCaptureAnimationCompleted;
            animation.Begin();
        }

        private void OnCaptureAnimationCompleted(object sender, EventArgs e)
        {
            LayoutRoot.Children.Remove(curtain);
        }

        private Grid extendedPanel = null;
        private bool isAnimatingExtendedPanel;

        private void ShowOrHideExtendedPanelWithView(FrameworkElement contentView)
        {
            if (isAnimatingExtendedPanel)
                return;

            if (extendedPanel != null)
            {
                if (extendedPanel.Children.Contains(contentView))
                    HideExtendedPanel();
                else
                    ShowExtendedPanelWithView(contentView);
            }
            else
                ShowExtendedPanelWithView(contentView);
        }

        private void HideExtendedPanel()
        {
            isAnimatingExtendedPanel = true;
            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.Duration = duration;

            DoubleAnimation extAnimation = new DoubleAnimation();
            animation.Children.Add(extAnimation);
            extAnimation.Duration = duration;
            extAnimation.To = 0;
            extAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(extAnimation, extendedPanel);
            Storyboard.SetTargetProperty(extAnimation, new PropertyPath("Opacity"));

            animation.Completed += OnExtendedPanelHideAnimationComplete;
            animation.Begin();
        }

        private void OnExtendedPanelHideAnimationComplete(object sender, EventArgs e)
        {
            extendedPanel.Children.Clear();

            isAnimatingExtendedPanel = false;
            LayoutRoot.Children.Remove(extendedPanel);
            extendedPanel = null;
        }

        private void OnExtendedPanelShowAnimationComplete(object sender, EventArgs e)
        {
            isAnimatingExtendedPanel = false;
        }

        private void ShowExtendedPanelWithView(FrameworkElement contentView)
        {
            if (extendedPanel == null)
            {
                isAnimatingExtendedPanel = true;

                extendedPanel = new Grid();
                extendedPanel.VerticalAlignment = VerticalAlignment.Top;
                extendedPanel.Background = TopToolbar.Background;
                //extendedPanel.Height = 80;
                extendedPanel.Margin = new Thickness(0, TopToolbar.Height, 0, 0);
                extendedPanel.Children.Add(contentView);
                extendedPanel.Opacity = 0;
                LayoutRoot.Children.Add(extendedPanel);

                Storyboard animation = new Storyboard();
                Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
                animation.Duration = duration;

                DoubleAnimation extAnimation = new DoubleAnimation();
                animation.Children.Add(extAnimation);
                extAnimation.Duration = duration;
                extAnimation.To = 1;
                extAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
                Storyboard.SetTarget(extAnimation, extendedPanel);
                Storyboard.SetTargetProperty(extAnimation, new PropertyPath("Opacity"));

                animation.Completed += OnExtendedPanelShowAnimationComplete;
                animation.Begin();
            }
            else
            {
                if (!extendedPanel.Children.Contains(contentView))
                {
                    extendedPanel.Children.Clear();
                    extendedPanel.Children.Add(contentView);
                }
            }
        }

    }
}
