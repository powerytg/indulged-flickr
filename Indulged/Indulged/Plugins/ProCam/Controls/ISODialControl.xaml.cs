using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using Indulged.Plugins.ProCam.Utils;

namespace Indulged.Plugins.ProCam.Controls
{
    public partial class ISODialControl : UserControl
    {
        private double currentY;
        private double rotationStep = 20;
        private double anglePerStep;
        private double previousStepY = 0;

        private int baseIndex;

        public bool IsInfiniteScrollingEnabled { get; set; }

        public Int32 CurrentValue { get; set; }

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
                anglePerStep = 360 / SupportedValues.Count;
                baseIndex = 0;
                CurrentIndex = baseIndex;

                Label.Text = SupportedValues[CurrentIndex].ToISOString();
            }
        }

        public int CurrentIndex { get; set; }

        // Events
        public EventHandler DragBegin;
        public EventHandler DragEnd;
        public EventHandler ValueChanged;

        // Constructor
        public ISODialControl()
        {
            InitializeComponent();

            // Initialize properties
            IsInfiniteScrollingEnabled = true;

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
                Label.Text = CurrentValue.ToISOString();

                // Show ticks
                var percent = CurrentIndex / ((float)SupportedValues.Count - 1);
                //ISOTickView.Opacity = 0.3 + 0.7 * percent;
                if (ValueChanged != null)
                {
                    ValueChanged(this, null);
                }
            }
        }

        
    }
}
