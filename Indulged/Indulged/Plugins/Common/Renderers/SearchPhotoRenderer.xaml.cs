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

namespace Indulged.Plugins.Common.Renderers
{
    public partial class SearchPhotoRenderer : PhotoRendererBase
    {
        public SearchPhotoRenderer()
        {
            InitializeComponent();
        }

        protected override Image GetImagePresenter()
        {
            return ImageView;
        }

        protected override void OnPhotoSourceChanged()
        {
            base.OnPhotoSourceChanged();
            ImageView.Source = new BitmapImage(new Uri(PhotoSource.GetImageUrl()));

            if (PhotoSource.Title != null && PhotoSource.Title.Length > 0)
            {
                TitleLabel.Text = PhotoSource.Title;
                TitleLabel.Visibility = Visibility.Visible;
            }
            else
            {
                TitleLabel.Text = "Untitled";
                TitleLabel.Visibility = Visibility.Visible;
            }


            if (PhotoSource.Description != null && PhotoSource.Description.Length > 0)
            {
                DescriptionLabel.Text = PhotoSource.Description;
                DescriptionLabel.Visibility = Visibility.Visible;
            }
            else
                DescriptionLabel.Visibility = Visibility.Collapsed;

            BitmapImage src = new BitmapImage(new Uri(PhotoSource.GetImageUrl()));
            src.DecodePixelWidth = 400;
            ImageView.Source = src;
        }

        protected override void OnTap(System.Windows.Input.GestureEventArgs e)
        {
            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;

            // Get photo collection context
            string collectionContext = PolicyKit.VioletPageSubscription;

            currentPage.NavigationService.Navigate(new Uri("/Plugins/Detail/DetailPage.xaml?photo_id=" + PhotoSource.ResourceId, UriKind.Relative));
        }
    }
}
