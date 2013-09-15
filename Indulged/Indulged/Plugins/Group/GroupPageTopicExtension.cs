using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Avarice.Controls;
using Indulged.API.Cinderella.Events;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Indulged.Plugins.Group
{
    public partial class GroupPage
    {
        private void RefreshTopicListButton_Click(object sender, EventArgs e)
        {
            // Show progress bar
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "loading topics";

            // Refresh group photos
            Anaconda.AnacondaCore.GetGroupTopicsAsync(GroupSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });

        }

        private void AddTopicButton_Click(object sender, EventArgs e)
        {
            ShowComposerView();
        }

        private TopicComposerView composer;
        private Popup composerPopup;
        private ApplicationBar AppBarBeforeComposerPopup;

        private string addTopicSessionId;

        private void ShowComposerView()
        {
            LayoutRoot.IsHitTestVisible = false;

            AppBarBeforeComposerPopup = (ApplicationBar)this.ApplicationBar;

            composer = new TopicComposerView();
            composer.Width = LayoutRoot.ActualWidth;

            var ct = (CompositeTransform)composer.RenderTransform;
            ct.TranslateY = -composer.Height;

            composerPopup = new Popup();
            composerPopup.Child = composer;
            composerPopup.IsOpen = true;

            composer.Projection = new PlaneProjection { CenterOfRotationX = 0, RotationX = -90 };

            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.Duration = duration;

            // Content view
            DoubleAnimation contentAnimation = new DoubleAnimation();
            animation.Children.Add(contentAnimation);
            contentAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            contentAnimation.To = 0.2;
            contentAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(contentAnimation, LayoutRoot);
            Storyboard.SetTargetProperty(contentAnimation, new PropertyPath("Opacity"));

            DoubleAnimation yAnimation = new DoubleAnimation();
            animation.Children.Add(yAnimation);
            yAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            yAnimation.To = 0;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, composer);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));

            var planeAnimation = new DoubleAnimation();
            planeAnimation.Duration = duration;
            planeAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            animation.Children.Add(planeAnimation);
            planeAnimation.To = 0;
            Storyboard.SetTarget(planeAnimation, composer.Projection);
            Storyboard.SetTargetProperty(planeAnimation, new PropertyPath("RotationX"));

            animation.Completed += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    ApplicationBar = Resources["ComposerAppBar"] as ApplicationBar;

                    // Auto focus on subject field
                    composer.SubjectTextBox.Focus();
                });
            };
            animation.Begin();
        }

        private void DismissComposerView()
        {
            composer.Projection = new PlaneProjection { CenterOfRotationX = 0, RotationX = 0 };

            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.Duration = duration;

            var alphaAnimation = new DoubleAnimation();
            alphaAnimation.Duration = duration;
            animation.Children.Add(alphaAnimation);
            alphaAnimation.To = 0;
            Storyboard.SetTarget(alphaAnimation, composer);
            Storyboard.SetTargetProperty(alphaAnimation, new PropertyPath("Opacity"));

            var yAnimation = new DoubleAnimation();
            yAnimation.Duration = duration;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            animation.Children.Add(yAnimation);
            yAnimation.To = -composer.Height;
            Storyboard.SetTarget(yAnimation, composer);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));

            var planeAnimation = new DoubleAnimation();
            planeAnimation.Duration = duration;
            planeAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            animation.Children.Add(planeAnimation);
            planeAnimation.To = -90;
            Storyboard.SetTarget(planeAnimation, composer.Projection);
            Storyboard.SetTargetProperty(planeAnimation, new PropertyPath("RotationX"));

            animation.Completed += (sender, e) =>
            {
                composerPopup.IsOpen = false;
                composerPopup = null;
                composer = null;

                LayoutRoot.Opacity = 1;
                LayoutRoot.IsHitTestVisible = true;

                ApplicationBar = AppBarBeforeComposerPopup;
            };

            animation.Begin();
        }

        private void ComfirmAddTopicButton_Click(object sender, EventArgs e)
        {
            if (composer.SubjectTextBox.Text.Length == 0 || composer.MessageTextBox.Text.Length == 0)
            {
                composer.StatusTextView.Text = "Subject and content cannot be empty";
            }
            else
            {
                // Dismiss keyboard
                this.Focus();

                ApplicationBar.IsVisible = false;
                composer.SubjectTextBox.IsEnabled = false;
                composer.MessageTextBox.IsEnabled = false;
                composer.ComposerView.Opacity = 0.4;

                composer.StatusTextView.Text = "Posting to discussion board";
                composer.ProgressView.Visibility = Visibility.Visible;

                addTopicSessionId = Guid.NewGuid().ToString().Replace("-", null);
                Anaconda.AnacondaCore.AddTopicAsync(addTopicSessionId, GroupSource.ResourceId, composer.SubjectTextBox.Text, composer.MessageTextBox.Text);
            }
        }

        private void CancelAddTopicButton_Click(object sender, EventArgs e)
        {
            DismissComposerView();
        }

        // Capture back button
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (ModalPopup.HasPopupHistory())
            {
                e.Cancel = true;
                ModalPopup.RemoveLastPopup();
            }
            else if (composerPopup != null)
            {
                e.Cancel = true;
                DismissComposerView();
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }

        // Events
        private void OnAddTopicException(object sender, AddTopicExceptionEventArgs e)
        {
            if (composer == null || e.SessionId != addTopicSessionId)
                return;

            Dispatcher.BeginInvoke(() =>
            {
                ApplicationBar.IsVisible = true;
                composer.SubjectTextBox.IsEnabled = true;
                composer.MessageTextBox.IsEnabled = true;
                composer.ComposerView.Opacity = 1;

                composer.StatusTextView.Text = "An error occured while adding topic.";
                composer.ProgressView.Visibility = Visibility.Collapsed;
            });

        }

        private void OnAddTopicComplete(object sender, AddTopicCompleteEventArgs e)
        {
            if (composer == null || e.SessionId != addTopicSessionId)
                return;

            Dispatcher.BeginInvoke(() =>
            {
                ApplicationBar.IsVisible = true;
                DismissComposerView();
            });
        }
    }
}
