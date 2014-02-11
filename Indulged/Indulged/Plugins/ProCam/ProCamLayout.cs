using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage
    {
        private void OnPageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            DismissOSD();

            if (e.Orientation == PageOrientation.LandscapeLeft || e.Orientation == PageOrientation.LandscapeRight)
            {
                LayoutInLandscapeMode();
            }
            else
            {
                LayoutInPortraitMode();
            }
        }

        private void LayoutInLandscapeMode()
        {
            LandscapeShutterButton.Visibility = Visibility.Visible;
            PortraitShutterButton.Visibility = Visibility.Collapsed;

            TopToolbarBackground.Opacity = 0;
            BottomToolbarBackground.Visibility = Visibility.Collapsed;
            
            EVDialer.HorizontalAlignment = HorizontalAlignment.Left;
            EVDialer.Margin = new Thickness(200, 0, 0, 20);
            EVDialer.LayoutInLandscapeMode();

            CameraSwitchButton.Margin = new Thickness(0, 0, 180, 150);
            CameraSwitchButton.LayoutInLandscapeMode();
        }

        private void LayoutInPortraitMode()
        {
            LandscapeShutterButton.Visibility = Visibility.Collapsed;
            PortraitShutterButton.Visibility = Visibility.Visible;

            TopToolbarBackground.Opacity = 0.6;
            BottomToolbarBackground.Visibility = Visibility.Visible;

            EVDialer.HorizontalAlignment = HorizontalAlignment.Right;
            EVDialer.Margin = new Thickness(0, 0, 20, 20);
            EVDialer.LayoutInPortraitMode();

            CameraSwitchButton.Margin = new Thickness(20, 0, 15, 180);
            CameraSwitchButton.LayoutInPortraitMode();
        }

        private void HideLandscapeShutterButton()
        {
            CameraSwitchButton.Visibility = Visibility.Collapsed;
            LandscapeShutterButton.Visibility = Visibility.Collapsed;
        }

        private void ShowLandscapeShutterButton()
        {
            TranslateTransform tf = (TranslateTransform)LandscapeShutterButton.RenderTransform;
            tf.X = 200;
            LandscapeShutterButton.Visibility = Visibility.Visible;

            if (supportCameraSwitch)
            {
                TranslateTransform switchTF = (TranslateTransform)CameraSwitchButton.RenderTransform;
                tf.X = 300;
                CameraSwitchButton.Visibility = Visibility.Visible;
            }

            Storyboard storyboard = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.5));
            storyboard.Duration = duration;

            DoubleAnimation shutterAnimation = new DoubleAnimation();
            shutterAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            shutterAnimation.To = 0;
            shutterAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(shutterAnimation, LandscapeShutterButton);
            Storyboard.SetTargetProperty(shutterAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(shutterAnimation);

            if (supportCameraSwitch)
            {
                DoubleAnimation switchAnimation = new DoubleAnimation();
                switchAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                switchAnimation.To = 0;
                switchAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                Storyboard.SetTarget(switchAnimation, CameraSwitchButton);
                Storyboard.SetTargetProperty(switchAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                storyboard.Children.Add(switchAnimation);
            }

            storyboard.Begin();
        }


    }
}
