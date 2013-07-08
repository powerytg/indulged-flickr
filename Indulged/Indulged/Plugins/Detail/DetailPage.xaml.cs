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
using Indulged.API.Cinderella.Models;

namespace Indulged.Plugins.Detail
{
    public partial class DetailPage : PhoneApplicationPage
    {        
        // Constructor
        public DetailPage()
        {
            InitializeComponent();
        }

        // Photo object
        private Photo _photo;
        public Photo CurrentPhoto
        {
            get
            {
                return _photo;
            }

            set
            {
                _photo = value;
                this.DataContext = _photo;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string photoId = NavigationContext.QueryString["photo_id"];
            if (Cinderella.CinderellaCore.PhotoCache.ContainsKey(photoId))
            {
                CurrentPhoto = Cinderella.CinderellaCore.PhotoCache[photoId];
            }
            else
            {

            }
        }
    }
}