using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using Indulged.Plugins.ProCam.Utils;
using System.Windows;

namespace Indulged.Plugins.ProCam.Controls
{
    public partial class EVDialControl : UserControl
    {
        private double currentY;
        private double rotationStep = 20;
        private double anglePerStep;
        private double previousStepY = 0;

        private int baseIndex;

        public bool IsInfiniteScrollingEnabled { get; set; }

        public Int32 CurrentValue { get; set; }

        private List<Int32> positiveValues = new List<int>();
        private List<Int32> negativeValues = new List<int>();

        private BitmapImage baseTickImage;
        private List<BitmapImage> positiveTickImages = new List<BitmapImage>();
        private List<BitmapImage> negativeTickImages = new List<BitmapImage>();

        private BitmapImage arrowLeftImage = new BitmapImage(new Uri("/Assets/ProCam/ArrowLeft.png", UriKind.Relative));
        private BitmapImage arrowRightImage = new BitmapImage(new Uri("/Assets/ProCam/ArrowRight.png", UriKind.Relative));

        private List<Int32> _supportedValues;
        public List<Int32> SupportedValues
        {
            get
            {
                return _supportedValues;
            }
            set
            {
                _supportedValues = value;
                positiveValues.Clear();
                negativeValues.Clear();

                foreach (var ev in SupportedValues)
                {
                    if (ev > 0)
                    {
                        positiveValues.Add(ev);
                    }
                    else if (ev < 0)
                    {
                        negativeValues.Add(ev);
                    }
                }

                anglePerStep = 360 / SupportedValues.Count;
                baseIndex = SupportedValues.IndexOf(0);
                CurrentIndex = baseIndex;

                Label.Text = SupportedValues[CurrentIndex].ToEVString();
            }
        }

        public int CurrentIndex { get; set; }

        // Events
        public EventHandler DragBegin;
        public EventHandler DragEnd;
        public EventHandler ValueChanged;

        // Constructor
        public EVDialControl()
        {
            InitializeComponent();

            // Initialize properties
            IsInfiniteScrollingEnabled = true;

            // Initialize tick images
            baseTickImage = new BitmapImage(new Uri("/Assets/ProCam/EV/EV0.png", UriKind.Relative));
            positiveTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV1.png", UriKind.Relative)));
            positiveTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV2.png", UriKind.Relative)));
            positiveTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV3.png", UriKind.Relative)));
            positiveTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV4.png", UriKind.Relative)));
            positiveTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV5.png", UriKind.Relative)));
            positiveTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV6.png", UriKind.Relative)));
            positiveTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV7.png", UriKind.Relative)));

            negativeTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV-1.png", UriKind.Relative)));
            negativeTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV-2.png", UriKind.Relative)));
            negativeTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV-3.png", UriKind.Relative)));
            negativeTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV-4.png", UriKind.Relative)));
            negativeTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV-5.png", UriKind.Relative)));
            negativeTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV-6.png", UriKind.Relative)));
            negativeTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV-7.png", UriKind.Relative)));
            negativeTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV-8.png", UriKind.Relative)));
            negativeTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV-9.png", UriKind.Relative)));
            negativeTickImages.Add(new BitmapImage(new Uri("/Assets/ProCam/EV/EV-10.png", UriKind.Relative)));

            GestureCaptureCanvas.ManipulationStarted += OnDialerDragStart;
            GestureCaptureCanvas.ManipulationCompleted += OnDialerDragEnd;
            GestureCaptureCanvas.ManipulationDelta += OnDialerDragDelta;
        }

        protected void OnDialerDragStart(object sender, ManipulationStartedEventArgs e)
        {
            DialerTransform.CenterX = Dialer.ActualWidth / 2;
            DialerTransform.CenterY = Dialer.ActualHeight / 2;

            previousStepY = currentY;

            if (DragBegin != null)
            {
                DragBegin(this, null);
            }
        }

        protected void OnDialerDragEnd(object sender, ManipulationCompletedEventArgs e)
        {
            if (DragEnd != null)
            {
                DragEnd(this, null);
            }
        }

        protected void OnDialerDragDelta(object sender, ManipulationDeltaEventArgs e)
        {           
            currentY += e.DeltaManipulation.Translation.Y;
            double accumatedDist = currentY - previousStepY;

            if (Math.Abs(accumatedDist) >= rotationStep)
            {
                if (accumatedDist < 0)
                {
                    // Moving up
                    CurrentIndex++;

                    if (CurrentIndex >= SupportedValues.Count)
                    {
                        CurrentIndex = IsInfiniteScrollingEnabled ? 0 : SupportedValues.Count - 1;
                    }
                }
                else
                {
                    // Moving down
                    CurrentIndex--;

                    if (CurrentIndex < 0)
                    {
                        CurrentIndex = IsInfiniteScrollingEnabled ? SupportedValues.Count - 1 : 0;
                    }
                }

                DialerTransform.Angle = (CurrentIndex - baseIndex) * anglePerStep;

                previousStepY = currentY;
                CurrentValue = SupportedValues[CurrentIndex];
                Label.Text = CurrentValue.ToEVString();

                // Show ticks
                if (CurrentIndex == baseIndex)
                {
                    EVTickView.Source = baseTickImage;
                }
                else if (CurrentIndex > baseIndex)
                {
                    var percent = (CurrentIndex - baseIndex) / (float)positiveValues.Count;
                    int tick = (int)Math.Floor(percent * positiveTickImages.Count);
                    tick = Math.Min(tick, positiveTickImages.Count - 1);
                    tick = Math.Max(tick, 0);
                    EVTickView.Source = positiveTickImages[tick];
                }
                else
                {
                    var percent = (baseIndex - CurrentIndex) / (float)negativeValues.Count;
                    int tick = (int)Math.Floor(percent * negativeTickImages.Count);
                    tick = Math.Min(tick, negativeTickImages.Count - 1);
                    tick = Math.Max(tick, 0);
                    EVTickView.Source = negativeTickImages[tick];
                }

                if (ValueChanged != null)
                {
                    ValueChanged(this, null);
                }
            }
        }

        public void LayoutInLandscapeMode()
        {
            LayoutRoot.ColumnDefinitions.Clear();
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60, GridUnitType.Pixel) });

            Dialer.SetValue(Grid.ColumnProperty, 0);
            
            Label.SetValue(Grid.ColumnProperty, 1);
            Label.HorizontalAlignment = HorizontalAlignment.Left;
            Label.Margin = new Thickness(10, 0, 0, 0);

            Arrow.SetValue(Grid.ColumnProperty, 0);
            Arrow.Source = arrowRightImage;
            GestureCaptureCanvas.SetValue(Grid.ColumnProperty, 0);
        }

        public void LayoutInPortraitMode()
        {
            LayoutRoot.ColumnDefinitions.Clear();
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60, GridUnitType.Pixel) });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Label.SetValue(Grid.ColumnProperty, 0);
            Label.HorizontalAlignment = HorizontalAlignment.Right;
            Label.Margin = new Thickness(0, 0, 10, 0);

            Dialer.SetValue(Grid.ColumnProperty, 1);


            Arrow.SetValue(Grid.ColumnProperty, 1);
            Arrow.Source = arrowLeftImage;
            GestureCaptureCanvas.SetValue(Grid.ColumnProperty, 1);
        }
    }
}
