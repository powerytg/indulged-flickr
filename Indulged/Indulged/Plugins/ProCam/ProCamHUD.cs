﻿using Indulged.Plugins.ProCam.HUD;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage
    {
        private EVHUD evHUDView;
        private ISOHUD isoHUDView;

        private void CreateEVHUD()
        {
            if (evHUDView != null)
            {
                return;
            }

            evHUDView = new EVHUD();
            evHUDView.SupportedValues = SupportedEVValues;
            evHUDView.HorizontalAlignment = HorizontalAlignment.Right;
            evHUDView.Visibility = Visibility.Collapsed;
            evHUDView.PanelTransform.X = evHUDView.Width;

            LayoutRoot.Children.Add(evHUDView);
            evHUDView.SelectedValue = EVDialer.CurrentValue;
        }
        
        public void ShowEVHUD()
        {
            if (evHUDView == null)
            {
                CreateEVHUD();
            }

            if (Orientation == PageOrientation.LandscapeLeft || Orientation == PageOrientation.LandscapeRight)
            {
                HideLandscapeShutterButton();

                evHUDView.VerticalAlignment = VerticalAlignment.Bottom;
                evHUDView.Margin = new Thickness(0, 0, 0, 30);
            }
            else
            {
                evHUDView.VerticalAlignment = VerticalAlignment.Bottom;
                evHUDView.Margin = new Thickness(0, 0, 0, CameraSwitchButton.Margin.Bottom + 85);
            }

            evHUDView.PanelTransform.X = evHUDView.Width;
            evHUDView.Visibility = Visibility.Visible;

            Storyboard storyboard = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            storyboard.Duration = duration;

            DoubleAnimation panelAnimation = new DoubleAnimation();
            panelAnimation.Duration = duration;
            panelAnimation.To = 0;
            panelAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(panelAnimation, evHUDView);
            Storyboard.SetTargetProperty(panelAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(panelAnimation);

            storyboard.Begin();
            storyboard.Completed += (sender, e) =>
            {
            };
        }

        public void DismissEVHUD()
        {
            if (evHUDView == null)
            {
                return;
            }

            Storyboard storyboard = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            storyboard.Duration = duration;

            DoubleAnimation panelAnimation = new DoubleAnimation();
            panelAnimation.Duration = duration;
            panelAnimation.To = evHUDView.Width;
            panelAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(panelAnimation, evHUDView);
            Storyboard.SetTargetProperty(panelAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(panelAnimation);

            storyboard.Begin();
            storyboard.Completed += (sender, e) =>
            {
                evHUDView.Visibility = Visibility.Collapsed;

                if (Orientation == PageOrientation.LandscapeLeft || Orientation == PageOrientation.LandscapeRight)
                {
                    ShowLandscapeShutterButton();
                }

            };
        }

        #region ISO HUD

        private void CreateISOHUD()
        {
            if (isoHUDView != null)
            {
                return;
            }

            isoHUDView = new ISOHUD();
            isoHUDView.SupportedValues = SupportedISOValues;
            isoHUDView.HorizontalAlignment = HorizontalAlignment.Left;
            isoHUDView.VerticalAlignment = VerticalAlignment.Center;
            isoHUDView.Visibility = Visibility.Collapsed;
            isoHUDView.PanelTransform.X = -isoHUDView.Width;

            LayoutRoot.Children.Add(isoHUDView);
            isoHUDView.SelectedValue = ISODialer.CurrentValue;
        }

        public void ShowISOHUD()
        {
            if (isoHUDView == null)
            {
                CreateISOHUD();
            }

            if (Orientation == PageOrientation.LandscapeLeft || Orientation == PageOrientation.LandscapeRight)
            {
                HideLandscapeShutterButton();
            }

            isoHUDView.PanelTransform.X = -isoHUDView.Width;
            isoHUDView.Visibility = Visibility.Visible;

            Storyboard storyboard = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            storyboard.Duration = duration;

            DoubleAnimation panelAnimation = new DoubleAnimation();
            panelAnimation.Duration = duration;
            panelAnimation.To = 0;
            panelAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(panelAnimation, isoHUDView);
            Storyboard.SetTargetProperty(panelAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(panelAnimation);

            storyboard.Begin();
            storyboard.Completed += (sender, e) =>
            {

            };
        }

        public void DismissISOHUD()
        {
            if (isoHUDView == null)
            {
                return;
            }

            if (Orientation == PageOrientation.LandscapeLeft || Orientation == PageOrientation.LandscapeRight)
            {
                HideLandscapeShutterButton();
            }


            Storyboard storyboard = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            storyboard.Duration = duration;

            DoubleAnimation panelAnimation = new DoubleAnimation();
            panelAnimation.Duration = duration;
            panelAnimation.To = -isoHUDView.Width;
            panelAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(panelAnimation, isoHUDView);
            Storyboard.SetTargetProperty(panelAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(panelAnimation);

            storyboard.Begin();
            storyboard.Completed += (sender, e) =>
            {
                isoHUDView.Visibility = Visibility.Collapsed;

                if (Orientation == PageOrientation.LandscapeLeft || Orientation == PageOrientation.LandscapeRight)
                {
                    ShowLandscapeShutterButton();
                }

            };
        }

        #endregion

        #region OSD

        public void ShowOSD(FrameworkElement view = null)
        {
            if (Orientation == PageOrientation.LandscapeLeft || Orientation == PageOrientation.LandscapeRight)
            {
                HideLandscapeShutterButton();
            }

            if (view == null)
            {
                view = OSD.MainOSD;
            }

            OSD.ShowOSD(view);
            HUDSwitchButton.IsOn = true;
        }

        public void DismissOSD()
        {
            OSD.DismissOSD(() => 
            {
                if (Orientation == PageOrientation.LandscapeLeft || Orientation == PageOrientation.LandscapeRight)
                {
                    ShowLandscapeShutterButton();
                }

            });

            HUDSwitchButton.IsOn = false;
        }

        #endregion

    }
}
