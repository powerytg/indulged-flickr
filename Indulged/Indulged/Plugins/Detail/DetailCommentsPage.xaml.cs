using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class DetailCommentsPage : PhoneApplicationPage
    {        
        // Events
        public static EventHandler FullScreenRequest;

        // Constructor
        public DetailCommentsPage()
        {
            InitializeComponent();

            Anaconda.AnacondaCore.AddPhotoCommentException += OnAddCommentException;
            Cinderella.CinderellaCore.AddPhotoCommentCompleted += OnAddCommentComplete;
        }

        // Data source
        private Photo photo;
        private ObservableCollection<ModelBase> dataSource;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string photoId = NavigationContext.QueryString["photo_id"];
            photo = Cinderella.CinderellaCore.PhotoCache[photoId];

            // Prepare data source
            dataSource = new ObservableCollection<ModelBase>(); 
            dataSource.Add(photo);

            foreach (var comment in photo.Comments)
            {
                dataSource.Add(comment);
            }

            CommentsListView.ItemsSource = dataSource;

            // App bar
            ApplicationBar = Resources["PhotoPageAppBar"] as ApplicationBar;

            // Background
            if (PolicyKit.ShouldUseBlurredBackground)
                BackgroundImage.PhotoSource = photo;
            else if (BackgroundImage.PhotoSource != null)
                BackgroundImage.PhotoSource = null;

        }

        private void CommentButton_Click(object sender, EventArgs e)
        {
            ShowComposerView();
        }

        
    }
}