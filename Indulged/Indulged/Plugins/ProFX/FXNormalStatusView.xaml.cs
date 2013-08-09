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
    public partial class FXNormalStatusView : UserControl
    {
        public FXNormalStatusView()
        {
            InitializeComponent();
        }

        private void TitleButton_Click(object sender, RoutedEventArgs e)
        {
            ImageProcessingPage.RequestFilterListView(this, null);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ImageProcessingPage.RequestFilterListView(this, null);
        }
    }
}
