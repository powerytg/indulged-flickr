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
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.Detail
{
    public partial class FullScreenPhotoView : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(FullScreenPhotoView), new PropertyMetadata(OnPhotoSourcePropertyChanged));

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
            ((FullScreenPhotoView)sender).OnPhotoSourceChanged();
        }

        protected virtual void OnPhotoSourceChanged()
        {
            ImageView.Source = new BitmapImage(new Uri(PhotoSource.GetImageUrl()));
        }

        // Constructor
        public FullScreenPhotoView()
        {
            InitializeComponent();
        }
    }
}
