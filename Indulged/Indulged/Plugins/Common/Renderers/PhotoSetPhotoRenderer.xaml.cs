using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.PolKit;
using Indulged.Plugins.PhotoCollection;

namespace Indulged.Plugins.Common.Renderers
{
    public partial class PhotoSetPhotoRenderer : UserControl
    {
        public static readonly DependencyProperty SetPhotoProperty = DependencyProperty.Register("SetPhoto", typeof(PhotoSetPhoto), typeof(PhotoSetPhotoRenderer), new PropertyMetadata(OnSetPhotoPropertyChanged));

        public PhotoSetPhoto SetPhoto
        {
            get
            {
                return (PhotoSetPhoto)GetValue(SetPhotoProperty);
            }
            set
            {
                SetValue(SetPhotoProperty, value);
            }
        }

        public static void OnSetPhotoPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((PhotoSetPhotoRenderer)sender).OnSetPhotoChanged();
        }

        // Constructor
        public PhotoSetPhotoRenderer()
        {
            InitializeComponent();
        }

        protected void OnSetPhotoChanged()
        {
            ImageView.Source = new BitmapImage { UriSource = new Uri(SetPhoto.PhotoSource.GetImageUrl()), DecodePixelWidth = 300 };

            if (SetPhoto.PhotoSource.Title != null && SetPhoto.PhotoSource.Title.Length > 0)
            {
                TitleLabel.Text = SetPhoto.PhotoSource.Title;
                TitleLabel.Visibility = Visibility.Visible;
            }
            else
            {
                TitleLabel.Text = "Untitled";
                TitleLabel.Visibility = Visibility.Visible;
            }


            if (SetPhoto.PhotoSource.Description != null && SetPhoto.PhotoSource.Description.Length > 0)
            {
                DescriptionLabel.Text = SetPhoto.PhotoSource.Description;
                DescriptionLabel.Visibility = Visibility.Visible;
            }
            else
                DescriptionLabel.Visibility = Visibility.Collapsed;
        }

        protected override void OnTap(System.Windows.Input.GestureEventArgs e)
        {
            base.OnTap(e);

            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;

            string urlString = "/Plugins/Detail/DetailPage.xaml?photo_id=" + SetPhoto.PhotoSource.ResourceId + "&context=" + SetPhoto.PhotoSetId + "&context_type=PhotoSet";

            currentPage.NavigationService.Navigate(new Uri(urlString, UriKind.Relative));
        }
    }
}
