using Indulged.Plugins.ProCam.HUD;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Indulged.Plugins.ProCam
{
    public partial class ProCamPage
    {
        private EVHUD evHUDView;

        private void CreateEVHUD()
        {
            if (evHUDView != null)
            {
                return;
            }

            evHUDView = new EVHUD();
            evHUDView.SupportedValues = supportedEVValues;
            evHUDView.HorizontalAlignment = HorizontalAlignment.Right;
            evHUDView.VerticalAlignment = VerticalAlignment.Bottom;
            evHUDView.Margin = new Thickness(0, 0, 0, BottomToolbar.ActualHeight + 60);
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
            };
        }

    }
}
