using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;

using Indulged.API.Utils;
using Indulged.API.Avarice.Events;
using System.Windows.Shapes;

namespace Indulged.API.Avarice.Controls
{
    public class ModalPopup : Control
    {
        // Button click event
        public event EventHandler<ModalPopupEventArgs> DismissWithButtonClick;

        public ModalPopup()
        {
            DefaultStyleKey = typeof(ModalPopup);
        }

        // Host view
        private Panel _hostView = null;
        internal Panel HostView
        {
            get
            {
                if (_hostView == null)
                {
                   
                    Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                    PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
                    _hostView = (Panel)VisualTreeHelper.GetChild(currentPage, 0);
                }

                return _hostView;
            }
        }

        // Curtain and borders
        protected Rectangle curtain;
        protected Image topShadow;
        protected Image bottomShadow;
        protected Canvas borderCanvas;
        protected Grid contentView;
        protected StackPanel buttonContainer;

        // Content elements
        protected FrameworkElement contentElement;

        // Measured content element size
        protected Size expectedContentSize;

        // Dialog title
        protected string title = null;

        // Button titles
        protected List<String> buttonTitles = new List<string>();

        // Show the popup window with custom content
        public static ModalPopup Show(FrameworkElement content, string title = null, List<string> buttonTitles = null)
        {
            ModalPopup popup = new ModalPopup();
            popup.contentElement = content;

            // Set title
            if (title != null)
                popup.title = title;

            // Create optional buttons
            if (buttonTitles != null)
            {
                foreach (string buttonTitle in buttonTitles)
                {
                    popup.buttonTitles.Add(buttonTitle);
                }
            }

            popup.HostView.Children.Add(popup);
            return popup;
        }

        // Show the popup window with text
        public static ModalPopup Show(string text, string title = null, List<string> buttonTitles = null)
        {
            // Create a text label
            TextBlock label = new TextBlock();
            label.Margin = new Thickness(28);
            label.Text = text;
            label.Width = System.Windows.Application.Current.Host.Content.ActualWidth - label.Margin.Left - label.Margin.Right;
            label.TextWrapping = TextWrapping.Wrap;
            label.SetValue(Grid.RowProperty, 1);
            return Show(label, title, buttonTitles);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            initialChildren();

            // Perform fade in animation
            this.PerformAppearanceAnimation();
        }

        // Calculated content height
        protected virtual void initialChildren()
        {
            double w = System.Windows.Application.Current.Host.Content.ActualWidth;

            curtain = GetTemplateChild("Curtain") as Rectangle;
            topShadow = GetTemplateChild("TopShadow") as Image;
            bottomShadow = GetTemplateChild("BottomShadow") as Image;
            borderCanvas = GetTemplateChild("BorderCanvas") as Canvas;
            contentView = GetTemplateChild("ContentView") as Grid;
            buttonContainer = GetTemplateChild("ButtonContainer") as StackPanel;

            // Add an optional title label
            TextBlock titleLabel = null;
            if (title != null)
            {
                titleLabel = new TextBlock();
                titleLabel.Text = title;
                titleLabel.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xd9, 0xf3));
                titleLabel.FontSize = 42;
                titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
                titleLabel.TextWrapping = TextWrapping.Wrap;
                titleLabel.SetValue(Grid.RowProperty, 0);
                contentView.Children.Add(titleLabel);
            }

            // Add any custom content
            if (contentElement != null)
            {
                contentElement.SetValue(Grid.RowProperty, 1);
                contentView.Children.Add(contentElement);
                contentView.InvalidateArrange();
                contentView.UpdateLayout();

                double measuredWidth = contentElement.ActualWidth + contentElement.Margin.Left + contentElement.Margin.Right;
                double measuredHeight = 0;
                if (titleLabel != null)
                {
                    measuredHeight += titleLabel.ActualHeight;
                }
                
                double contentHeight = Math.Max(contentElement.ActualHeight, contentElement.Height);
                if(!double.IsNaN(contentHeight))
                    measuredHeight += contentHeight + contentElement.Margin.Top + contentElement.Margin.Bottom;
                else
                    measuredHeight += 240 + contentElement.Margin.Top + contentElement.Margin.Bottom;

                expectedContentSize = new Size(measuredWidth, measuredHeight);
            }
            else
            {
                expectedContentSize = new Size(w, 240);
            }

            // Add custom buttons
            if (buttonTitles.Count > 0)
            {
                foreach (string buttonTitle in buttonTitles)
                {
                    var button = new Avarice.Controls.Button();
                    button.Content = buttonTitle;
                    button.Margin = new Thickness(14, 0, 0, 0);
                    button.HorizontalAlignment = HorizontalAlignment.Right;
                    buttonContainer.Children.Add(button);
                    button.Click += OnButtonClick;
                }
            }
        }

        protected void PerformAppearanceAnimation()
        {
            double w = System.Windows.Application.Current.Host.Content.ActualWidth;
            
            // Initial settings
            borderCanvas.Width = w;
            borderCanvas.Height = expectedContentSize.Height;

            topShadow.Width = w;
            topShadow.SetValue(Canvas.LeftProperty, -w);

            bottomShadow.Width = w;
            bottomShadow.SetValue(Canvas.LeftProperty, w);
            bottomShadow.SetValue(Canvas.TopProperty, borderCanvas.Height);

            buttonContainer.Opacity = 0;
            
            // Content view
            contentView.Projection = new PlaneProjection { CenterOfRotationX = 0, RotationX = -90 };
            //contentView.Opacity = 0;

            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));

            animation.Duration = duration;

            DoubleAnimation curtainAnimation = new DoubleAnimation();
            animation.Children.Add(curtainAnimation);
            curtainAnimation.Duration = duration;
            curtainAnimation.To = 1;
            curtainAnimation.From = 0;
            curtainAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(curtainAnimation, curtain);
            Storyboard.SetTargetProperty(curtainAnimation, new PropertyPath("Opacity"));

            DoubleAnimation topShadowAnimation = new DoubleAnimation();
            animation.Children.Add(topShadowAnimation);
            topShadowAnimation.Duration = duration;
            topShadowAnimation.To = 0;
            topShadowAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(topShadowAnimation, topShadow);
            Storyboard.SetTargetProperty(topShadowAnimation, new PropertyPath("(Canvas.Left)"));

            DoubleAnimation bottomShadowAnimation = new DoubleAnimation();
            animation.Children.Add(bottomShadowAnimation);
            bottomShadowAnimation.Duration = duration;
            bottomShadowAnimation.To = 0;
            bottomShadowAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(bottomShadowAnimation, bottomShadow);
            Storyboard.SetTargetProperty(bottomShadowAnimation, new PropertyPath("(Canvas.Left)"));

            // Button container animation
            var buttonContainerAnimation = new DoubleAnimation();
            buttonContainerAnimation.Duration = duration;
            animation.Children.Add(buttonContainerAnimation);
            buttonContainerAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            buttonContainerAnimation.To = 1;
            Storyboard.SetTarget(buttonContainerAnimation, buttonContainer);
            Storyboard.SetTargetProperty(buttonContainerAnimation, new PropertyPath("Opacity"));

            // Content view
            var contentAnimation = new DoubleAnimation();
            contentAnimation.Duration = duration;
            animation.Children.Add(contentAnimation);
            contentAnimation.To = 0;
            Storyboard.SetTarget(contentAnimation, contentView.Projection);
            Storyboard.SetTargetProperty(contentAnimation, new PropertyPath("RotationX"));


            animation.Begin();
        }

        public void DismissWithButtonIndex(int buttonIndex)
        {
            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.Duration = duration;

            DoubleAnimation curtainAnimation = new DoubleAnimation();
            animation.Children.Add(curtainAnimation);
            curtainAnimation.Duration = duration;
            curtainAnimation.To = 0;
            Storyboard.SetTarget(curtainAnimation, this);
            Storyboard.SetTargetProperty(curtainAnimation, new PropertyPath("Opacity"));

            animation.Begin();
            animation.Completed += (sender, args) =>
            {
                HostView.Children.Remove(this);

                var e = new ModalPopupEventArgs();
                e.ButtonIndex = buttonIndex;
                DismissWithButtonClick.DispatchEventOnMainThread(this, e);
            };
        }

        // Button click event
        protected void OnButtonClick(object sender, RoutedEventArgs args)
        {
            Avarice.Controls.Button targetButton = (Avarice.Controls.Button)sender;
            int buttonIndex = buttonContainer.Children.IndexOf(targetButton);

            // Dismiss self
            DismissWithButtonIndex(buttonIndex);
        }
    }
}
