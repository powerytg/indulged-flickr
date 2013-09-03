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
using Indulged.API.Avarice.Controls;

namespace Indulged.Plugins.Detail
{
    public partial class DetailPage : PhoneApplicationPage
    {        
        // Events
        public static EventHandler FullScreenRequest;

        // Constructor
        public DetailPage()
        {
            InitializeComponent();

            // Events
            FullScreenRequest += OnFullScreenRequest;
        }

        // Photo collection context
        public List<Photo> CollectionContext = new List<Photo>();

        private bool executedOnce = false;
        private string contextString = null;
        private string contextTypeString = null;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (executedOnce)
                return;

            executedOnce = true;

            string photoId = NavigationContext.QueryString["photo_id"];
            Photo currentPhoto = Cinderella.CinderellaCore.PhotoCache[photoId];

            contextString = null;
            if(NavigationContext.QueryString.ContainsKey("context"))
                contextString = NavigationContext.QueryString["context"];

            contextTypeString = null;
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

                // Get photo info
                Anaconda.AnacondaCore.GetPhotoInfoAsync(currentPhoto.ResourceId, Cinderella.CinderellaCore.CurrentUser.ResourceId, false);
            }

            // Download comments
            Anaconda.AnacondaCore.GetPhotoCommentsAsync(currentPhoto.ResourceId);

            if (PolicyKit.ShouldUseBlurredBackground)
                BackgroundImage.PhotoSource = currentPhoto;
            else if (BackgroundImage.PhotoSource != null)
                BackgroundImage.PhotoSource = null;

        }

        private void OnFullScreenRequest(object sender, EventArgs e)
        {
            int selectedIndex = PhotoPivot.SelectedIndex;
            Photo currentPhoto = CollectionContext[selectedIndex];

            string urlString = "/Plugins/Detail/FullScreenPage.xaml?photo_id=" + currentPhoto.ResourceId + "&context=" + contextString;
            if (contextTypeString != null)
                urlString += "&context_type=" + contextTypeString;

            NavigationService.Navigate(new Uri(urlString, UriKind.Relative));
        }

        private void FavButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = PhotoPivot.SelectedIndex;
            Photo currentPhoto = CollectionContext[selectedIndex];

            var statusView = new FavStatusView();
            statusView.PhotoSource = currentPhoto;
            var popupContainer = ModalPopup.ShowWithButtons(statusView, "Favourite", statusView.Buttons, false);
            statusView.PopupContainer = popupContainer;

            statusView.BeginFavRequest();
        }

    }
}