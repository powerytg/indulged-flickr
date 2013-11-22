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
using Indulged.API.Avarice.Controls.SupportClasses;
using Indulged.Resources;

namespace Indulged.Plugins.Group
{
    public partial class GroupJoiningRequestStatusView : UserControl, IModalPopupContent
    {
        public FlickrGroup Group { get; set; }
        public string Message { get; set; }

        // Reference to the modal popup container
        public ModalPopup PopupContainer { get; set; }

        private Indulged.API.Avarice.Controls.Button doneButton;
        public List<Indulged.API.Avarice.Controls.Button> Buttons { get; set; }

        public void BeginJoinGroupRequest()
        {
            StatusLabel.Text = AppResources.GroupSendingRequestText;
            doneButton.IsEnabled = false;

            if(Group.Rules != null && Group.Rules.Length > 0)
                Anaconda.AnacondaCore.SendJoinGroupRequestAsync(Group.ResourceId, Message, new Dictionary<string, string> { {"accept_rules", "1" }});
            else
                Anaconda.AnacondaCore.SendJoinGroupRequestAsync(Group.ResourceId, Message);
        }

        public void OnPopupRemoved()
        {
            Anaconda.AnacondaCore.JoinGroupRequestException -= OnJoinGroupException;
            Anaconda.AnacondaCore.JoinGroupRequestComplete -= OnJoinGroupComplete;

            Group = null;
            Message = null;
            PopupContainer = null;
        }

        // Constructor
        public GroupJoiningRequestStatusView()
        {
            InitializeComponent();

            Buttons = new List<API.Avarice.Controls.Button>();
            doneButton = new API.Avarice.Controls.Button();
            doneButton.Content = AppResources.GenericDoneText;
            doneButton.Click += (sender, e) =>
            {
                PopupContainer.Dismiss();
            };

            Buttons.Add(doneButton);

            // Events
            Anaconda.AnacondaCore.JoinGroupRequestException += OnJoinGroupException;
            Anaconda.AnacondaCore.JoinGroupRequestComplete += OnJoinGroupComplete;
        }

        private void OnJoinGroupException(object sender, JoinGroupRequestExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.GroupId != Group.ResourceId)
                    return;

                ProgressView.Visibility = Visibility.Collapsed;
                StatusLabel.Text = e.Message;
                doneButton.IsEnabled = true;
            });
        }

        private void OnJoinGroupComplete(object sender, JoinGroupRequestEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.GroupId != Group.ResourceId)
                    return;

                ProgressView.Visibility = Visibility.Collapsed;
                StatusLabel.Text = AppResources.GroupRequestSentText;
                doneButton.IsEnabled = true;

            });
        }
    }
}
