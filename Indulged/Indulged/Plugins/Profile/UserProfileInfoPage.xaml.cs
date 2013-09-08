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
using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;

namespace Indulged.Plugins.Profile
{
    public partial class UserProfileInfoPage : UserControl
    {
        public static readonly DependencyProperty UserSourceProperty = DependencyProperty.Register("UserSource", typeof(User), typeof(UserProfileInfoPage), new PropertyMetadata(OnUserSourcePropertyChanged));

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
            ((UserProfileInfoPage)sender).OnUserSourceChanged();
        }

        protected virtual void OnUserSourceChanged()
        {
            if (UserSource.IsFullInfoLoaded)
            {
                UpdateInfoView();
            }
            else
            {
                StatusLabel.Visibility = Visibility.Visible;
                Anaconda.AnacondaCore.GetUserInfoAsync(UserSource.ResourceId);
            }
        }

        private void UpdateInfoView()
        {
            StatusLabel.Visibility = Visibility.Collapsed;
            ContentView.Visibility = Visibility.Visible;

            AvatarView.Source = new BitmapImage(new Uri(UserSource.AvatarUrl));
            NameLabel.Text = UserSource.Name;

            if (UserSource.IsProUser)
                ProLabel.Visibility = Visibility.Visible;
            else
                ProLabel.Visibility = Visibility.Collapsed;

            if (UserSource.RealName != null && UserSource.RealName.Length > 0)
            {
                RealNameSection.Visibility = Visibility.Visible;
                RealNameLabel.Text = UserSource.RealName;
            }
            else
            {
                RealNameSection.Visibility = Visibility.Collapsed;
            }

            if (UserSource.Location != null && UserSource.Location.Length > 0)
            {
                LocationSection.Visibility = Visibility.Visible;
                LocationLabel.Text = UserSource.Location;
            }
            else
            {
                LocationSection.Visibility = Visibility.Collapsed;
            }

            if (UserSource.Description != null && UserSource.Description.Length > 0)
            {
                DescSection.Visibility = Visibility.Visible;
                DescLabel.Text = UserSource.Description;
            }
            else
            {
                DescSection.Visibility = Visibility.Collapsed;
            }

            if (UserSource.ProfileUrl != null && UserSource.ProfileUrl.Length > 0)
            {
                ProfileUrlSection.Visibility = Visibility.Visible;
                ProfileUrlLabel.Text = UserSource.ProfileUrl;
            }
            else
            {
                ProfileUrlSection.Visibility = Visibility.Collapsed;
            }

            if (UserSource.PhotoCount == 0)
                PhotoCountLabel.Text = "No photo yet";
            else
                PhotoCountLabel.Text = UserSource.PhotoCount.ToString();
        }

        // Constructor
        public UserProfileInfoPage()
        {
            InitializeComponent();

            // Events
            Cinderella.CinderellaCore.UserInfoUpdated += OnUserProfileUpdated;
        }

        private void OnUserProfileUpdated(object sender, UserInfoUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.UserId != UserSource.ResourceId)
                    return;

                UpdateInfoView();
            });
        }

        private void ProfileUrlLabel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask wbTask = new WebBrowserTask();
            wbTask.Uri = new Uri(UserSource.ProfileUrl);
            wbTask.Show();
        }

    }
}
