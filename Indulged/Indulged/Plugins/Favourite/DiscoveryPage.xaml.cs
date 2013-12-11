using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.API.Cinderella;
using System.Collections.ObjectModel;
using Indulged.API.Cinderella.Models;
using Indulged.API.Anaconda;
using Indulged.PolKit;
using Indulged.API.Cinderella.Events;
using Indulged.API.Avarice.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Indulged.Resources;

namespace Indulged.Plugins.Favourite
{
    public partial class DiscoveryPage : PhoneApplicationPage
    {
        private ObservableCollection<Photo> _photos = new ObservableCollection<Photo>();

        // Constructor
        public DiscoveryPage()
        {
            InitializeComponent();
        }

        private bool executedOnce = false;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (executedOnce)
                return;

            executedOnce = true;

            // Events
            Cinderella.CinderellaCore.DiscoveryStreamUpdated += OnDiscoveryStreamUpdated;
            Anaconda.AnacondaCore.DiscoveryStreamException += OnDiscoveryStreamException;
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
            Anaconda.AnacondaCore.DiscoveryStreamException -= OnDiscoveryStreamException;
            Cinderella.CinderellaCore.DiscoveryStreamUpdated -= OnDiscoveryStreamUpdated;

            ResultListView.ItemsSource = null;
            _photos.Clear();
            _photos = null;

            base.OnRemovedFromJournal(e);
        }

        // Can't load discovery stream
        private void OnDiscoveryStreamException(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (_photos.Count == 0)
                {
                    StatusLabel.Visibility = Visibility.Visible;
                    StatusLabel.Text = AppResources.DiscoveryPageLoadingErrorText;
                    ResultListView.Visibility = Visibility.Collapsed;

                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = false;
                }
            });
        }

        // Discovery stream updated
        private void OnDiscoveryStreamUpdated(object sender, DiscoveryStreamUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (SystemTray.ProgressIndicator != null)
                    SystemTray.ProgressIndicator.IsVisible = false;

                if (Cinderella.CinderellaCore.DiscoveryList.Count == 0)
                {
                    StatusLabel.Visibility = Visibility.Visible;
                    StatusLabel.Text = AppResources.GenericNoContentFound;
                    ResultListView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StatusLabel.Visibility = Visibility.Collapsed;
                    ResultListView.Visibility = Visibility.Visible;
                }

                if (e.NewPhotos.Count == 0)
                    return;

                foreach (var photo in e.NewPhotos)
                {
                    if(!_photos.Contains(photo))
                        _photos.Add(photo);
                }

            });
        }

        private void ResultListView_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            Photo photo = e.Container.Content as Photo;
            if (photo == null)
                return;

            bool canLoad = canLoad = (!Anaconda.AnacondaCore.IsLoadingDiscoveryStream && Cinderella.CinderellaCore.DiscoveryList.Count < Cinderella.CinderellaCore.TotalDiscoveryPhotosCount && Cinderella.CinderellaCore.TotalDiscoveryPhotosCount != 0);
            int index = _photos.IndexOf(photo);

            if (_photos.Count - index <= 2 && canLoad)
            {
                int page = page = Cinderella.CinderellaCore.DiscoveryList.Count / PolicyKit.StreamItemsCountPerPage + 1;

                if (SystemTray.ProgressIndicator != null)
                    SystemTray.ProgressIndicator.IsVisible = true;

                Anaconda.AnacondaCore.GetDiscoveryStreamAsync(new Dictionary<string, string> { { "page", page.ToString() }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
            }
        }

        private void RefreshPhotoListButton_Click(object sender, EventArgs e)
        {
            if (SystemTray.ProgressIndicator != null)
                SystemTray.ProgressIndicator.IsVisible = true;

            Anaconda.AnacondaCore.GetDiscoveryStreamAsync(new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });

        }

        private void PerformAppearanceAnimation()
        {
            double w = System.Windows.Application.Current.Host.Content.ActualWidth;
            double h = System.Windows.Application.Current.Host.Content.ActualHeight;

            CompositeTransform ct = (CompositeTransform)LayoutRoot.RenderTransform;
            ct.TranslateY = h;

            ct = (CompositeTransform)ContentPanel.RenderTransform;
            ct.TranslateX = w;

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
                // Show loading progress indicator
                SystemTray.ProgressIndicator = new ProgressIndicator();
                SystemTray.ProgressIndicator.IsIndeterminate = true;

                if (SystemTray.ProgressIndicator != null)
                    SystemTray.ProgressIndicator.IsVisible = false;

                SystemTray.ProgressIndicator.Text = "retrieving photos";

                if (Cinderella.CinderellaCore.DiscoveryList.Count > 0)
                {
                    StatusLabel.Visibility = Visibility.Collapsed;
                    ResultListView.Visibility = Visibility.Visible;
                    _photos.Clear();
                    foreach (var photo in Cinderella.CinderellaCore.DiscoveryList)
                    {
                        _photos.Add(photo);
                    }
                }
                else
                {
                    StatusLabel.Visibility = Visibility.Visible;
                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = true;
                    var currentUser = Cinderella.CinderellaCore.CurrentUser;
                    Anaconda.AnacondaCore.GetDiscoveryStreamAsync(new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
                }

                ResultListView.ItemsSource = _photos;

                PerformContentFlyInAnimation();

                // App bar
                ApplicationBar = Resources["AppBar"] as ApplicationBar;

            };
        }

        private void PerformContentFlyInAnimation()
        {
            double w = System.Windows.Application.Current.Host.Content.ActualWidth;

            LayoutRoot.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Content animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            galleryAnimation.To = 0.0;
            galleryAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, ContentPanel);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            animation.Children.Add(galleryAnimation);
            animation.Begin();
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