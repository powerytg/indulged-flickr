using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Media;

namespace Indulged.Plugins.ProCam.Controls
{
    public partial class EVDialControl : UserControl
    {
        private double currentY;
        private double rotationStep = 40;
        private double anglePerStep;
        private double previousStepY = 0;

        private int baseIndex;

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
                baseIndex = SupportedValues.IndexOf(0);
                CurrentIndex = baseIndex;

                Label.Text = SupportedValues[CurrentIndex].ToString();
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

            GestureCaptureCanvas.ManipulationStarted += OnDialerDragStart;
            GestureCaptureCanvas.ManipulationCompleted += OnDialerDragEnd;
            GestureCaptureCanvas.ManipulationDelta += OnDialerDragDelta;
        }

        protected void OnDialerDragStart(object sender, ManipulationStartedEventArgs e)
        {
            BaseRotateTransform.CenterX = DialBaseView.ActualWidth / 2;
            BaseRotateTransform.CenterY = DialBaseView.ActualHeight / 2;

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
                        CurrentIndex = 0;
                    }
                }
                else
                {
                    // Moving down
                    CurrentIndex--;

                    if (CurrentIndex < 0)
                    {
                        CurrentIndex = SupportedValues.Count - 1;
                    }
                }

                BaseRotateTransform.Angle = (CurrentIndex - baseIndex) * anglePerStep;

                previousStepY = currentY;
                CurrentValue = SupportedValues[CurrentIndex];
                Label.Text = CurrentValue.ToString();

                if (ValueChanged != null)
                {
                    ValueChanged(this, null);
                }
            }
        }

        
    }
}
