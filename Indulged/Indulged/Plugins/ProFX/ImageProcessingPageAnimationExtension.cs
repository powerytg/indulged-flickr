﻿using System;
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


    }
}
