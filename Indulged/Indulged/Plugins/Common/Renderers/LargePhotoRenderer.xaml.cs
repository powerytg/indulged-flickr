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

namespace Indulged.Plugins.Common.Renderers
{
    public partial class LargePhotoRenderer : PhotoRendererBase
    {
        public LargePhotoRenderer()
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

            if (PhotoSource.Description.Length > 0)
            {
                DescriptionView.PhotoSource = PhotoSource;
                DescriptionView.Visibility = Visibility.Visible;
                TitleLabel.Visibility = Visibility.Collapsed;
            }
            else
            {
                TitleLabel.Visibility = Visibility.Visible;
                TitleLabel.Text = PhotoSource.Title;
                DescriptionView.Visibility = Visibility.Collapsed;
            }

            if (PhotoSource.ViewCount == 0)
            {
                StatView.Visibility = Visibility.Collapsed;
            }
            else
            {
                StatView.Visibility = Visibility.Visible;
                StatView.PhotoSource = PhotoSource;
            }

        }
    }
}