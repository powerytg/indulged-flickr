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
using Indulged.Plugins.Common.PhotoRenderers;

namespace Indulged.Plugins.PhotoCollection.Renderers
{
    public partial class SimplePhotoRenderer : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(SimplePhotoRenderer), new PropertyMetadata(OnPhotoSourcePropertyChanged));

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
            ((SimplePhotoRenderer)sender).OnPhotoSourceChanged();
        }

        protected virtual void OnPhotoSourceChanged()
        {
            Thumbnail.Source = PhotoRendererDecoder.GetDecodedBitmapImage(PhotoSource, PhotoRendererDecoder.DecodeResolutions.Small);
            Label.Text = PhotoSource.Title;
        }

        // Constructor
        public SimplePhotoRenderer()
        {
            InitializeComponent();
        }
    }
}
