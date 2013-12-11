using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Diagnostics;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.Plugins.Common;
using Indulged.API.Anaconda;
using Indulged.API.Avarice.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Indulged.Resources;


namespace Indulged.Plugins.Login
{
    public partial class LoginPage : PhoneApplicationPage
    {
        // Progress view
        private ProgressView progressView;

        public LoginPage()
        {
            InitializeComponent();

            // Events
            Anaconda.AnacondaCore.RequestTokenGranted += requestTokenGranted;
            Anaconda.AnacondaCore.AccessTokenGranted += accessTokenGranted;
            Anaconda.AnacondaCore.AccessTokenFailed += accessTokenFailed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            Anaconda.AnacondaCore.RequestTokenGranted -= requestTokenGranted;
            Anaconda.AnacondaCore.AccessTokenGranted -= accessTokenGranted;
            Anaconda.AnacondaCore.AccessTokenFailed -= accessTokenFailed;

            base.OnRemovedFromJournal(e);
        }

        private void ShowBrowserView()
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            CompositeTransform ct = (CompositeTransform)BrowserView.RenderTransform;
            ct.TranslateY = h;

            BrowserView.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            galleryAnimation.To = 0.0;
            galleryAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, BrowserView);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);
            animation.Completed += (sender, e) => {
                LoadingText.Visibility = Visibility.Visible;

                Anaconda.AnacondaCore.GetRequestTokenAsync();
            };
            animation.Begin();
        }

        private void HideBrowserView()
        {
            double w = LayoutRoot.ActualWidth;
            double h = LayoutRoot.ActualHeight;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            galleryAnimation.To = h;
            galleryAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, BrowserView);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);
            animation.Completed += (sender, e) =>
            {
                BrowserView.Visibility = Visibility.Collapsed;
            };

            animation.Begin();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (ModalPopup.HasPopupHistory())
            {
                e.Cancel = true;
                ModalPopup.RemoveLastPopup();
            }
            else if (BrowserView.Visibility == Visibility.Visible)
            {
                e.Cancel = true;
                HideBrowserView();                
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }

        private void requestTokenGranted(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (Browser.Visibility == Visibility.Collapsed)
                    return;

                // Show the authentication page
                string authUrl = "http://www.flickr.com/services/oauth/authorize?oauth_token=" + Anaconda.AnacondaCore.RequestToken + "&perms=write";
                Uri loginUri = new Uri(authUrl, UriKind.Absolute);

                Browser.Navigate(loginUri);
            });
        }

        private void OnBrowserBeginNavigating(object sender, NavigatingEventArgs e)
        {
            string url = e.Uri.AbsoluteUri;
            Debug.WriteLine(url);

            LoadingText.Visibility = Visibility.Collapsed;
            LoadingProgressView.Visibility = Visibility.Visible;

            if (url.StartsWith("indulged://auth/?oauth_token="))
            {
                // Auth is successful
                e.Cancel = true;

                string paramString = url.Split('?')[1];
                string[] parts = paramString.Split('&');

                Anaconda.AnacondaCore.RequestTokenVerifier = parts[1].Split('=')[1];

                // Exchange for access token
                LayoutRoot.Visibility = Visibility.Collapsed;
                progressView = new ProgressView();
                progressView.Show();

                Anaconda.AnacondaCore.GetAccessTokenAsync();
            }
             
        }

        private void accessTokenGranted(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (progressView != null)
                    progressView.Close();

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                NavigationService.RemoveBackEntry();

            });
        }

        private void accessTokenFailed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (progressView != null)
                    progressView.Close();

                ModalPopup.Show(AppResources.LoginAuthErrorText, AppResources.GenericErrorTitleText, new List<string> { AppResources.GenericConfirmText });
            });
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            ShowBrowserView();
        }

        private void BrowserBackButton_Click(object sender, RoutedEventArgs e)
        {
            HideBrowserView();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            string aboutText = "Indulged for Windows Phone\n\nVersion 1.1\n\nFrom photographer, for photographer!\n\n2013 - 2014 Tiangong You, all rights reserved";
            ModalPopup.Show(aboutText, AppResources.LoginAboutAppTitleText, new List<string> { AppResources.GenericDoneText });
        }

        private void Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            LoadingProgressView.Visibility = Visibility.Collapsed;
        }

    }
}