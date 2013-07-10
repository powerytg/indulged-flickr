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
    public partial class HeadlinePhotoRenderer : PhotoRendererBase
    {
        public HeadlinePhotoRenderer()
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

            TitleLabel.Text = "Cover Photo: " + PhotoSource.Title;
            DescriptionLabel.Text = PhotoSource.Description;
        }
    }
}