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
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using System.Windows.Media.Imaging;
using Indulged.API.Cinderella.Models;
using System.Windows.Documents;
using Indulged.Resources;

namespace Indulged.Plugins.Dashboard.SummersaltRenderers
{
    public partial class SummersaltActivityPhotoRenderer : UserControl
    {
        public static readonly DependencyProperty ActivityProperty = DependencyProperty.Register("Activity", typeof(PhotoActivity), typeof(SummersaltActivityPhotoRenderer), new PropertyMetadata(OnActivityPropertyChanged));

        public PhotoActivity Activity
        {
            get
            {
                return (PhotoActivity)GetValue(ActivityProperty);
            }
            set
            {
                SetValue(ActivityProperty, value);
            }
        }

        public static void OnActivityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SummersaltActivityPhotoRenderer)sender).OnActivityChanged();
        }

        protected virtual void OnActivityChanged()
        {
            TitleLabel.Text = Activity.Title;
            ImageView.Source = new BitmapImage { UriSource = new Uri(Activity.TargetPhoto.GetImageUrl()), DecodePixelWidth = 480 };
            UpdateFavView();
        }

        private void UpdateFavView()
        {
            if(Activity.FavUsers.Count == 0){
                FavView.Visibility = Visibility.Collapsed;
                return;
            }
            else{
                FavView.Visibility = Visibility.Visible;
            }

            string userString = Activity.FavUsers[0].Name;
            string endString = AppResources.SummersaltAddedFavText;
            if (Activity.FavUsers.Count == 1)
            {
                userString += endString;
            }
            else if (Activity.FavUsers.Count == 2)
            {
                userString += AppResources.SummersaltAndText + Activity.FavUsers[1].Name + endString;
            }
            else if (Activity.FavUsers.Count == 3)
            {
                userString += " ," + Activity.FavUsers[1].Name + AppResources.SummersaltAndText + Activity.FavUsers[2].Name + endString;
            }
            else
            {
                int extCount = Activity.FavUsers.Count - 3;
                if (extCount == 1)
                    endString = AppResources.SummersaltSingleFavText;
                else
                    endString = AppResources.SummersaltAndText + extCount.ToString() + AppResources.SummersaltMultiFavText;

                userString += " ," + Activity.FavUsers[1].Name + ", " + Activity.FavUsers[2].Name + endString;
            }

            FavLabel.Text = userString;
        }

        // Constructor
        public SummersaltActivityPhotoRenderer()
        {
            InitializeComponent();
        }

        private void ImageView_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;

            string urlString = "/Plugins/Detail/DetailPage.xaml?photo_id=" + Activity.TargetPhoto.ResourceId;
            currentPage.NavigationService.Navigate(new Uri(urlString, UriKind.Relative));
        }


    }
}
