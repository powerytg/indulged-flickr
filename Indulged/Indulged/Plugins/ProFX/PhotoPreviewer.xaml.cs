using System;
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
using System.Windows.Shapes;
using System.Windows.Input;
using Indulged.Plugins.ProFX.Events;

namespace Indulged.Plugins.ProFX
{
    public partial class PhotoPreviewer : UserControl
    {
        public static EventHandler RequestChangeAspectRatioToFit;
        public static EventHandler RequestChangeAspectRatioToFill;


        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(PhotoPreviewer), new PropertyMetadata(OnSourcePropertyChanged));

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
            ((PhotoPreviewer)sender).OnSourceChanged();
        }

        protected virtual void OnSourceChanged()
        {
            PhotoView.Source = Source;
        }

        // Constructor
        public PhotoPreviewer()
        {
            InitializeComponent();

            // Events
            Handle.ManipulationDelta += OnHandleDrag;
            Viewfinder.ManipulationDelta += OnViewfinderDrag;

            RequestChangeAspectRatioToFill += OnRequestAspectFill;
            RequestChangeAspectRatioToFit += OnRequestAspectFit;
        }

        // Crop area
        private Rect cropRect = new Rect();

        public void ShowCropFinder()
        {
            Curtain.Visibility = Visibility.Visible;

            WriteableBitmap bmp = (WriteableBitmap)Source;

            if (cropRect.Width == 0 || cropRect.Height == 0)
            {
                Viewfinder.Width = bmp.PixelWidth / 2;
                Viewfinder.Height = bmp.PixelHeight / 2;
            }
            else
            {
                Viewfinder.Width = cropRect.Width;
                Viewfinder.Height = cropRect.Height;
            }

            Viewfinder.SetValue(Canvas.LeftProperty, CropCanvas.ActualWidth / 2 - Viewfinder.Width / 2);
            Viewfinder.SetValue(Canvas.TopProperty, CropCanvas.ActualHeight / 2 - Viewfinder.Height / 2);
            Viewfinder.Visibility = Visibility.Visible;

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
            double viewfinderLeft = (double)Viewfinder.GetValue(Canvas.LeftProperty);
            double viewfinderTop = (double)Viewfinder.GetValue(Canvas.TopProperty);

            var evt = new CropAreaChangedEventArgs();
            evt.X = viewfinderLeft;
            evt.Y = viewfinderTop;
            evt.Width = Viewfinder.Width;
            evt.Height = Viewfinder.Height;

            ImageProcessingPage.CropAreaChanged(this, evt);
        }

        public void DismissCropFinder()
        {
            Curtain.Visibility = Visibility.Collapsed;
            Viewfinder.Visibility = Visibility.Collapsed;

            // Handles
            Handle.Visibility = Visibility.Collapsed;
        }

        private void PositionHandlesAroundViewfinder()
        {
            double viewfinderLeft = (double)Viewfinder.GetValue(Canvas.LeftProperty);
            double viewfinderTop = (double)Viewfinder.GetValue(Canvas.TopProperty);

            Handle.SetValue(Canvas.LeftProperty, viewfinderLeft + Viewfinder.Width - Handle.Width / 2);
            Handle.SetValue(Canvas.TopProperty, viewfinderTop + Viewfinder.Height - Handle.Height / 2);            
        }

        private void OnHandleDrag(object sender, ManipulationDeltaEventArgs e)
        {
            double left = (double)Handle.GetValue(Canvas.LeftProperty);
            double top = (double)Handle.GetValue(Canvas.TopProperty);

            double viewfinderLeft = (double)Viewfinder.GetValue(Canvas.LeftProperty);
            double viewfinderTop = (double)Viewfinder.GetValue(Canvas.TopProperty);

            double minWidth = 20;
            double minHeight = 20;
            double newWidth = Viewfinder.Width + e.DeltaManipulation.Translation.X;
            double newHeight = Viewfinder.Height + e.DeltaManipulation.Translation.Y;

            left += e.DeltaManipulation.Translation.X;
            if (Math.Abs(left - viewfinderLeft) <= minWidth)
            {
                left = viewfinderLeft + minWidth;
                newWidth = minWidth;
            }
            
            if (left >= Curtain.ActualWidth - Handle.Width / 2)
            {
                left = Curtain.ActualWidth - Handle.Width / 2;
                newWidth = Math.Min(Viewfinder.Width, newWidth) ;
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
               newHeight = Math.Min(Viewfinder.Height, newHeight);
            }

            Handle.SetValue(Canvas.LeftProperty, left);
            Handle.SetValue(Canvas.TopProperty, top);

            Viewfinder.Width = newWidth;
            Viewfinder.Height = newHeight;

            // Events
            BroadcastCropAreaChangeEvent();
        }

        private void OnViewfinderDrag(object sender, ManipulationDeltaEventArgs e)
        {
            double viewfinderLeft = (double)Viewfinder.GetValue(Canvas.LeftProperty);
            double viewfinderTop = (double)Viewfinder.GetValue(Canvas.TopProperty);

            double newViewfinderLeft = viewfinderLeft + e.DeltaManipulation.Translation.X;
            double newViewfinderTop = viewfinderTop +  e.DeltaManipulation.Translation.Y;
            if (newViewfinderLeft < 0)
                newViewfinderLeft = 0;

            if (newViewfinderLeft > Curtain.ActualWidth - Viewfinder.Width)
                newViewfinderLeft = Curtain.ActualWidth - Viewfinder.Width;

            if (newViewfinderTop < 0)
                newViewfinderTop = 0;

            if (newViewfinderTop > Curtain.ActualHeight - Viewfinder.Height)
                newViewfinderTop = Curtain.ActualHeight - Viewfinder.Height;

            Viewfinder.SetValue(Canvas.LeftProperty, newViewfinderLeft);
            Viewfinder.SetValue(Canvas.TopProperty, newViewfinderTop);

            // Snap handle
            PositionHandlesAroundViewfinder();

            // Events
            BroadcastCropAreaChangeEvent();
        }

        private void OnRequestAspectFill(object sender, EventArgs e)
        {
            PhotoView.Stretch = Stretch.UniformToFill;
        }

        private void OnRequestAspectFit(object sender, EventArgs e)
        {
            PhotoView.Stretch = Stretch.Uniform;
        }
    }
}
