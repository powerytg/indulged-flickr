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

namespace Indulged.Plugins.Common.Renderers
{
    public partial class PhotoStatBoxView : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(PhotoStatBoxView), new PropertyMetadata(OnPhotoSourcePropertyChanged));

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
            ((PhotoStatBoxView)sender).OnPhotoSourceChanged();
        }

        protected void OnPhotoSourceChanged()
        {
            if (PhotoSource.ViewCount > 0)
            {
                ViewLabel.Text = PhotoSource.ViewCount.ToString();
            }
            else
            {
                ViewLabel.Text = null;
            }

        }

        // Constructor
        public PhotoStatBoxView()
        {
            InitializeComponent();
        }
    }
}
