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

namespace Indulged.Plugins.Dashboard.SummersaltRenderers
{
    public partial class SummersaltContactHeaderRenderer : UserControl
    {
        public static readonly DependencyProperty UserSourceProperty = DependencyProperty.Register("UserSource", typeof(User), typeof(SummersaltContactHeaderRenderer), new PropertyMetadata(OnUserSourcePropertyChanged));

        public User UserSource
        {
            get
            {
                return (User)GetValue(UserSourceProperty);
            }
            set
            {
                SetValue(UserSourceProperty, value);
            }
        }

        public static void OnUserSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SummersaltContactHeaderRenderer)sender).OnUserSourceChanged();

        }

        protected virtual void OnUserSourceChanged()
        {
            TitleLabel.Text = "By " + UserSource.Name;
        }

        // Constructor
        public SummersaltContactHeaderRenderer()
        {
            InitializeComponent();
        }

        private void TitleLabel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
            currentPage.NavigationService.Navigate(new Uri("/Plugins/Profile/UserProfilePage.xaml?user_id=" + UserSource.ResourceId, UriKind.Relative));

        }

    }
}
