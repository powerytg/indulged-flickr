using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Controls;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage
    {
        private void ShowLoadingView()
        {
            HideUIChrome();
            LoadingView.Visibility = Visibility.Visible;
        }

        private void HideLoadingView()
        {
            LoadingView.Visibility = Visibility.Collapsed;
            ShowUIChrome();
        }

        private void ShowUIChrome()
        {
            LayoutRoot.IsHitTestVisible = true;
            Chrome.Opacity = 1;
        }

        private void HideUIChrome()
        {
            LayoutRoot.IsHitTestVisible = false;
            Chrome.Opacity = 0;
        }

        private void OnPageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            CorrectViewfinderOrientation(e.Orientation);
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
            Storyboard storyboard = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.5));
            storyboard.Duration = duration;

            if (LandscapeShutterButton.Visibility != Visibility.Visible)
            {
                TranslateTransform tf = (TranslateTransform)LandscapeShutterButton.RenderTransform;
                tf.X = 200;
                LandscapeShutterButton.Visibility = Visibility.Visible;

                DoubleAnimation shutterAnimation = new DoubleAnimation();
                shutterAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
                shutterAnimation.To = 0;
                shutterAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                Storyboard.SetTarget(shutterAnimation, LandscapeShutterButton);
                Storyboard.SetTargetProperty(shutterAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                storyboard.Children.Add(shutterAnimation);
            }
            

            if (SupportedCameras.Count > 1)
            {
                if (CameraSwitchButton.Visibility != Visibility.Visible)
                {
                    TranslateTransform switchTF = (TranslateTransform)CameraSwitchButton.RenderTransform;
                    switchTF.X = 200;
                    CameraSwitchButton.Visibility = Visibility.Visible;

                    DoubleAnimation switchAnimation = new DoubleAnimation();
                    switchAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                    switchAnimation.To = 0;
                    switchAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                    Storyboard.SetTarget(switchAnimation, CameraSwitchButton);
                    Storyboard.SetTargetProperty(switchAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                    storyboard.Children.Add(switchAnimation);
                }
            }


            if (storyboard.Children.Count != 0)
            {
                storyboard.Begin();
            }
        }


    }
}
