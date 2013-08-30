﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.Resources;

using Indulged.API.Utils;
using Indulged.API.Avarice.Controls;
using Indulged.API.Avarice.Events;
using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.PolKit;
using Indulged.Plugins.Dashboard;
using Indulged.Plugins.Search;

namespace Indulged
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Retrieve policy settings
            PolicyKit.RetrieveSettings();
        }

        private bool hasExecutedOnce = false;
        private int streamItemsPerPage = 100;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (hasExecutedOnce)
                return;

            hasExecutedOnce = true;

            // Try to get credenticls
            bool tokenRetrieved = Anaconda.AnacondaCore.RetrieveAcessCredentials();
            User currentUser = Cinderella.CinderellaCore.RetrieveCurrentUserInfo();

            //NavigationService.Navigate(new Uri("/Plugins/ProFX/ImageProcessingPage.xaml", UriKind.Relative));
            //return;

            if (tokenRetrieved && currentUser != null)
            {
                // Get the set list
                Anaconda.AnacondaCore.GetPhotoSetListAsync(currentUser.ResourceId);

                // Get group list
                Anaconda.AnacondaCore.GetGroupListAsync(currentUser.ResourceId);

                // Get the first page of current_user's photo stream
                if(PolicyKit.VioletPageSubscription == PolicyKit.MyStream)
                    Anaconda.AnacondaCore.GetPhotoStreamAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString()} });
                else if(PolicyKit.VioletPageSubscription == PolicyKit.DiscoveryStream)
                    Anaconda.AnacondaCore.GetDiscoveryStreamAsync(new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });
                else if (PolicyKit.VioletPageSubscription == PolicyKit.FavouriteStream)
                    Anaconda.AnacondaCore.GetFavouritePhotoStreamAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", PolicyKit.StreamItemsCountPerPage.ToString() } });

            }
            else
            {
                // Show the login page
                NavigationService.Navigate(new Uri("/Plugins/Login/LoginPage.xaml", UriKind.Relative));
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Reset dashboard selections
            Dashboard.ResetListSelections();

            base.OnNavigatedFrom(e);
        }

        private void OnTakePhotoClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/ProCamera/ProCameraPage.xaml", UriKind.Relative));
        }

        private void OnSubscriptionSettingsClick(object sender, EventArgs e)
        {
            SubscriptionSettingsView settingsView = new SubscriptionSettingsView();            
            var settingsDialog = ModalPopup.Show(settingsView, "Subscription", new List<string> {"Confirm", "Cancel" });
            settingsDialog.DismissWithButtonClick += (s, args) =>
            {
                int buttonIndex = (args as ModalPopupEventArgs).ButtonIndex;
                if (buttonIndex == 0)
                {
                    string subscriptionName = "";
                    if (settingsView.MyStreamButton.IsChecked == true)
                        subscriptionName = PolicyKit.MyStream;
                    else if (settingsView.DiscoveryStreamButton.IsChecked == true)
                        subscriptionName = PolicyKit.DiscoveryStream;
                    else if (settingsView.FavouriteStreamButton.IsChecked == true)
                        subscriptionName = PolicyKit.FavouriteStream;

                    if (subscriptionName != PolicyKit.VioletPageSubscription)
                    {
                        PolicyKit.VioletPageSubscription = subscriptionName;
                        PolicyChangedEventArgs policyArgs = new PolicyChangedEventArgs();
                        policyArgs.PolicyName = "VioletPageSubscription";
                        PolicyKit.SaveSettings();
                        PolicyKit.PolicyChanged.DispatchEvent(this, policyArgs);
                    }

                }
            };
        }

        private void OnSearchClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/Search/SearchPage.xaml", UriKind.Relative));
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}