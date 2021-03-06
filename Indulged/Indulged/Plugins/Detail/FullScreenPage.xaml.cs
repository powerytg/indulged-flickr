﻿using System;
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
using System.Windows.Media;
using Indulged.API.Cinderella;
using Indulged.PolKit;

namespace Indulged.Plugins.Detail
{
    public partial class FullScreenPage : PhoneApplicationPage
    {
        
        // Constructor
        public FullScreenPage()
        {
            InitializeComponent();
        }


        private bool executedOnce = false;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (executedOnce)
                return;

            executedOnce = true;


            string photoId = NavigationContext.QueryString["photo_id"];
            Photo currentPhoto = Cinderella.CinderellaCore.PhotoCache[photoId];

            string contextString = null;
            if (NavigationContext.QueryString.ContainsKey("context"))
                contextString = NavigationContext.QueryString["context"];

            string contextTypeString = null;
            if (NavigationContext.QueryString.ContainsKey("context_type"))
                contextTypeString = NavigationContext.QueryString["context_type"];

            List<Photo> CollectionContext = null;
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
                else if (contextTypeString == "PhotoSet")
                {
                    CollectionContext = Cinderella.CinderellaCore.PhotoSetCache[contextString].Photos.ToList();
                }
                else if (contextTypeString == "UserPhotoStream")
                {
                    CollectionContext = Cinderella.CinderellaCore.UserCache[contextString].Photos.ToList();
                }
            }
            else
            {
                CollectionContext = new List<Photo>();
                CollectionContext.Add(currentPhoto);
            }

            PhotoView.PhotoSource = currentPhoto;
        }
        
    }
}