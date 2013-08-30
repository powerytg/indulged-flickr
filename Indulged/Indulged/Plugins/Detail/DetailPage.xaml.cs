using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Indulged.API.Anaconda;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.PolKit;
using System.Windows.Media.Imaging;
using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;
using System.IO;
using Windows.Storage.Streams;

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
            Photo currentPhoto = Cinderella.CinderellaCore.PhotoCache[photoId];

            string contextString = null;
            if(NavigationContext.QueryString.ContainsKey("context"))
                contextString = NavigationContext.QueryString["context"];

            string contextTypeString = null;
            if (NavigationContext.QueryString.ContainsKey("context_type"))
                contextTypeString = NavigationContext.QueryString["context_type"];


            if (contextString == PolicyKit.MyStream)
                CollectionContext = Cinderella.CinderellaCore.CurrentUser.Photos.ToList();
            else if (contextString == PolicyKit.DiscoveryStream)
                CollectionContext = Cinderella.CinderellaCore.DiscoveryList.ToList();
            else if (contextString == PolicyKit.FavouriteStream)
                CollectionContext = Cinderella.CinderellaCore.FavouriteList.ToList();
            else if (contextTypeString != null)
            {
                if (contextTypeString == "Group")
                {
                    CollectionContext = Cinderella.CinderellaCore.GroupCache[contextString].Photos.ToList();
                }
            }
            else
            {
                CollectionContext.Add(currentPhoto);
            }
            
            PhotoPivot.ItemsSource = CollectionContext;
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

            if (PolicyKit.ShouldUseBlurredBackground)
                BackgroundImage.PhotoSource = currentPhoto;
            else if (BackgroundImage.PhotoSource != null)
                BackgroundImage.PhotoSource = null;

        }

    }
}