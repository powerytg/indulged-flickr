using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Indulged.Plugins.Dashboard.SummersaltRenderers
{
    public partial class SummersaltContactPhotoHeaderRenderer : UserControl
    {
        public SummersaltContactPhotoHeaderRenderer()
        {
            InitializeComponent();
        }

        private void ContactButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
            currentPage.NavigationService.Navigate(new Uri("/Plugins/Profile/ContactPage.xaml", UriKind.Relative));

        }
    }
}
