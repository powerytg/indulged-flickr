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
using Indulged.API.Avarice.Controls;
using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;

namespace Indulged.Plugins.Group
{
    public partial class GroupJoiningStatusView : UserControl
    {
        public FlickrGroup Group { get; set; }

        // Reference to the modal popup container
        public ModalPopup PopupContainer { get; set; }

        private Indulged.API.Avarice.Controls.Button doneButton;
        public List<Indulged.API.Avarice.Controls.Button> Buttons { get; set; }

        public void BeginJoinGroup()
        {
            StatusLabel.Text = "Sending request";
            doneButton.IsEnabled = false;

            if(Group.Rules != null && Group.Rules.Length > 0)
                Anaconda.AnacondaCore.JoinGroupAsync(Group.ResourceId, new Dictionary<string, string> { {"accept_rules", "1" }});
            else
                Anaconda.AnacondaCore.JoinGroupAsync(Group.ResourceId);
        }

        // Constructor
        public GroupJoiningStatusView()
        {
            InitializeComponent();

            Buttons = new List<API.Avarice.Controls.Button>();
            doneButton = new API.Avarice.Controls.Button();
            doneButton.Content = "Done";
            doneButton.Click += (sender, e) =>
            {
                PopupContainer.Dismiss();
            };

            Buttons.Add(doneButton);

            // Events
            Anaconda.AnacondaCore.JoinGroupException += OnJoinGroupException;
            Cinderella.CinderellaCore.JoinGroupComplete += OnGroupJoined;
        }

        private void OnJoinGroupException(object sender, JoinGroupExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.GroupId != Group.ResourceId)
                    return;

                ProgressView.Visibility = Visibility.Collapsed;
                StatusLabel.Text = e.Message;
                doneButton.IsEnabled = true;
            });
        }

        private void OnGroupJoined(object sender, GroupJoinedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.GroupId != Group.ResourceId)
                    return;

                PopupContainer.DismissWithAction(() =>
                {
                    Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                    PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
                    currentPage.NavigationService.Navigate(new Uri("/Plugins/Group/GroupPage.xaml?group_id=" + Group.ResourceId, UriKind.Relative));
                });
            });
        }
    }
}
