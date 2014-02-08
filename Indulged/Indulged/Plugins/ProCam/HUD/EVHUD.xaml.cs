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
using System.Windows;
using System.Windows.Media.Animation;

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
                    evLabel.FontSize = 36;
                    evLabel.FontWeight = FontWeights.Medium;
                    evLabel.Text = ev.ToString();

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
            TextBlock firstItem = ValuePanel.Children[0] as TextBlock;
            double itemHeight = firstItem.ActualHeight;
            double targetY = -itemHeight * (index - baseIndex);
            
            Storyboard storyboard = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.2));
            storyboard.Duration = duration;

            DoubleAnimation panelAnimation = new DoubleAnimation();
            panelAnimation.Duration = duration;
            panelAnimation.To = targetY;
            panelAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(panelAnimation, ValuePanel);
            Storyboard.SetTargetProperty(panelAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            storyboard.Children.Add(panelAnimation);

            storyboard.Begin();
            storyboard.Completed += (sender, e) =>
            {
                
            };
        }
    }
}
