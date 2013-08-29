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
using Microsoft.Phone.Shell;

using Indulged.API.Utils;
using Indulged.API.Avarice.Events;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Indulged.API.Avarice.Controls
{
    public class ModalPopup : Control
    {
        // Button click event
        public event EventHandler<ModalPopupEventArgs> DismissWithButtonClick;
        public event EventHandler<ModalPopupEventArgs> ButtonClick;

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

        private PhoneApplicationPage CurrentPage
        {
            get
            {
                Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                return (PhoneApplicationPage)rootVisual.Content;
            }
        }

        // Curtain and borders
        protected Rectangle topShadow;
        protected Rectangle bottomShadow;
        protected Rectangle curtain;
        protected Grid contentView;
        protected StackPanel buttonContainer;
        protected TextBlock titleLabel;

        // Content elements
        protected FrameworkElement contentElement;

        // Measured content element size
        protected Size expectedContentSize;

        // Dialog title
        protected string title = null;
        
        // Button titles
        protected bool shouldAutoDismissWhenClickOnButtons;
        protected List<String> buttonTitles = new List<string>();
        public List<Avarice.Controls.Button> Buttons = new List<Button>();

        // Reference to the parent popup
        protected Popup popupContainer;

        // Show the popup window with custom content
        public static ModalPopup ShowWithButtons(FrameworkElement content, string title = null, List<Avarice.Controls.Button> _buttons = null, bool _shoulsAutoDismissWhenClickingOnButtons = true)
        {
            Popup popupContainer = new Popup();
            ModalPopup popup = new ModalPopup();
            popupContainer.Child = popup;
            popup.contentElement = content;
            popup.popupContainer = popupContainer;
            popup.shouldAutoDismissWhenClickOnButtons = _shoulsAutoDismissWhenClickingOnButtons;

            // Set title
            if (title != null)
                popup.title = title;

            // Create optional buttons
            if (_buttons != null)
            {
                foreach (var btn in _buttons)
                {
                    popup.Buttons.Add(btn);
                }
            }

            popup.HostView.Opacity = 0.2;
            popup.HostView.IsHitTestVisible = false;
            popupContainer.IsOpen = true;

            return popup;

        }

        public static ModalPopup Show(FrameworkElement content, string title = null, List<string> buttonTitles = null, bool _shoulsAutoDismissWhenClickingOnButtons = true)
        {
            Popup popupContainer = new Popup();
            ModalPopup popup = new ModalPopup();
            popupContainer.Child = popup;
            popup.contentElement = content;
            popup.popupContainer = popupContainer;
            popup.shouldAutoDismissWhenClickOnButtons = _shoulsAutoDismissWhenClickingOnButtons;

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

            popup.HostView.Opacity = 0.2;
            popup.HostView.IsHitTestVisible = false;
            popupContainer.IsOpen = true;
            return popup;
        }

        // Show the popup window with text
        public static ModalPopup Show(string text, string title = null, List<string> buttonTitles = null, bool _shoulsAutoDismissWhenClickingOnButtons = true)
        {
            // Create a text label
            TextBlock label = new TextBlock();
            label.Foreground = new SolidColorBrush(Colors.White);
            label.FontSize = 24;
            label.Margin = new Thickness(28);
            label.Text = text;
            label.Width = System.Windows.Application.Current.Host.Content.ActualWidth - label.Margin.Left - label.Margin.Right;
            label.TextWrapping = TextWrapping.Wrap;
            return Show(label, title, buttonTitles, _shoulsAutoDismissWhenClickingOnButtons);
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

            topShadow = GetTemplateChild("TopShadow") as Rectangle;
            bottomShadow = GetTemplateChild("BottomShadow") as Rectangle;
            contentView = GetTemplateChild("ContentView") as Grid;
            buttonContainer = GetTemplateChild("ButtonContainer") as StackPanel;
            curtain = GetTemplateChild("Curtain") as Rectangle;

            // Add an optional title label
            titleLabel = null;
            if (title != null)
            {
                titleLabel = new TextBlock();
                titleLabel.Text = title;
                titleLabel.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xd9, 0xf3));
                titleLabel.FontSize = 42;
                titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
                titleLabel.TextWrapping = TextWrapping.Wrap;
                titleLabel.Margin = new Thickness(0, 8, 0, 8);
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
                    button.Margin = new Thickness(20, 0, 20, 0);
                    button.HorizontalAlignment = HorizontalAlignment.Right;
                    buttonContainer.Children.Add(button);
                    button.Click += OnButtonClick;
                }
            }
            else if(Buttons.Count > 0)
            {
                foreach (var button in Buttons)
                {
                    button.Margin = new Thickness(20, 0, 20, 0);
                    button.HorizontalAlignment = HorizontalAlignment.Right;
                    buttonContainer.Children.Add(button);
                    button.Click += OnButtonClick;
                }

            }
        }

        private bool isApplicationBarVisibleBeforePopup;
        private bool isSystemTrayVisibleBeforePopup;

        public void Dismiss()
        {
            DismissWithAction(null);
        }

        public void DismissWithAction(Action action)
        {
            this.Projection = new PlaneProjection { CenterOfRotationX = 0, RotationX = 0 };

            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.Duration = duration;

            var alphaAnimation = new DoubleAnimation();
            alphaAnimation.Duration = duration;
            alphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            animation.Children.Add(alphaAnimation);
            alphaAnimation.To = 0;
            Storyboard.SetTarget(alphaAnimation, this);
            Storyboard.SetTargetProperty(alphaAnimation, new PropertyPath("Opacity"));


            var planeAnimation = new DoubleAnimation();
            planeAnimation.Duration = duration;
            planeAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            animation.Children.Add(planeAnimation);
            planeAnimation.To = -90;
            Storyboard.SetTarget(planeAnimation, this.Projection);
            Storyboard.SetTargetProperty(planeAnimation, new PropertyPath("RotationX"));

            animation.Begin();
            animation.Completed += (sender, args) =>
            {
                popupContainer.IsOpen = false;
                popupContainer = null;

                Dispatcher.BeginInvoke(() =>
                {
                    // Show application bar
                    if (isApplicationBarVisibleBeforePopup)
                        CurrentPage.ApplicationBar.IsVisible = true;

                    if (isSystemTrayVisibleBeforePopup)
                        SystemTray.IsVisible = true;

                    HostView.IsHitTestVisible = true;
                    HostView.Opacity = 1;

                    // Perform afterburn action
                    if(action != null)
                        action();
                });

            };
        }

        public void ReplaceContentWith(string newTitle, FrameworkElement newElement, List<Avarice.Controls.Button> newButtons)
        {
            double w = System.Windows.Application.Current.Host.Content.ActualWidth;

            // Add new content element
            newElement.SetValue(Grid.RowProperty, 1);
            CompositeTransform ct = (CompositeTransform)newElement.RenderTransform;
            if (ct == null)
            {
                ct = new CompositeTransform();
                newElement.RenderTransform = ct;
            }

            ct.TranslateX = w;

            contentView.Children.Add(newElement);

                
            // Slide away the old contents
            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.5));

            DoubleAnimation oldContentXAnimation = new DoubleAnimation();
            animation.Children.Add(oldContentXAnimation);
            oldContentXAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            oldContentXAnimation.To = -w;
            oldContentXAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(oldContentXAnimation, contentElement);
            Storyboard.SetTargetProperty(oldContentXAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));

            // Slide in the new content element
            DoubleAnimationUsingKeyFrames newContentXAnimation = new DoubleAnimationUsingKeyFrames();
            animation.Children.Add(newContentXAnimation);
            newContentXAnimation.Duration = animation.Duration;
            newContentXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = w });
            newContentXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.2), Value = w });
            newContentXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.5), Value = 0 });
            Storyboard.SetTarget(newContentXAnimation, newElement);
            Storyboard.SetTargetProperty(newContentXAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));

            animation.Completed += (sender, e) => {
                contentView.Children.Remove(contentElement);
                contentElement = newElement;
            };
            animation.Begin();

            PerformOldButtonSwapOutAnimation(newButtons);

            
            // Change title
            titleLabel.Text = newTitle;
        }

        private void PerformOldButtonSwapOutAnimation(List<Avarice.Controls.Button> newButtons)
        {
            double w = System.Windows.Application.Current.Host.Content.ActualWidth;

            // Slide away old buttons
            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation oldButtonDeckAnimation = new DoubleAnimation();
            animation.Children.Add(oldButtonDeckAnimation);
            oldButtonDeckAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            oldButtonDeckAnimation.To = -w;
            oldButtonDeckAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(oldButtonDeckAnimation, buttonContainer);
            Storyboard.SetTargetProperty(oldButtonDeckAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));

            animation.Completed += (sender, e) => {
                buttonContainer.Children.Clear();
                PerformNewButtonSwapInAnimation(newButtons);
            };

            animation.Begin();
        }

        private void PerformNewButtonSwapInAnimation(List<Avarice.Controls.Button> newButtons)
        {
            double w = System.Windows.Application.Current.Host.Content.ActualWidth;

            var ct = (CompositeTransform)buttonContainer.RenderTransform;
            ct.TranslateX = w;

            foreach (var button in newButtons)
            {
                button.Margin = new Thickness(20, 0, 20, 0);
                button.HorizontalAlignment = HorizontalAlignment.Right;
                buttonContainer.Children.Add(button);
                button.Click += OnButtonClick;
            }

            // Slide in new buttons
            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));

            DoubleAnimation oldButtonDeckAnimation = new DoubleAnimation();
            animation.Children.Add(oldButtonDeckAnimation);
            oldButtonDeckAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            oldButtonDeckAnimation.To = 0;
            oldButtonDeckAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(oldButtonDeckAnimation, buttonContainer);
            Storyboard.SetTargetProperty(oldButtonDeckAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));

            animation.Completed += (sender, e) =>
            {
                Buttons = newButtons;
            };

            animation.Begin();
        }

        protected void PerformAppearanceAnimation()
        {
            // Hide application bar and system tray
            isSystemTrayVisibleBeforePopup = SystemTray.IsVisible;
            SystemTray.IsVisible = false;

            if (CurrentPage.ApplicationBar != null)
            {
                isApplicationBarVisibleBeforePopup = CurrentPage.ApplicationBar.IsVisible;
                CurrentPage.ApplicationBar.IsVisible = false;
            }

            double w = System.Windows.Application.Current.Host.Content.ActualWidth;
            double h = System.Windows.Application.Current.Host.Content.ActualHeight;

            // Initial settings
            Width = w;
            Height = h;

            CompositeTransform ct = (CompositeTransform)topShadow.RenderTransform;
            ct.TranslateY = -h;

            ct = (CompositeTransform)bottomShadow.RenderTransform;
            ct.TranslateY = h;

            ct = (CompositeTransform)curtain.RenderTransform;
            ct.ScaleY = h / expectedContentSize.Height;

            // Content view
            contentView.Opacity = 0;

            // Buttons
            buttonContainer.Opacity = 0;
            ct = (CompositeTransform)buttonContainer.RenderTransform;
            ct.TranslateY = -120;

            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.5));

            animation.Duration = duration;

            DoubleAnimation topShadowAnimation = new DoubleAnimation();
            animation.Children.Add(topShadowAnimation);
            topShadowAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            topShadowAnimation.To = 0;
            topShadowAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(topShadowAnimation, topShadow);
            Storyboard.SetTargetProperty(topShadowAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));

            DoubleAnimation bottomShadowAnimation = new DoubleAnimation();
            animation.Children.Add(bottomShadowAnimation);
            bottomShadowAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            bottomShadowAnimation.To = 0;
            bottomShadowAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(bottomShadowAnimation, bottomShadow);
            Storyboard.SetTargetProperty(bottomShadowAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));

            // Curtain animation
            DoubleAnimation curtainHeightAnimation = new DoubleAnimation();
            animation.Children.Add(curtainHeightAnimation);
            curtainHeightAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            curtainHeightAnimation.To = 1.0;
            curtainHeightAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(curtainHeightAnimation, curtain);
            Storyboard.SetTargetProperty(curtainHeightAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));

            // Curtain alpha animation
            var curtainAlphaAnimation = new DoubleAnimationUsingKeyFrames();
            curtainAlphaAnimation.Duration = duration;
            animation.Children.Add(curtainAlphaAnimation);
            curtainAlphaAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = 0.3 });
            curtainAlphaAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.3), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } });
            Storyboard.SetTarget(curtainAlphaAnimation, curtain);
            Storyboard.SetTargetProperty(curtainAlphaAnimation, new PropertyPath("Opacity"));


            // Content view
            var contentAnimation = new DoubleAnimationUsingKeyFrames();
            contentAnimation.Duration = duration;
            animation.Children.Add(contentAnimation);
            contentAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.2), Value = 0.0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            contentAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.4), Value = 1.0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            Storyboard.SetTarget(contentAnimation, contentView);
            Storyboard.SetTargetProperty(contentAnimation, new PropertyPath("Opacity"));

            // Button container animation
            var buttonAnimation = new DoubleAnimationUsingKeyFrames();
            buttonAnimation.Duration = duration;
            animation.Children.Add(buttonAnimation);
            buttonAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0), Value = -120 });
            buttonAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.5), Value = 0, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            Storyboard.SetTarget(buttonAnimation, buttonContainer);
            Storyboard.SetTargetProperty(buttonAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));

            var buttonAlphaAnimation = new DoubleAnimationUsingKeyFrames();
            buttonAlphaAnimation.Duration = duration;
            animation.Children.Add(buttonAlphaAnimation);
            buttonAlphaAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.3), Value = 0 });
            buttonAlphaAnimation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.5), Value = 1, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } });
            Storyboard.SetTarget(buttonAlphaAnimation, buttonContainer);
            Storyboard.SetTargetProperty(buttonAlphaAnimation, new PropertyPath("Opacity"));
            animation.Begin();
        }

        public void DismissWithButtonIndex(int buttonIndex)
        {
            this.Projection = new PlaneProjection { CenterOfRotationX = 0, RotationX = 0 };

            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.Duration = duration;

            var alphaAnimation = new DoubleAnimation();
            alphaAnimation.Duration = duration;
            alphaAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            animation.Children.Add(alphaAnimation);
            alphaAnimation.To = 0;
            Storyboard.SetTarget(alphaAnimation, this);
            Storyboard.SetTargetProperty(alphaAnimation, new PropertyPath("Opacity"));


            var planeAnimation = new DoubleAnimation();
            planeAnimation.Duration = duration;
            planeAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            animation.Children.Add(planeAnimation);
            planeAnimation.To = -90;
            Storyboard.SetTarget(planeAnimation, this.Projection);
            Storyboard.SetTargetProperty(planeAnimation, new PropertyPath("RotationX"));

            animation.Begin();
            animation.Completed += (sender, args) =>
            {
                popupContainer.IsOpen = false;
                popupContainer = null;

                var e = new ModalPopupEventArgs();
                e.ButtonIndex = buttonIndex;
                DismissWithButtonClick.DispatchEventOnMainThread(this, e);

                Dispatcher.BeginInvoke(() => {
                    // Show application bar
                    if (isApplicationBarVisibleBeforePopup)
                        CurrentPage.ApplicationBar.IsVisible = true;

                    if (isSystemTrayVisibleBeforePopup)
                        SystemTray.IsVisible = true;

                    HostView.IsHitTestVisible = true;
                    HostView.Opacity = 1;
                });

            };
        }

        // Button click event
        protected void OnButtonClick(object sender, RoutedEventArgs args)
        {
            Avarice.Controls.Button targetButton = (Avarice.Controls.Button)sender;
            int buttonIndex = buttonContainer.Children.IndexOf(targetButton);

            // Dismiss self
            if (shouldAutoDismissWhenClickOnButtons)
            {
                DismissWithButtonIndex(buttonIndex);
            }
            else
            {
                var evt = new ModalPopupEventArgs();
                evt.ButtonIndex = buttonIndex;
                ButtonClick.DispatchEvent(this, evt);
            }
        }
    }
}
