using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Indulged.Plugins.ProCam.HUD
{
    public partial class HUDContainer : UserControl
    {
        public FrameworkElement CurrentOSD;

        // Constructor
        public HUDContainer()
        {
            InitializeComponent();
            CurrentOSD = MainOSD;

            Width = CurrentOSD.Width;
            Height = CurrentOSD.Height;

            LayoutRoot.Clip = new RectangleGeometry { Rect = new Rect(0, 0, Width, Height) };
        }

        public void ShowOSD(FrameworkElement selectedOSD)
        {

            FrameworkElement previousOSD = CurrentOSD;
            CurrentOSD = selectedOSD;
            Width = selectedOSD.Width;
            Height = selectedOSD.Height;

            LayoutRoot.Clip = new RectangleGeometry { Rect = new Rect(0, 0, Width, Height) };

            if (this.Visibility == Visibility.Collapsed)
            {
                ReplaceOSDFromInvisible(previousOSD, CurrentOSD);
            }
            else
            {
                ReplaceOSD(previousOSD, CurrentOSD);
            }
        }

        private void ReplaceOSDFromInvisible(FrameworkElement previousOSD, FrameworkElement selectedOSD)
        {
            Duration containerAnimationDuration = new Duration(TimeSpan.FromSeconds(0.3));
            Duration totalAnimationDuration = new Duration(TimeSpan.FromSeconds(0.4));

            Storyboard storyboard = new Storyboard();
            storyboard.Duration = totalAnimationDuration;

            // OSD is out of screen
            PanelTransform.X = Width;
            this.Visibility = Visibility.Visible;

            DoubleAnimation containerAnimation = new DoubleAnimation();
            containerAnimation.Duration = containerAnimationDuration;
            containerAnimation.To = 0;
            containerAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(containerAnimation, this);
            Storyboard.SetTargetProperty(containerAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(containerAnimation);

            // No need to perform view switch animation. Direct hide the old osd and show the new one
            if (previousOSD != CurrentOSD)
            {
                previousOSD.Visibility = Visibility.Collapsed;
                TranslateTransform tf = CurrentOSD.RenderTransform as TranslateTransform;
                tf.X = CurrentOSD.Width;
                CurrentOSD.Visibility = Visibility.Visible;

                DoubleAnimation osdAnimation = new DoubleAnimation();
                osdAnimation.Duration = totalAnimationDuration;
                osdAnimation.To = 0;
                osdAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                Storyboard.SetTarget(osdAnimation, CurrentOSD);
                Storyboard.SetTargetProperty(osdAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                storyboard.Children.Add(osdAnimation);
            }
            else
            {
                CurrentOSD.Visibility = Visibility.Visible;
                TranslateTransform tf = CurrentOSD.RenderTransform as TranslateTransform;
                tf.X = 0;
            }

            storyboard.Begin();
        }

        private void ReplaceOSD(FrameworkElement previousOSD, FrameworkElement selectedOSD)
        {
            TranslateTransform tf = selectedOSD.RenderTransform as TranslateTransform;
            if (previousOSD == selectedOSD)
            {
                selectedOSD.Visibility = Visibility.Visible;
                tf.X = 0;

                return;
            }

            Storyboard storyboard = new Storyboard();
            storyboard.Duration = new Duration(TimeSpan.FromSeconds(0.4));

            tf.X = selectedOSD.Width;
            selectedOSD.Visibility = Visibility.Visible;

            // OSD container is visible
            DoubleAnimation oldAnimation = new DoubleAnimation();
            oldAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            oldAnimation.To = -previousOSD.Width;
            oldAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(oldAnimation, previousOSD);
            Storyboard.SetTargetProperty(oldAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(oldAnimation);

            DoubleAnimation newAnimation = new DoubleAnimation();
            newAnimation.Duration = storyboard.Duration;
            newAnimation.To = 0;
            newAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(newAnimation, selectedOSD);
            Storyboard.SetTargetProperty(newAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(newAnimation);

            storyboard.Completed += (sender, e) =>
            {
                previousOSD.Visibility = Visibility.Collapsed;
            };

            storyboard.Begin();
        }

        public void DismissOSD(Action completeAction = null)
        {
            Storyboard storyboard = new Storyboard();
            storyboard.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation containerAnimation = new DoubleAnimation();
            containerAnimation.Duration = storyboard.Duration;
            containerAnimation.To = Width;
            containerAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(containerAnimation, this);
            Storyboard.SetTargetProperty(containerAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(containerAnimation);

            storyboard.Completed += (sender, e) =>
            {
                this.Visibility = Visibility.Collapsed;

                if (completeAction != null)
                {
                    completeAction();
                }
            };

            storyboard.Begin();
        }
    }
}
