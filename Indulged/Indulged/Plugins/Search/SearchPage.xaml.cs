using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Indulged.Plugins.Search
{
    public partial class SearchPage : PhoneApplicationPage
    {
        // Constructor
        public SearchPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PerformAppearanceAnimation();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            PerformDisappearAnimation();
            base.OnNavigatingFrom(e);
        }

        private void OnSearchBoxKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string trimmedQueryString = SearchBox.Text.Trim();
            if (e.Key == Key.Enter && trimmedQueryString.Length > 0)
            {
                NavigationService.Navigate(new Uri("/Plugins/Search/SearchResultPage.xaml?query=" + trimmedQueryString, UriKind.Relative));
                NavigationService.RemoveBackEntry();
            }
        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            TagListView.OnRemovedFromJournal();
            base.OnRemovedFromJournal(e);
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
            //galleryAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, LayoutRoot);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);
            animation.Begin();
            animation.Completed += (sender, e) =>
            {
                SearchBox.Focus();
            };
        }

        private void PerformDisappearAnimation()
        {
            double h = System.Windows.Application.Current.Host.Content.ActualHeight;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            galleryAnimation.To = h;
            galleryAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, LayoutRoot);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);
            animation.Begin();
        }

    }
}