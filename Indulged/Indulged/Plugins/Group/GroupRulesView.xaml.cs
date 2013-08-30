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

namespace Indulged.Plugins.Group
{
    public partial class GroupRulesView : UserControl
    {
        public static readonly DependencyProperty GroupSourceProperty = DependencyProperty.Register("GroupSource", typeof(FlickrGroup), typeof(GroupRulesView), new PropertyMetadata(OnGroupSourcePropertyChanged));

        public FlickrGroup GroupSource
        {
            get
            {
                return (FlickrGroup)GetValue(GroupSourceProperty);
            }
            set
            {
                SetValue(GroupSourceProperty, value);
            }
        }

        public static void OnGroupSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((GroupRulesView)sender).OnGroupSourceChanged();
        }

        protected virtual void OnGroupSourceChanged()
        {
            RulesLabel.Text = GroupSource.Rules;
        }

        // Reference to the popup container
        public ModalPopup PopupContainer { get; set; }

        private Indulged.API.Avarice.Controls.Button joinButton;
        private Indulged.API.Avarice.Controls.Button cancelButton;
        public List<Indulged.API.Avarice.Controls.Button> Buttons { get; set; }

        // Constructor
        public GroupRulesView()
        {
            InitializeComponent();

            joinButton = new API.Avarice.Controls.Button();
            joinButton.Content = "I Agree";
            joinButton.Click += (sender, e) => {
                if (GroupSource.IsInvitationOnly)
                {
                    var requestView = new GroupJoinRequestView();
                    requestView.Group = GroupSource;
                    requestView.PopupContainer = PopupContainer;

                    PopupContainer.ReplaceContentWith("Invitation Request", requestView, requestView.Buttons);
                }
                else
                {
                    var statusView = new GroupJoiningStatusView();
                    statusView.Group = GroupSource;
                    statusView.PopupContainer = PopupContainer;

                    PopupContainer.ReplaceContentWith("Joinning Group", statusView, statusView.Buttons, () =>
                    {
                        statusView.BeginJoinGroup();
                    });
                }

            };

            cancelButton = new API.Avarice.Controls.Button();
            cancelButton.Content = "Cancel";
            cancelButton.Click += (sender, e) =>
            {
                PopupContainer.Dismiss();
            };

            Buttons = new List<API.Avarice.Controls.Button>();
            Buttons.Add(joinButton);
            Buttons.Add(cancelButton);
        }
    }
}
