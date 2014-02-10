using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace Indulged.Plugins.ProCam.Controls
{
    public partial class HUDSwitchControl : UserControl
    {

        // Events
        public EventHandler HUDStateChanged;

        private BitmapImage OnLabelImage = new BitmapImage(new Uri("/Assets/ProCam/OSDOn.png", UriKind.Relative));
        private BitmapImage OffLabelImage = new BitmapImage(new Uri("/Assets/ProCam/OSDOff.png", UriKind.Relative));

        private bool _isOn = true;
        public bool IsOn
        {
            get
            {
                return _isOn; 
            }

            set
            {
                _isOn = value;
                if (_isOn)
                {
                    Label.Source = OnLabelImage;
                    ArrowTransform.Angle = 0;
                }
                else
                {
                    Label.Source = OffLabelImage;
                    ArrowTransform.Angle = 90;
                }
            }
        }

        // Constructor
        public HUDSwitchControl()
        {
            InitializeComponent();
        }

        private void OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _isOn = !_isOn;
            double targetArrowAngle;

            if (_isOn)
            {
                Label.Source = OnLabelImage;
                targetArrowAngle = 0;
            }
            else
            {
                Label.Source = OffLabelImage;
                targetArrowAngle = 90;
            }

            if (HUDStateChanged != null)
            {
                HUDStateChanged(this, null);
            }

            Storyboard storyboard = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            storyboard.Duration = duration;

            DoubleAnimation panelAnimation = new DoubleAnimation();
            panelAnimation.Duration = duration;
            panelAnimation.To = targetArrowAngle;
            panelAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(panelAnimation, Arrow);
            Storyboard.SetTargetProperty(panelAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
            storyboard.Children.Add(panelAnimation);

            storyboard.Begin();
        }
    }
}
