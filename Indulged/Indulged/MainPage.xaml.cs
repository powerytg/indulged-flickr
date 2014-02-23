using Indulged.API.Anaconda;
using Indulged.API.Avarice.Controls;
using Indulged.API.Avarice.Events;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.API.Utils;
using Indulged.Plugins.Common.PhotoRenderers;
using Indulged.Plugins.Dashboard;
using Indulged.Plugins.Dashboard.Events;
using Indulged.PolKit;
using Indulged.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Indulged
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Effects
            TiltEffect.TiltableItems.Add(typeof(FullPhotoRenderer));
            TiltEffect.TiltableItems.Add(typeof(MediumPhotoRenderer)); 

            // Events
            DashboardNavigator.DashboardPageChanged += DashboardPageChanged;

            // Retrieve policy settings
            PolicyKit.RetrieveSettings();

            // App bar
            ApplicationBar = GetPreludeAppBar();
        }

        private bool hasExecutedOnce = false;
        private bool hasLoggedIn = false;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.New)
            {
                Dashboard.OnNavigatingFromPage();
            }

            base.OnNavigatingFrom(e);
        }

         private void OnTakePhotoClick(object sender, EventArgs e)
        {
            ShowUploadOptions();
        }

         private void ShowUploadOptions()
         {
             var optionsView = new UploadOptionsView();
             var dialog = ModalPopup.Show(optionsView, AppResources.PreludeUploadSource, new List<string> { AppResources.GenericConfirmText, AppResources.GenericCancelText });
             dialog.DismissWithButtonClick += (s, args) =>
             {
                 int buttonIndex = (args as ModalPopupEventArgs).ButtonIndex;
                 if (buttonIndex == 0)
                 {
                     Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                     PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;

                     if (optionsView.CameraButton.IsChecked == true)
                     {
                         currentPage.NavigationService.Navigate(new Uri("/Plugins/ProCam/ImagePickerPage.xaml", UriKind.Relative));
                     }
                     else
                     {
                         currentPage.NavigationService.Navigate(new Uri("/Plugins/ProCam/ImagePickerPage.xaml?is_from_library=true", UriKind.Relative));
                     }
                 }
             };

         }

        // Subscription settings view
         private ModalPopup settingsDialog = null;

        private void OnSubscriptionSettingsClick(object sender, EventArgs e)
        {
            var settingsView = new SubscriptionSettingsView();            
            settingsDialog = ModalPopup.Show(settingsView, AppResources.MainPageSubscriptionTitleText, new List<string> {AppResources.GenericConfirmText, AppResources.GenericCancelText });
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

                settingsDialog = null;
            };
        }

        private void OnRefreshClick(object sender, EventArgs e)
        {
            Dashboard.RefreshPreludeStreams();
        }

        private void OnRefrehVioletClick(object sender, EventArgs e)
        {
            Dashboard.RefreshVioletStreams();
        }

        private void OnSettingsClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/Login/SettingsPage.xaml", UriKind.Relative));
        }

        private void OnFeedbackClick(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
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

        // Log out popup
        private ModalPopup logoutDialog = null;

        private void OnLogoutClick(object sender, EventArgs e)
        {
            logoutDialog = ModalPopup.Show(AppResources.MainPageLogoutContentText, AppResources.MainPageLogoutTitleText, new List<string> { AppResources.MainPageLogoutButtonText, AppResources.GenericCancelText });
            logoutDialog.DismissWithButtonClick += (s, args) => {
                int buttonIndex = (args as ModalPopupEventArgs).ButtonIndex;
                if (buttonIndex == 0)
                {
                    logoutDialog = null;

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
            {
                ApplicationBar = GetPreludeAppBar();
            }
            else if (e.SelectedPage.PageName == "VioletPage")
            {
                ApplicationBar = GetVioletAppBar();
            }
            else if (e.SelectedPage.PageName == "SummersaltPage")
                ApplicationBar = GetSummersaltAppBar();
        }

        private ApplicationBar GetPreludeAppBar()
        {
            ApplicationBar appBar = new ApplicationBar();
            appBar.IsMenuEnabled = true;
            appBar.Mode = ApplicationBarMode.Minimized;
            appBar.BackgroundColor = Colors.Black;
            appBar.ForegroundColor = Colors.White;
            appBar.Opacity = 0.9;

            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.MainPageAppBarRefreshText);
            appBarMenuItem.Click += OnRefreshClick;
            appBar.MenuItems.Add(appBarMenuItem);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.MainPageAppBarUploadPhotoText);
            appBarMenuItem.Click += OnTakePhotoClick;
            appBar.MenuItems.Add(appBarMenuItem);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.MainPageAppBarSearchText);
            appBarMenuItem.Click += OnSearchClick;
            appBar.MenuItems.Add(appBarMenuItem);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.MainPageAppBarSettingsText);
            appBarMenuItem.Click += OnSettingsClick;
            appBar.MenuItems.Add(appBarMenuItem);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.MainPageAppBarFeedbackText);
            appBarMenuItem.Click += OnFeedbackClick;
            appBar.MenuItems.Add(appBarMenuItem);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.MainPageAppBarLogoutText);
            appBarMenuItem.Click += OnLogoutClick;
            appBar.MenuItems.Add(appBarMenuItem);

            return appBar;
        }

        private ApplicationBar GetVioletAppBar()
        {
            ApplicationBar appBar = new ApplicationBar();
            appBar.IsMenuEnabled = true;
            appBar.Mode = ApplicationBarMode.Minimized;
            appBar.BackgroundColor = Colors.Black;
            appBar.ForegroundColor = Colors.White;
            appBar.Opacity = 0.9;

            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.MainPageAppBarRefreshText);
            appBarMenuItem.Click += OnRefreshClick;
            appBar.MenuItems.Add(appBarMenuItem);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.MainPageAppBarUploadPhotoText);
            appBarMenuItem.Click += OnTakePhotoClick;
            appBar.MenuItems.Add(appBarMenuItem);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.MainPageAppBarSubscriptionText);
            appBarMenuItem.Click += OnSubscriptionSettingsClick;
            appBar.MenuItems.Add(appBarMenuItem);
            return appBar;
        }

        private ApplicationBar GetSummersaltAppBar()
        {
            ApplicationBar appBar = new ApplicationBar();
            appBar.IsMenuEnabled = true;
            appBar.Mode = ApplicationBarMode.Minimized;
            appBar.BackgroundColor = Colors.Black;
            appBar.ForegroundColor = Colors.White;
            appBar.Opacity = 0.9;

            var appBarMenuItem = new ApplicationBarMenuItem(AppResources.MainPageAppBarMyProfileText);
            appBarMenuItem.Click += OnMyProfileClick;
            appBar.MenuItems.Add(appBarMenuItem);

            appBarMenuItem = new ApplicationBarMenuItem(AppResources.MainPageAppBarContactsText);
            appBarMenuItem.Click += OnContactsClick;
            appBar.MenuItems.Add(appBarMenuItem);

            return appBar;
        }
    }
}