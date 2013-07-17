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

namespace Indulged.Plugins.Detail
{
    public partial class PhotoTagsView : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(PhotoTagsView), new PropertyMetadata(OnPhotoSourcePropertyChanged));

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
            ((PhotoTagsView)sender).OnPhotoSourceChanged();
        }

        protected virtual void OnPhotoSourceChanged()
        {
            if (PhotoSource.Tags.Count == 0)
            {
                NoTagLabel.Visibility = Visibility.Visible;
                TagsListView.Visibility = Visibility.Collapsed;
                TagsListView.ItemsSource = null;
            }
            else
            {
                NoTagLabel.Visibility = Visibility.Collapsed;
                TagsListView.Visibility = Visibility.Visible;
                TagsListView.ItemsSource = PhotoSource.Tags;
            }
        }

        // Constructor
        public PhotoTagsView()
        {
            InitializeComponent();
        }

    }
}
