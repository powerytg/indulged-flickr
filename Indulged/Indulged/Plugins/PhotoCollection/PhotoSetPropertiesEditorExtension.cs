using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.Resources;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Indulged.Plugins.PhotoCollection
{
    public partial class PhotoSetPage
    {
        private PhotoSetPropertyView composer = null;
        private Popup composerPopup;
        private ApplicationBar AppBarBeforeComposerPopup;

        private void ComfirmEditPropertyButton_Click(object sender, System.EventArgs e)
        {
            if (composer.TitleTextBox.Text.Length == 0)
            {
                composer.StatusTextView.Text = AppResources.ReplyCannotBeEmptyText;
            }
            else
            {
                // Dismiss keyboard
                this.Focus();

                ApplicationBar.IsVisible = false;
                composer.DescTextBox.IsEnabled = false;
                composer.ComposerView.Opacity = 0.4;

                composer.StatusTextView.Text = AppResources.DiscussionPostingText;
                composer.ProgressView.Visibility = Visibility.Visible;

                var descText = (composer.DescTextBox.Text.Length == 0) ? null : composer.DescTextBox.Text;
                Anaconda.AnacondaCore.EditPhotoSetAsync(PhotoSetSource.ResourceId, composer.TitleTextBox.Text, descText);
            }
        }

        private void CancelEditPropertyButton_Click(object sender, System.EventArgs e)
        {
            DismissPropertyEditorView();
        }

        private void ShowPropertyEditorView()
        {
            Cinderella.CinderellaCore.PhotoSetUpdated += OnPhotoSetUpdated;
            Anaconda.AnacondaCore.PhotoSetEditException += OnEditPhotoSetException;

            LayoutRoot.IsHitTestVisible = false;

            AppBarBeforeComposerPopup = (ApplicationBar)this.ApplicationBar;

            composer = new PhotoSetPropertyView();
            composer.TitleTextBox.Text = PhotoSetSource.Title;
            composer.DescTextBox.Text = PhotoSetSource.Description;
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
                    ApplicationBar = Resources["PropertyEditorrAppBar"] as ApplicationBar;

                    // Auto focus on subject field
                    composer.TitleTextBox.Focus();
                });
            };
            animation.Begin();
        }

        private void DismissPropertyEditorView()
        {
            Cinderella.CinderellaCore.PhotoSetUpdated -= OnPhotoSetUpdated;
            Anaconda.AnacondaCore.PhotoSetEditException -= OnEditPhotoSetException;

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
                ApplicationBar.IsVisible = true;
            };

            animation.Begin();
        }

        // Events
        private void OnEditPhotoSetException(object sender, EditPhotoSetExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (composer == null)
                {
                    return;
                }

                ApplicationBar.IsVisible = true;
                composer.TitleTextBox.IsEnabled = true;
                composer.ComposerView.Opacity = 1;

                composer.StatusTextView.Text = "An error happened while editing this photo set";
                composer.ProgressView.Visibility = Visibility.Collapsed;
            });

        }

        private void OnPhotoSetUpdated(object sender, PhotoSetUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (composer == null)
                {
                    return;
                }

                DismissPropertyEditorView();
                PhotoStreamListView.ItemsSource = null;
                PhotoStreamListView.ItemsSource = PhotoCollection;
            });
        }

     }
}
