using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

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
            if (GroupSource == null)
                return;

            PhotoPageView.Group = GroupSource;
            TopicPageView.GroupSource = GroupSource;

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
            Cinderella.CinderellaCore.GroupPhotoListUpdated += OnPhotoListUpdated;
            Anaconda.AnacondaCore.GroupPhotoException += OnPhotoListException;

            Cinderella.CinderellaCore.GroupTopicsUpdated += OnTopicListUpdated;
            Anaconda.AnacondaCore.GroupTopicsException += OnTopicListException;

            // Events
            Anaconda.AnacondaCore.AddTopicException += OnAddTopicException;
            Cinderella.CinderellaCore.AddTopicCompleted += OnAddTopicComplete;

        }

        private bool executedOnce = false;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (executedOnce)
                return;

            executedOnce = true;

            PerformAppearanceAnimation();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                PerformDisappearanceAnimation();
            }
        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            Anaconda.AnacondaCore.AddTopicException -= OnAddTopicException;
            Cinderella.CinderellaCore.AddTopicCompleted -= OnAddTopicComplete;

            Cinderella.CinderellaCore.GroupPhotoListUpdated -= OnPhotoListUpdated;

            Cinderella.CinderellaCore.GroupTopicsUpdated -= OnTopicListUpdated;

            PhotoPageView.RemoveEventListeners();
            TopicPageView.RemoveEventListeners();


            GroupSource = null;

            base.OnRemovedFromJournal(e);
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

        private void OnPhotoListUpdated(object sender, GroupPhotoListUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.GroupId == GroupSource.ResourceId)
                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;
            });
        }

        private void OnPhotoListException(object sender, GetGroupPhotosExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.GroupId == GroupSource.ResourceId)
                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;

            });
        }

        private void OnTopicListUpdated(object sender, GroupTopicsUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.GroupId == GroupSource.ResourceId)
                {
                    if(SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;
                }
            });
        }

        private void OnTopicListException(object sender, GetGroupTopicsExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.GroupId == GroupSource.ResourceId)
                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;
            });
        }

        private void PerformAppearanceAnimation()
        {
            double h = System.Windows.Application.Current.Host.Content.ActualHeight;

            CompositeTransform ct = (CompositeTransform)LayoutRoot.RenderTransform;
            ct.TranslateY = h;

            LayoutRoot.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            galleryAnimation.To = 0.0;
            galleryAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, LayoutRoot);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);
            animation.Begin();
            animation.Completed += (sender, e) =>
            {
                string groupId = NavigationContext.QueryString["group_id"];

                if (Cinderella.CinderellaCore.GroupCache.ContainsKey(groupId))
                {
                    this.GroupSource = Cinderella.CinderellaCore.GroupCache[groupId];
                    this.DataContext = GroupSource;

                    // Config app bar
                    ApplicationBar = Resources["PhotoPageAppBar"] as ApplicationBar;
                }
            };
        }

 
        private void PerformDisappearanceAnimation()
        {
            double w = System.Windows.Application.Current.Host.Content.ActualWidth;
            double h = System.Windows.Application.Current.Host.Content.ActualHeight;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            yAnimation.To = h;
            yAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(yAnimation, LayoutRoot);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(yAnimation);
            animation.Begin();
        }
    }
}