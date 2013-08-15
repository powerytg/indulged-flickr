using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.ProFX
{
    public partial class FXSettingsStatusView : UserControl
    {
        public FXSettingsStatusView()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ImageProcessingPage.RequestDismissSettingsView(this, null);
        }
    }
}
