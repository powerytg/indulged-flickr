using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Models;
using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;

namespace Indulged.Plugins.Detail
{
    public partial class PhotoEXIFView : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(PhotoEXIFView), new PropertyMetadata(OnPhotoSourcePropertyChanged));

        public Photo PhotoSource
        {
            get
            {
                return (Photo)GetValue(PhotoSourceProperty);
            }
            set
            {
                SetValue(PhotoSourceProperty, value);
            }
        }

        public static void OnPhotoSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((PhotoEXIFView)sender).OnPhotoSourceChanged();
        }

        protected virtual void OnPhotoSourceChanged()
        {
            if (PhotoSource.EXIF != null)
            {
                LoadingView.Visibility = Visibility.Collapsed;
                DescriptionLabel.Visibility = Visibility.Visible;
                DescriptionLabel.Text = this.GetEXIFString();
            }
            else
            {
                LoadingView.Visibility = Visibility.Visible;
                DescriptionLabel.Visibility = Visibility.Collapsed;
            }
        }

        // Constructor
        public PhotoEXIFView()
        {
            InitializeComponent();

            // Events
            Cinderella.CinderellaCore.EXIFUpdated += OnEXIFUpdated;
            Anaconda.AnacondaCore.EXIFException += OnEXIFException;
        }

        private void OnEXIFUpdated(object sender, EXIFUpdatedEventArgs e)
        {
            if(e.PhotoId != PhotoSource.ResourceId)
                return;

             LoadingView.Visibility = Visibility.Collapsed;
             DescriptionLabel.Visibility = Visibility.Visible;
             DescriptionLabel.Text = this.GetEXIFString();
        }

        private void OnEXIFException(object sender, GetEXIFExceptionEventArgs e)
        {
            if (e.PhotoId != PhotoSource.ResourceId)
                return;

            LoadingView.Visibility = Visibility.Collapsed;
            DescriptionLabel.Visibility = Visibility.Visible;
            DescriptionLabel.Text = "Cannot retrieve EXIF Info";
        }

        private string GetEXIFString()
        {
            if (PhotoSource.EXIF.Count == 0)
            {
                return "EXIF data is not available";
            }

            string result = "";

            if(PhotoSource.EXIF.ContainsKey("Model"))
                result = "This photo was taken with " + PhotoSource.EXIF["Model"] + ". ";

            if (PhotoSource.EXIF.ContainsKey("Aperture"))
                result += "Aperture was " + PhotoSource.EXIF["Aperture"] + ". ";

            if (PhotoSource.EXIF.ContainsKey("Exposure Program"))
                result += "Exposure mode was " + PhotoSource.EXIF["Exposure Program"] + ". ";

            if (PhotoSource.EXIF.ContainsKey("ISO Speed") && PhotoSource.EXIF.ContainsKey("Exposure"))
                result += "ISO  " + PhotoSource.EXIF["ISO Speed"] + " at " + PhotoSource.EXIF["Exposure"] + ".";

            return result;

        }
    }
}
