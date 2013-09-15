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
using System.Windows.Input;
using Indulged.API.Avarice.Controls.SupportClasses;

namespace Indulged.Plugins.Group
{
    public partial class GroupJoinRequestView : UserControl, IModalPopupContent
    {
        public FlickrGroup Group { get; set; }

        // Reference to the modal popup container
        public ModalPopup PopupContainer { get; set; }

        private Indulged.API.Avarice.Controls.Button confirmButton;
        private Indulged.API.Avarice.Controls.Button cancelButton;
        public List<Indulged.API.Avarice.Controls.Button> Buttons { get; set; }

        // Constructor
        public GroupJoinRequestView()
        {
            InitializeComponent();

            Buttons = new List<API.Avarice.Controls.Button>();
            confirmButton = new API.Avarice.Controls.Button();
            confirmButton.Content = "Confirm";
            confirmButton.Click += (sender, e) =>
            {
                JoinGroup();
            };


            cancelButton = new API.Avarice.Controls.Button();
            cancelButton.Content = "Cancel";
            cancelButton.Click += (sender, e) =>
            {
                PopupContainer.Dismiss();
            };

            Buttons.Add(confirmButton);
        }

        public void OnPopupRemoved()
        {
            PopupContainer = null;
            Group = null;
        }

        private void JoinGroup()
        {
            string trimmedQueryString = MessageBox.Text.Trim();
            if (trimmedQueryString.Length <= 0)
                return;

            var statusView = new GroupJoiningRequestStatusView();
            statusView.Group = Group;
            statusView.Message = trimmedQueryString;
            statusView.PopupContainer = PopupContainer;

            PopupContainer.ReplaceContentWith("Group Membership Request", statusView, statusView.Buttons, () =>
            {
                statusView.BeginJoinGroupRequest();
            });

        }

        private void MessageBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string trimmedQueryString = MessageBox.Text.Trim();
            if (e.Key == Key.Enter && trimmedQueryString.Length > 0)
            {
                JoinGroup();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            JoinGroup();
        }
    }
}
