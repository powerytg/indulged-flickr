using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella;
using Indulged.API.Anaconda;
using Indulged.API.Avarice.Controls;
using Indulged.API.Avarice.Events;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;

namespace Indulged.Plugins.Group
{
    public partial class GroupPage : PhoneApplicationPage
    {
        public static readonly DependencyProperty GroupSourceProperty = DependencyProperty.Register("GroupSource", typeof(FlickrGroup), typeof(GroupPage), new PropertyMetadata(OnGroupSourcePropertyChanged));

        public FlickrGroup GroupSource
        {
            get
            {
                return (FlickrGroup)GetValue(GroupSourceProperty);
            }
            set
            {
                SetValue(GroupSourceProperty, value);
            }
        }

        public static void OnGroupSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((GroupPage)sender).OnGroupSourceChanged();
        }

        protected virtual void OnGroupSourceChanged()
        {
            PhotoPageView.Group = GroupSource;
            TopicPageView.Group = GroupSource;

            // Show loading progress indicator
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "loading photos";

            // Get first page of photos
            Anaconda.AnacondaCore.GetGroupPhotosAsync(GroupSource.ResourceId, new Dictionary<string, string> { {"page" , "1"}, {"per_page" , Anaconda.DefaultItemsPerPage.ToString()} });

            // Get first page of topics
            Anaconda.AnacondaCore.GetGroupTopicsAsync(GroupSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

        // Constructor
        public GroupPage()
        {
            InitializeComponent();

            // Events
            Cinderella.CinderellaCore.GroupPhotoListUpdated += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (e.GroupId == GroupSource.ResourceId)
                        SystemTray.ProgressIndicator.IsVisible = false;
                });
            };

            Cinderella.CinderellaCore.GroupTopicsUpdated += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (e.GroupId == GroupSource.ResourceId)
                        SystemTray.ProgressIndicator.IsVisible = false;
                });
            };

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string groupId = NavigationContext.QueryString["group_id"];
            
            this.GroupSource = Cinderella.CinderellaCore.GroupCache[groupId];
            this.DataContext = GroupSource;

            // Config app bar
            ApplicationBar = Resources["PhotoPageAppBar"] as ApplicationBar;
        }

        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PanoramaView.SelectedIndex == 0)
            {
                ApplicationBar = Resources["PhotoPageAppBar"] as ApplicationBar;
            }
            else if (PanoramaView.SelectedIndex == 1)
            {
                ApplicationBar = Resources["TopicPageAppBar"] as ApplicationBar;
            }
        }

        private void RefreshPhotoListButton_Click(object sender, EventArgs e)
        {
            // Show progress bar
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "loading photos";

            // Refresh group photos
            Anaconda.AnacondaCore.GetGroupPhotosAsync(GroupSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

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

        private void ShowComposerView()
        {
            LayoutRoot.Opacity = 0;
            LayoutRoot.IsHitTestVisible = false;

            AppBarBeforeComposerPopup = (ApplicationBar)this.ApplicationBar;

            composer = new TopicComposerView();
            composer.Width = LayoutRoot.ActualWidth;
            
            var ct = (CompositeTransform)composer.RenderTransform;
            ct.TranslateY = -composer.Height;

            composerPopup = new Popup();
            composerPopup.Child = composer;
            composerPopup.IsOpen = true;


            Storyboard animation = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(0.3));
            animation.Duration = duration;

            DoubleAnimation yAnimation = new DoubleAnimation();
            animation.Children.Add(yAnimation);
            yAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            yAnimation.To = 0;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(yAnimation, composer);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));

            animation.Completed += (sender, e) => {
                Dispatcher.BeginInvoke(() => {
                    ApplicationBar = Resources["ComposerAppBar"] as ApplicationBar;
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
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
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

            animation.Completed += (sender, e) => {
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
            DismissComposerView();
        }

        private void CancelAddTopicButton_Click(object sender, EventArgs e)
        {
            DismissComposerView();
        }

        // Capture back button
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (composerPopup != null)
            {
                e.Cancel = true;
                DismissComposerView();
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }
    }
}