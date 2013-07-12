using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.PolKit;

namespace Indulged.Plugins.Detail
{
    public partial class DetailPage : PhoneApplicationPage
    {        
        // Constructor
        public DetailPage()
        {
            InitializeComponent();
        }

        // Photo collection context
        public List<Photo> CollectionContext = new List<Photo>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string photoId = NavigationContext.QueryString["photo_id"];
            string contextString = NavigationContext.QueryString["context"];
            
            if (contextString == PolicyKit.MyStream)
                CollectionContext = Cinderella.CinderellaCore.CurrentUser.Photos.ToList();
            else if (contextString == PolicyKit.DiscoveryStream)
                CollectionContext = Cinderella.CinderellaCore.DiscoveryList.ToList();
            
            PhotoPivot.ItemsSource = CollectionContext;
            
            Photo currentPhoto = Cinderella.CinderellaCore.PhotoCache[photoId];
            PhotoPivot.SelectedIndex = CollectionContext.IndexOf(currentPhoto);
        }

        private void OnCurrentPageChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = PhotoPivot.SelectedIndex;
            Photo currentPhoto = CollectionContext[selectedIndex];

            // Download EXIF info
            if (currentPhoto.EXIF == null)
            {
                if (!Anaconda.AnacondaCore.IsGettingEXIFInfo(currentPhoto.ResourceId))
                    Anaconda.AnacondaCore.GetEXIFAsync(currentPhoto.ResourceId);
            }
        }
    }
}