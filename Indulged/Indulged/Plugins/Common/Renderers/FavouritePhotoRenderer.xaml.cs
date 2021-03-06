﻿using System;
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
using Indulged.Plugins.Detail;
using Indulged.API.Avarice.Controls;

namespace Indulged.Plugins.Common.Renderers
{
    public partial class FavouritePhotoRenderer : PhotoRendererBase
    {
        public FavouritePhotoRenderer()
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
            ImageView.Source = new BitmapImage { UriSource = new Uri(PhotoSource.GetImageUrl()), DecodePixelWidth = 480 };

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
        }

        protected override void OnTap(System.Windows.Input.GestureEventArgs e)
        {
            // Ignore
            return;

        }

        private void UnfavButton_Click(object sender, RoutedEventArgs e)
        {
            var statusView = new FavStatusView();
            statusView.PhotoSource = PhotoSource;
            var popupContainer = ModalPopup.ShowWithButtons(statusView, "Favourite", statusView.Buttons, false);
            statusView.PopupContainer = popupContainer;
            statusView.PassInfoDetection = true;
            statusView.BeginFavRequest();

        }

        private void ImageView_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;

            // Get photo collection context
            string collectionContext = PolicyKit.FavouriteStream;

            currentPage.NavigationService.Navigate(new Uri("/Plugins/Detail/DetailPage.xaml?photo_id=" + PhotoSource.ResourceId + "&context=" + collectionContext, UriKind.Relative));

        }
    }
}
