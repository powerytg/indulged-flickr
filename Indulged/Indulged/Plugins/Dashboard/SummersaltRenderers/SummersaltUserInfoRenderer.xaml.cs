using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using System.Windows.Media.Imaging;
using Indulged.API.Cinderella.Models;
using System.Windows.Documents;

namespace Indulged.Plugins.Dashboard.SummersaltRenderers
{
    public partial class SummersaltUserInfoRenderer : UserControl
    {
        private User currentUser;

        // Constructor
        public SummersaltUserInfoRenderer()
        {
            InitializeComponent();

            if (Cinderella.CinderellaCore.CurrentUser.IsFullInfoLoaded)
                UpdateInfoView();

            // Events
            Cinderella.CinderellaCore.UserInfoUpdated += OnUserInfoUpdated;
        }

        private void OnUserInfoUpdated(object sender, UserInfoUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.UserId != Cinderella.CinderellaCore.CurrentUser.ResourceId)
                    return;

                UpdateInfoView();
            });
        }

        private void UpdateInfoView()
        {
            LoadingView.Visibility = Visibility.Collapsed;
            ContentView.Visibility = Visibility.Visible;

            currentUser = Cinderella.CinderellaCore.CurrentUser;
            AvatarView.Source = new BitmapImage(new Uri(currentUser.AvatarUrl));
            NameLabel.Text = currentUser.UserName;
            ProLabel.Visibility = currentUser.IsProUser ? Visibility.Visible : Visibility.Collapsed;

            // Description label
            if (currentUser.Description != null && currentUser.Description.Length > 0)
            {
                formatUserInfoText(currentUser.Description);
            }
            else
            {
                if (currentUser.hasFirstDate)
                    formatUserInfoText("Member since " + currentUser.FirstDate.ToShortDateString());
                else if (currentUser.PhotoCount > 0)
                    formatUserInfoText("Photo uploaded: " + currentUser.PhotoCount.ToString());
                else
                    formatUserInfoText("No extra info available");
            }
        }

        private void formatUserInfoText(string text)
        {
            DescLabel.Inlines.Clear();

            if (text.Length <= 1)
                return;

            Run initialRun = new Run();
            initialRun.FontWeight = FontWeights.Bold;
            initialRun.FontSize = 72;
            initialRun.Text = text.Substring(0, 1);
            DescLabel.Inlines.Add(initialRun);

            Run bodyRun = new Run();
            bodyRun.FontWeight = FontWeights.Light;
            bodyRun.FontSize = 30;
            bodyRun.Text = text.Substring(1);
            DescLabel.Inlines.Add(bodyRun);
        }

    }
}
