﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Indulged.Plugins.ProFX.Events;

namespace Indulged.Plugins.ProFX.Controls
{
    public partial class ViewFinderControl : UserControl
    {
         // Events
        public EventHandler<CropAreaChangedEventArgs> CropAreaChanged;

        // Crop area
        private Rect cropRect = new Rect();

        // Constructor
        public ViewFinderControl()
        {
            InitializeComponent();

            // Events
            Handle.ManipulationDelta += OnHandleDrag;
            HighlightBox.ManipulationDelta += OnViewfinderDrag;
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ViewFinderControl), new PropertyMetadata(OnSourcePropertyChanged));

        public ImageSource Source
        {
            get
            {
                return (ImageSource)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        public static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ViewFinderControl)sender).OnSourceChanged();
        }

        protected virtual void OnSourceChanged()
        {
            ImageView.Source = Source;
        }

        public static readonly DependencyProperty OriginalBitmapProperty = DependencyProperty.Register("OriginalBitmap", typeof(BitmapImage), typeof(ViewFinderControl), new PropertyMetadata(OnOriginalBitmapPropertyChanged));

        public BitmapImage OriginalBitmap
        {
            get
            {
                return (BitmapImage)GetValue(OriginalBitmapProperty);
            }
            set
            {
                SetValue(OriginalBitmapProperty, value);
            }
        }

        public static void OnOriginalBitmapPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ViewFinderControl)sender).OnOriginalBitmapChanged();
        }

        protected virtual void OnOriginalBitmapChanged()
        {
            double w = OriginalBitmap.PixelWidth;
            double h = OriginalBitmap.PixelHeight;
            InfoLabel.Text = w.ToString() + " by " + h.ToString() + " JPEG";
        }

        public void ShowInfoView()
        {
            InfoView.Visibility = Visibility.Visible;
        }

        public void HideInfoView()
        {
            InfoView.Visibility = Visibility.Collapsed;
        }

        public void ShowCropFinder()
        {
            InfoView.Visibility = Visibility.Collapsed;
            Curtain.Visibility = Visibility.Visible;

            WriteableBitmap bmp = (WriteableBitmap)Source;

            if (cropRect.Width == 0 || cropRect.Height == 0)
            {
                double w = bmp.PixelWidth / 2;
                double h = bmp.PixelHeight / 2;

                HighlightBox.Width = w;
                HighlightBox.Height = h;
            }
            else
            {
                HighlightBox.Width = cropRect.Width;
                HighlightBox.Height = cropRect.Height;
            }

            HighlightBox.SetValue(Canvas.LeftProperty, CropCanvas.ActualWidth / 2 - HighlightBox.Width / 2);
            HighlightBox.SetValue(Canvas.TopProperty, CropCanvas.ActualHeight / 2 - HighlightBox.Height / 2);
            HighlightBox.Visibility = Visibility.Visible;

            // Handles
            Handle.Visibility = Visibility.Visible;
            PositionHandlesAroundViewfinder();

            // Events
            BroadcastCropAreaChangeEvent();
        }

        public void ResetCropArea()
        {
            cropRect = new Rect();
        }

        private void BroadcastCropAreaChangeEvent()
        {
            double viewfinderLeft = (double)HighlightBox.GetValue(Canvas.LeftProperty);
            double viewfinderTop = (double)HighlightBox.GetValue(Canvas.TopProperty);

            if (CropAreaChanged != null)
            {
                var evt = new CropAreaChangedEventArgs();
                evt.X = viewfinderLeft;
                evt.Y = viewfinderTop;
                evt.Width = HighlightBox.Width;
                evt.Height = HighlightBox.Height;

                CropAreaChanged(this, evt);
            }
        }

        public void DismissCropFinder()
        {
            Curtain.Visibility = Visibility.Collapsed;
            HighlightBox.Visibility = Visibility.Collapsed;
            InfoView.Visibility = Visibility.Visible;

            // Handles
            Handle.Visibility = Visibility.Collapsed;
        }

        private void PositionHandlesAroundViewfinder()
        {
            double viewfinderLeft = (double)HighlightBox.GetValue(Canvas.LeftProperty);
            double viewfinderTop = (double)HighlightBox.GetValue(Canvas.TopProperty);

            Handle.SetValue(Canvas.LeftProperty, viewfinderLeft + HighlightBox.Width - Handle.Width / 2);
            Handle.SetValue(Canvas.TopProperty, viewfinderTop + HighlightBox.Height - Handle.Height / 2);
        }

        private void OnHandleDrag(object sender, ManipulationDeltaEventArgs e)
        {
            double left = (double)Handle.GetValue(Canvas.LeftProperty);
            double top = (double)Handle.GetValue(Canvas.TopProperty);

            double viewfinderLeft = (double)HighlightBox.GetValue(Canvas.LeftProperty);
            double viewfinderTop = (double)HighlightBox.GetValue(Canvas.TopProperty);

            double minWidth = 20;
            double minHeight = 20;
            double newWidth = HighlightBox.Width + e.DeltaManipulation.Translation.X;
            double newHeight = HighlightBox.Height + e.DeltaManipulation.Translation.Y;

            left += e.DeltaManipulation.Translation.X;
            if (Math.Abs(left - viewfinderLeft) <= minWidth)
            {
                left = viewfinderLeft + minWidth;
                newWidth = minWidth;
            }

            if (left >= Curtain.ActualWidth - Handle.Width / 2)
            {
                left = Curtain.ActualWidth - Handle.Width / 2;
                newWidth = Math.Min(HighlightBox.Width, newWidth);
            }

            top += e.DeltaManipulation.Translation.Y;
            if (Math.Abs(top - viewfinderTop) <= minHeight)
            {
                top = viewfinderTop + minHeight;
                newHeight = minHeight;
            }

            if (top >= Curtain.ActualHeight - Handle.Height / 2)
            {
                top = Curtain.ActualHeight - Handle.Height / 2;
                newHeight = Math.Min(HighlightBox.Height, newHeight);
            }

            Handle.SetValue(Canvas.LeftProperty, left);
            Handle.SetValue(Canvas.TopProperty, top);

            HighlightBox.Width = newWidth;
            HighlightBox.Height = newHeight;

            // Events
            BroadcastCropAreaChangeEvent();
        }

        private void OnViewfinderDrag(object sender, ManipulationDeltaEventArgs e)
        {
            double viewfinderLeft = (double)HighlightBox.GetValue(Canvas.LeftProperty);
            double viewfinderTop = (double)HighlightBox.GetValue(Canvas.TopProperty);

            double newViewfinderLeft = viewfinderLeft + e.DeltaManipulation.Translation.X;
            double newViewfinderTop = viewfinderTop + e.DeltaManipulation.Translation.Y;
            if (newViewfinderLeft < 0)
                newViewfinderLeft = 0;

            if (newViewfinderLeft > Curtain.ActualWidth - HighlightBox.Width)
                newViewfinderLeft = Curtain.ActualWidth - HighlightBox.Width;

            if (newViewfinderTop < 0)
                newViewfinderTop = 0;

            if (newViewfinderTop > Curtain.ActualHeight - HighlightBox.Height)
                newViewfinderTop = Curtain.ActualHeight - HighlightBox.Height;

            HighlightBox.SetValue(Canvas.LeftProperty, newViewfinderLeft);
            HighlightBox.SetValue(Canvas.TopProperty, newViewfinderTop);

            // Snap handle
            PositionHandlesAroundViewfinder();

            // Events
            BroadcastCropAreaChangeEvent();
        }
    }
}
