using System;
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
using Indulged.Plugins.Dashboard.Events;

namespace Indulged
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Events
            DashboardNavigator.DashboardPageChanged += DashboardPageChanged;

            // Retrieve policy settings
            PolicyKit.RetrieveSettings();
        }

        private bool hasExecutedOnce = false;
        private bool hasLoggedIn = false;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(hasExecutedOnce)
                Dashboard.OnNavigatedToPage();

            if (hasExecutedOnce && hasLoggedIn)
                return;

            hasExecutedOnce = true;

            // Try to get credenticls
            bool tokenRetrieved = Anaconda.AnacondaCore.RetrieveAcessCredentials();
            User currentUser = Cinderella.CinderellaCore.RetrieveCurrentUserInfo();

            if (tokenRetrieved && currentUser != null)
            {
                hasLoggedIn = true;

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
                NavigationService.RemoveBackEntry();
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Reset dashboard selections
            Dashboard.OnNavigatedFromPage();
            

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

        private void OnRefreshClick(object sender, EventArgs e)
        {
            Dashboard.RefreshPreludeStreams();
        }

        private void OnSettingsClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/Login/SettingsPage.xaml", UriKind.Relative));
        }

        private void OnSearchClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/Search/SearchPage.xaml", UriKind.Relative));
        }

        private void OnMyProfileClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/Profile/UserProfilePage.xaml?user_id=" + Cinderella.CinderellaCore.CurrentUser.ResourceId, UriKind.Relative));
        }

        private void OnContactsClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/Profile/ContactPage.xaml", UriKind.Relative));
        }

        private void OnLogoutClick(object sender, EventArgs e)
        {
            var logoutDialog = ModalPopup.Show("You will be taken back to login screen and your privacy will be cleared out on this device", "Sign Out", new List<string> { "Sign Out", "Cancel" });
            logoutDialog.DismissWithButtonClick += (s, args) => {
                int buttonIndex = (args as ModalPopupEventArgs).ButtonIndex;
                if (buttonIndex == 0)
                {
                    // Clear caches
                    Cinderella.CinderellaCore.SignOut();

                    NavigationService.Navigate(new Uri("/Plugins/Login/LoginPage.xaml", UriKind.Relative));
                    NavigationService.RemoveBackEntry();
                }
            };
            
        }

        // Navigator changed event
        private void DashboardPageChanged(object sender, DashboardPageEventArgs e)
        {
            if (e.SelectedPage.PageName == "PreludePage")
                ApplicationBar = Resources["PreludeAppBar"] as ApplicationBar;
            else if (e.SelectedPage.PageName == "VioletPage")
                ApplicationBar = Resources["VioletAppBar"] as ApplicationBar;
            else if (e.SelectedPage.PageName == "SummersaltPage")
                ApplicationBar = Resources["SummersaltAppBar"] as ApplicationBar;

        }
    }
}