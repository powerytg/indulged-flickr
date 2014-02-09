using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Indulged.Plugins.ProCam.Utils;
using System.Diagnostics;

namespace Indulged.Plugins.ProCam.HUD
{
    public partial class EVHUD : UserControl
    {
        private int baseIndex;

        protected List<Int32> _supportedValues;
        public List<Int32> SupportedValues
        {
            get
            {
                return _supportedValues;
            }
            set
            {
                _supportedValues = value;
                
                baseIndex = _supportedValues.IndexOf(0);

                // Rebuild value panel
                ValuePanel.Children.Clear();

                foreach(var ev in SupportedValues)
                {
                    TextBlock evLabel = new TextBlock();
                    evLabel.Foreground = new SolidColorBrush(Colors.White);
                    evLabel.FontSize = 18;
                    evLabel.FontWeight = FontWeights.Medium;
                    evLabel.Text = ev.ToEVString();
                    evLabel.Margin = new Thickness(0, 0, 6, 0);
                    ValuePanel.Children.Add(evLabel);
                }

            }
        }

        protected Int32 _selectedValue;
        public Int32 SelectedValue
        {
            get
            {
                return _selectedValue;
            }
            set
            {
                _selectedValue = value;
                int index = SupportedValues.IndexOf(_selectedValue);

                // Shift the value list
                if (ValuePanel.Children.Count > 0)
                {
                    SmoothShiftValueListToPosition(index);
                }
            }
        }

        // Constructor
        public EVHUD()
        {
            InitializeComponent();
        }
        
        private void SmoothShiftValueListToPosition(int index)
        {
            // Preparation
            double itemHeight = (ValuePanel.Children[0] as FrameworkElement).ActualHeight;
            double targetY = itemHeight * index - Scroller.ActualHeight / 2 + itemHeight / 2;
            
            Scroller.ScrollToVerticalOffset(targetY);
        }

    }
}
