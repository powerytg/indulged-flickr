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
using System.Windows.Media;

namespace Indulged.Plugins.Common.Renderers
{
    public partial class PhotoStatView : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(PhotoStatView), new PropertyMetadata(OnPhotoSourcePropertyChanged));

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
            ((PhotoStatView)sender).OnPhotoSourceChanged();
        }

        protected void OnPhotoSourceChanged()
        {
            if (PhotoSource.ViewCount > 0)
            {
                ViewIcon.Visibility = Visibility.Visible;
                ViewLabel.Text = PhotoSource.ViewCount.ToString();
            }
            else
            {
                ViewIcon.Visibility = Visibility.Collapsed;
                ViewLabel.Text = null;
            }

        }

        // Constructor
        public PhotoStatView()
        {
            InitializeComponent();
        }
    }
}
