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

            Anaconda.AnacondaCore.GetRequestTokenAsync();
        }

        private void requestTokenGranted(object sender, EventArgs e)
        {
            // Show the authentication page
            string authUrl = "http://www.flickr.com/services/oauth/authorize?oauth_token=" + Anaconda.AnacondaCore.RequestToken + "&perms=write";
            Uri loginUri = new Uri(authUrl, UriKind.Absolute);

            Browser.Navigate(loginUri);
        }

        private void OnBrowserBeginNavigating(object sender, NavigatingEventArgs e)
        {
            string url = e.Uri.AbsoluteUri;
            Debug.WriteLine(url);

            if (url.StartsWith("indulged://auth/?oauth_token="))
            {
                // Auth is successful
                e.Cancel = true;

                string paramString = url.Split('?')[1];
                string[] parts = paramString.Split('&');

                Anaconda.AnacondaCore.RequestTokenVerifier = parts[1].Split('=')[1];

                // Exchange for access token
                progressView = new ProgressView();
                progressView.Show();

                Anaconda.AnacondaCore.GetAccessTokenAsync();
            }
             
        }

        private void accessTokenGranted(object sender, EventArgs e)
        {
            if(progressView != null)
                progressView.Close();
            NavigationService.GoBack();
        }

        private void accessTokenFailed(object sender, EventArgs e)
        {
            if (progressView != null)
                progressView.Close();
            NavigationService.GoBack();

            ModalPopup.Show("Cannot authenticate with Flickr at this time", "Error", new List<string> {"Confirm"} );
        }

    }
}