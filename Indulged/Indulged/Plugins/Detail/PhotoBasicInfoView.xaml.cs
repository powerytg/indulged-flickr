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

using Indulged.PolKit;
using Microsoft.Phone.Tasks;

namespace Indulged.Plugins.Detail
{
    public partial class PhotoBasicInfoView : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(PhotoBasicInfoView), new PropertyMetadata(OnPhotoSourcePropertyChanged));

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
            ((PhotoBasicInfoView)sender).OnPhotoSourceChanged();
        }

        protected virtual void OnPhotoSourceChanged()
        {
        }

        public PhotoBasicInfoView()
        {
            InitializeComponent();
        }

        private void OnLicenseButtonClick(object sender, RoutedEventArgs e)
        {
            if (PhotoSource.LicenseId == null)
                return;

            License license = PolicyKit.CurrentPolicy.Licenses[PhotoSource.LicenseId];
            if (license.Url == null)
                return;

            WebBrowserTask wbTask = new WebBrowserTask();
            wbTask.Uri = new Uri(license.Url, UriKind.RelativeOrAbsolute);
            wbTask.Show();
        }
    }
}
