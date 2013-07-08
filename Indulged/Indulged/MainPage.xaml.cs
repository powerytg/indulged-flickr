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

using Indulged.API.Avarice.Controls;
using Indulged.API.Avarice.Events;
using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;

namespace Indulged
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Try to get credenticls
            bool tokenRetrieved = Anaconda.AnacondaCore.RetrieveAcessCredentials();
            User currentUser = Cinderella.CinderellaCore.RetrieveCurrentUserInfo();

            if (tokenRetrieved && currentUser != null)
            {
                // Get the set list
                Anaconda.AnacondaCore.GetPhotoSetListAsync();

                // Get the first page of current_user's photo stream
                Anaconda.AnacondaCore.GetPhotoStreamAsync(Cinderella.CinderellaCore.CurrentUser.ResourceId, new Dictionary<string, string> { {"page" , "1"}, {"per_page" , "100"} });
            }
            else
            {
                // Show the login page
                NavigationService.Navigate(new Uri("/Plugins/Login/LoginPage.xaml", UriKind.Relative));
            }

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