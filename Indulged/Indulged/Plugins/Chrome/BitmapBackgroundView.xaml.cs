﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime; 

using Indulged.Plugins.Dashboard;
using Indulged.Plugins.Dashboard.Events;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Indulged.API.Cinderella.Models;
using Windows.Storage.Streams;

namespace Indulged.Plugins.Chrome
{
    public partial class BitmapBackgroundView : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(WriteableBitmap), typeof(BitmapBackgroundView), new PropertyMetadata(OnSourcePropertyChanged));

        public WriteableBitmap Source
        {
            get
            {
                return (WriteableBitmap)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        public static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((BitmapBackgroundView)sender).OnSourceChanged();
        }

        protected virtual void OnSourceChanged()
        {
            // Fade out the old image
            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation fadeOutAnimation = new DoubleAnimation();
            animation.Children.Add(fadeOutAnimation);
            fadeOutAnimation.Duration = animation.Duration;
            fadeOutAnimation.To = 0;
            Storyboard.SetTarget(fadeOutAnimation, BackgroundImage);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));

            animation.Completed += FadeOutAnimationCompleted;
            animation.Begin();
        }

        // Constructor
        public BitmapBackgroundView()
        {
            InitializeComponent();
        }

        private async void ApplyFilterToBackgroundImageAsync()
        {
            MemoryStream bitmapStream = new MemoryStream();
            Source.SaveJpeg(bitmapStream, Source.PixelWidth, Source.PixelHeight, 0, 50);
            IBuffer bmpBuffer = bitmapStream.GetWindowsRuntimeBuffer();

            // Output buffer
            WriteableBitmap outputImage = new WriteableBitmap(Source.PixelWidth, Source.PixelHeight);

            using (EditingSession editsession = new EditingSession(bmpBuffer))
            {
                // First add an antique effect 
                editsession.AddFilter(FilterFactory.CreateBlurFilter(BlurLevel.Blur6));

                // Finally, execute the filtering and render to a bitmap
                await editsession.RenderToBitmapAsync(outputImage.AsBitmap());
                outputImage.Invalidate();
                FadeInNewImage(outputImage);
            }
        }


        private void FadeOutAnimationCompleted(object sender, EventArgs e)
        {
            ApplyFilterToBackgroundImageAsync();
        }

        private void FadeInNewImage(WriteableBitmap newImage)
        {
            // Fade in the new image
            BackgroundImage.Source = newImage;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation fadeInAnimation = new DoubleAnimation();
            animation.Children.Add(fadeInAnimation);
            fadeInAnimation.Duration = animation.Duration;
            fadeInAnimation.To = 1;
            Storyboard.SetTarget(fadeInAnimation, BackgroundImage);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
            animation.Begin();

        }
         
    }
}
