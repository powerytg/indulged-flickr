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
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Events;
using Indulged.API.Avarice.Controls;
using System.Windows.Media.Imaging;
using Indulged.API.Avarice.Events;
using Indulged.API.Avarice.Controls.SupportClasses;
using Indulged.Resources;

namespace Indulged.Plugins.Group
{
    public partial class GroupInfoView : UserControl, IModalPopupContent
    {
        private Indulged.API.Avarice.Controls.Button joinButton;
        private Indulged.API.Avarice.Controls.Button browseButton;
        private Indulged.API.Avarice.Controls.Button doneButton;
        private ModalPopup _popupContainer;

        public void ShowAsModal()
        {
            joinButton = new API.Avarice.Controls.Button();
            joinButton.Content = AppResources.GroupJoinText;
            joinButton.Click += (sender, e) => {
                JoinGroup();
            };

            browseButton = new API.Avarice.Controls.Button();
            browseButton.Content = AppResources.GroupBrowseText;
            browseButton.Click += (sender, e) => {
                BrowseGroup();
            };

            doneButton = new API.Avarice.Controls.Button();
            doneButton.Content = AppResources.GenericDoneText;
            doneButton.Click += (sender, e) => {
                _popupContainer.Dismiss();
            };

            if (!_group.IsInfoRetrieved)
            {
                joinButton.IsEnabled = false;
                browseButton.IsEnabled = false;
            }

            _popupContainer = ModalPopup.ShowWithButtons(this, _group.Name, new List<Indulged.API.Avarice.Controls.Button> { browseButton, joinButton, doneButton }, false);            
        }

        private FlickrGroup _group;
        public FlickrGroup Group 
        {
            get
            {
                return _group;
            }
            set
            {
                if(_group != value)
                    _group = value;

                if(_group != null)
                    OnGroupChanged();
            }
        }

        // Constructor
        public GroupInfoView()
        {
            InitializeComponent();

            // Events
            Cinderella.CinderellaCore.GroupInfoUpdated += OnGroupInfoUpdated;
        }

        public void OnPopupRemoved()
        {
            Cinderella.CinderellaCore.GroupInfoUpdated -= OnGroupInfoUpdated;
        }

        private void OnGroupInfoUpdated(object sender, GroupInfoUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (_group == null)
                    return;

                if (_group.ResourceId != e.GroupId)
                    return;

                joinButton.IsEnabled = true;
                browseButton.IsEnabled = true;

                UpdateDisplayListAndHideLoadingView();
            });
        }

        private void OnGroupChanged()
        {
            if (_group.IsInfoRetrieved)
            {
                UpdateDisplayListAndHideLoadingView();
            }
            else
            {
                Anaconda.AnacondaCore.GetGroupInfoAsync(_group.ResourceId);
                ShowLoadingView();
            }
        }

        private void ShowLoadingView()
        {
            LoadingView.Visibility = Visibility.Visible;
            DescriptionView.Visibility = Visibility.Collapsed;
        }

        private void UpdateDisplayListAndHideLoadingView()
        {
            LoadingView.Visibility = Visibility.Collapsed;
            DescriptionView.Visibility = Visibility.Visible;

            DescriptionLabel.Text = _group.Description;

            if (_group.ThrottleMode == "none")
            {
                ThrottleIconView.Source = new BitmapImage(new Uri("/Assets/Group/NoThrottleIcon.png", UriKind.Relative));
                ThrottleDescriptionView.Text = "No upload limit";
            }
            else
            {
                ThrottleIconView.Source = new BitmapImage(new Uri("/Assets/Group/ThrottleIcon.png", UriKind.Relative));
                ThrottleDescriptionView.Text = "Upload limit: " + _group.ThrottleMaxCount.ToString() + " per " + _group.ThrottleMode;

            }
        }

        private void BrowseGroup()
        {
            _popupContainer.DismissWithAction(() => {
                Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
                PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
                currentPage.NavigationService.Navigate(new Uri("/Plugins/Group/GroupPage.xaml?group_id=" + _group.ResourceId, UriKind.Relative));
            });
        }

        private void JoinGroup()
        {
            if (Group.Rules != null && Group.Rules.Length > 0)
            {
                var rulesView = new GroupRulesView();
                rulesView.GroupSource = Group;
                rulesView.PopupContainer = _popupContainer;

                _popupContainer.ReplaceContentWith(AppResources.GroupRulesText, rulesView, rulesView.Buttons);
            }
            else
            {
                if (Group.IsInvitationOnly)
                {
                    var requestView = new GroupJoinRequestView();
                    requestView.Group = Group;
                    requestView.PopupContainer = _popupContainer;

                    _popupContainer.ReplaceContentWith(AppResources.GroupInvitationRequestText, requestView, requestView.Buttons);
                }
                else
                {
                    var statusView = new GroupJoiningStatusView();
                    statusView.Group = Group;
                    statusView.PopupContainer = _popupContainer;

                    _popupContainer.ReplaceContentWith(AppResources.GroupJoiningText, statusView, statusView.Buttons, () =>
                    {
                        statusView.BeginJoinGroup();
                    });
                }
            }
        }
    }
}
