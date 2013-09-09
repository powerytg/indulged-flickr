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
using System.Collections.ObjectModel;
using Indulged.API.Cinderella;
using Indulged.API.Anaconda;
using Indulged.API.Cinderella.Events;

namespace Indulged.Plugins.Profile
{
    public partial class ContactPage : PhoneApplicationPage
    {
        // Constructor
        public ContactPage()
        {
            InitializeComponent();
        }

        // Reply data source
        public ObservableCollection<User> UserCollection { get; set; }

        private bool executedOnce;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (executedOnce)
                return;

            executedOnce = true;
           
            // Reply list
            UserCollection = new ObservableCollection<User>();
            ContactListView.ItemsSource = UserCollection;
            
            foreach (var contact in Cinderella.CinderellaCore.ContactList)
            {
                UserCollection.Add(contact);
            }

            // Events
            Cinderella.CinderellaCore.ContactListUpdated += OnContactListUpdated;

            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.Text = "retrieving contacts";
            SystemTray.ProgressIndicator.IsVisible = true;

            // Refresh reply list
            Anaconda.AnacondaCore.GetContactListAsync(1, Anaconda.DefaultItemsPerPage);
        }

        private void OnContactListUpdated(object sender, ContactListUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                SystemTray.ProgressIndicator.IsVisible = false;

                if (Cinderella.CinderellaCore.ContactList.Count == 0)
                {
                    StatusLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    StatusLabel.Visibility = Visibility.Collapsed;
                }

                foreach (var contact in e.NewUsers)
                {
                    if(!UserCollection.Contains(contact))
                        UserCollection.Add(contact);
                }
            });
        }

        private void ContactListView_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            User contact = e.Container.Content as User;
            if (contact == null)
                return;

            int index = UserCollection.IndexOf(contact);

            bool canLoad = (Cinderella.CinderellaCore.ContactList.Count < Cinderella.CinderellaCore.ContactCount);
            if (UserCollection.Count - index <= 2 && canLoad)
            {
                SystemTray.ProgressIndicator.Text = "retrieving contacts";
                SystemTray.ProgressIndicator.IsVisible = true;

                int page = Cinderella.CinderellaCore.ContactList.Count / Anaconda.DefaultItemsPerPage + 1;
                Anaconda.AnacondaCore.GetContactListAsync(page, Anaconda.DefaultItemsPerPage);
            }
        }
    }
}