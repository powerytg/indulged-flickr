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
using Indulged.API.Cinderella;
using Indulged.API.Anaconda;
using Indulged.API.Avarice.Controls;

namespace Indulged.Plugins.Profile
{
    public partial class UserProfilePage : PhoneApplicationPage
    {
        // Constructor
        public UserProfilePage()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty UserSourceProperty = DependencyProperty.Register("UserSource", typeof(User), typeof(UserProfilePage), new PropertyMetadata(OnUserSourcePropertyChanged));

        public User UserSource
        {
            get
            {
                return (User)GetValue(UserSourceProperty);
            }
            set
            {
                SetValue(UserSourceProperty, value);
            }
        }

        public static void OnUserSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((UserProfilePage)sender).OnUserSourceChanged();
        }

        protected virtual void OnUserSourceChanged()
        {
            if (UserSource == null)
                return;

            PhotoPageView.UserSource = UserSource;
            InfoPageView.UserSource = UserSource;

            // Show loading progress indicator
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "loading photos";

            // Get first page of photos
            Anaconda.AnacondaCore.GetPhotoStreamAsync(UserSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

        private bool executedOnce;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
 	        base.OnNavigatedTo(e);

            if (executedOnce)
                return;

            executedOnce = true;

            string userId = NavigationContext.QueryString["user_id"];
            if (!Cinderella.CinderellaCore.UserCache.ContainsKey(userId))
                return;

            UserSource = Cinderella.CinderellaCore.UserCache[userId];

            // Title
            this.DataContext = UserSource;

        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            PhotoPageView.RemoveEventListeners();
            InfoPageView.RemoveEventListeners();

            UserSource = null;
            this.DataContext = null;

            base.OnRemovedFromJournal(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (ModalPopup.HasPopupHistory())
            {
                e.Cancel = true;
                ModalPopup.RemoveLastPopup();
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }


    }
}