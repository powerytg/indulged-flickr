using Indulged.API.Anaconda;
using Indulged.API.Avarice.Controls;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using Indulged.PolKit;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

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
            Anaconda.AnacondaCore.AddPhotoCommentException += OnAddCommentException;
            Cinderella.CinderellaCore.AddPhotoCommentCompleted += OnAddCommentComplete;
        }

        private bool executedOnce = false;
        private string contextString = null;
        private string contextTypeString = null;

        private Photo currentPhoto;

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            FullScreenRequest -= OnFullScreenRequest;
            Anaconda.AnacondaCore.AddPhotoCommentException -= OnAddCommentException;
            Cinderella.CinderellaCore.AddPhotoCommentCompleted -= OnAddCommentComplete;

            InfoView.RemoveEventListeners();

            base.OnRemovedFromJournal(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (executedOnce)
                return;

            executedOnce = true;

            string photoId = NavigationContext.QueryString["photo_id"];
            currentPhoto = Cinderella.CinderellaCore.PhotoCache[photoId];

            contextString = null;
            if(NavigationContext.QueryString.ContainsKey("context"))
                contextString = NavigationContext.QueryString["context"];

            contextTypeString = null;
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

            PerformAppearanceAnimation();
        }
       
        private void OnCurrentIndexChanged()
        {
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
            string urlString = "/Plugins/Detail/FullScreenPage.xaml?photo_id=" + currentPhoto.ResourceId + "&context=" + contextString;
            if (contextTypeString != null)
                urlString += "&context_type=" + contextTypeString;

            NavigationService.Navigate(new Uri(urlString, UriKind.Relative));
        }

        private void FavButton_Click(object sender, EventArgs e)
        {
            var statusView = new FavStatusView();
            statusView.PhotoSource = currentPhoto;
            var popupContainer = ModalPopup.ShowWithButtons(statusView, "Favourite", statusView.Buttons, false);
            statusView.PopupContainer = popupContainer;

            statusView.BeginFavRequest();
        }

        private void CommentButton_Click(object sender, EventArgs e)
        {
            ShowComposerView();
        }

        private void PerformAppearanceAnimation()
        {
            double h = System.Windows.Application.Current.Host.Content.ActualHeight;

            CompositeTransform ct = (CompositeTransform)LayoutRoot.RenderTransform;
            ct.TranslateY = h;

            LayoutRoot.Visibility = Visibility.Visible;

            Storyboard animation = new Storyboard();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            // Y animation
            DoubleAnimation galleryAnimation = new DoubleAnimation();
            galleryAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            galleryAnimation.To = 0.0;
            //galleryAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(galleryAnimation, LayoutRoot);
            Storyboard.SetTargetProperty(galleryAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            animation.Children.Add(galleryAnimation);
            animation.Begin();
            animation.Completed += (sender, e) => {
                InfoView.PhotoSource = currentPhoto;
                InfoView.Visibility = Visibility.Visible;
                OnCurrentIndexChanged();

                LoadingView.Visibility = Visibility.Collapsed;

                // App bar
                ApplicationBar = Resources["PhotoPageAppBar"] as ApplicationBar;
            };
        }

    }
}