using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

using Indulged.API.Cinderella.Models;
using Indulged.Plugins.Common.PhotoGroupRenderers;
using Indulged.API.Cinderella;

namespace Indulged.Plugins.PhotoCollection.Renderers
{
    public partial class PrimaryRenderer : CommonPhotoGroupRendererBase
    {
        // Constructor
        public PrimaryRenderer()
            : base()
        {
            InitializeComponent();
        }

        protected override void OnPhotoGroupSourceChanged()
        {
            base.OnPhotoGroupSourceChanged();

            PhotoSet photoset = Cinderella.CinderellaCore.PhotoSetCache[PhotoGroupSource.context];
            TitleView.Text = photoset.Name;
            StatLabel.Text = photoset.PhotoCount.ToString() + " items";
            DateView.Text = "Created at " + photoset.CreationDate.ToString("MMM dd, yyyy");

            if (photoset.Description.Length > 0)
            {
                DescView.Text = photoset.Description;
            }
            else
            {
                DescView.Text = "No description available. You can tap on the edit description button to add content.";
            }

            ImageView.Source = new BitmapImage { UriSource = new Uri(photoset.PrimaryPhoto.GetImageUrl()), DecodePixelWidth = 640 };
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoSetPage.RequestAddPhotoDialog(this, null);
        }

        private void CameraButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoSetPage.RequestCamera(this, null);
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoSetPage.RequestUpload(this, null);
        }

        private void ChangePrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoSetPage.RequestChangePrimaryPhoto(this, null);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoSetPage.RequestEditProperties(this, null);
        }

        private void EditDescriptionButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoSetPage.RequestEditProperties(this, null);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoSetPage.RequestDeletePhotoSet(this, null);
        }
    }
}
