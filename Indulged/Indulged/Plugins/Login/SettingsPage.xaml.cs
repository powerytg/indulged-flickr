using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Utils;
using Indulged.PolKit;
using Indulged.Plugins.Chrome;
using Indulged.API.Avarice.Controls;

namespace Indulged.Plugins.Login
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        // Constructor
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ProCamSwitch.IsChecked = PolicyKit.ShouldUseProCamera;
            BlurBackgroundSwitch.IsChecked = PolicyKit.ShouldUseBlurredBackground;

            if (ThemeManager.CurrentTheme == Themes.Dark)
                FXBackgroundPicker.SelectedIndex = 0;
            else
                FXBackgroundPicker.SelectedIndex = 1;

            if (PolicyKit.VioletPageSubscription == PolicyKit.MyStream)
                VioletPicker.SelectedIndex = 0;
            else if (PolicyKit.VioletPageSubscription == PolicyKit.FavouriteStream)
                VioletPicker.SelectedIndex = 1;
            else if (PolicyKit.VioletPageSubscription == PolicyKit.DiscoveryStream)
                VioletPicker.SelectedIndex = 2;
        }

        private void ProCamSwitch_Checked(object sender, RoutedEventArgs e)
        {
            PolicyKit.ShouldUseProCamera = true;
            PolicyKit.SaveSettings();
        }

        private void ProCamSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            PolicyKit.ShouldUseProCamera = false;
            PolicyKit.SaveSettings();
        }

        private void BlurBackgroundSwitch_Checked(object sender, RoutedEventArgs e)
        {
            PolicyKit.ShouldUseBlurredBackground = true;
            PolicyKit.SaveSettings();
        }

        private void BlurBackgroundSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            PolicyKit.ShouldUseBlurredBackground = false;
            PolicyKit.SaveSettings();
        }

        private void FXBackgroundPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FXBackgroundPicker == null)
                return;

            if (FXBackgroundPicker.SelectedIndex == 0)
                ThemeManager.CurrentTheme = Themes.Dark;
            else
                ThemeManager.CurrentTheme = Themes.Light;

            PolicyKit.SaveSettings();
        }

        private void VioletPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VioletPicker == null)
                return;

            string subscriptionName = "";
            if (VioletPicker.SelectedIndex == 0)
                subscriptionName = PolicyKit.MyStream;
            else if (VioletPicker.SelectedIndex == 1)
                subscriptionName = PolicyKit.FavouriteStream;
            else if (VioletPicker.SelectedIndex == 2)
                subscriptionName = PolicyKit.DiscoveryStream;

            if (subscriptionName != PolicyKit.VioletPageSubscription)
            {
                PolicyKit.VioletPageSubscription = subscriptionName;
                PolicyChangedEventArgs policyArgs = new PolicyChangedEventArgs();
                policyArgs.PolicyName = "VioletPageSubscription";
                PolicyKit.SaveSettings();
                PolicyKit.PolicyChanged.DispatchEvent(this, policyArgs);
            }
        }


    }
}