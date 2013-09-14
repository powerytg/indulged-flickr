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
using Indulged.API.Anaconda.Events;
using Indulged.API.Anaconda;

namespace Indulged.Plugins.Detail
{
    public partial class PhotoCommentsView : UserControl
    {
        public static readonly DependencyProperty PhotoSourceProperty = DependencyProperty.Register("PhotoSource", typeof(Photo), typeof(PhotoCommentsView), new PropertyMetadata(OnPhotoSourcePropertyChanged));

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
            ((PhotoCommentsView)sender).OnPhotoSourceChanged();
        }

        protected virtual void OnPhotoSourceChanged()
        {
            LoadingView.Visibility = Visibility.Visible;
            renderer1.Visibility = Visibility.Collapsed;
            renderer2.Visibility = Visibility.Collapsed;
            renderer3.Visibility = Visibility.Collapsed;
            ViewAllButton.Visibility = Visibility.Collapsed;
        }

        // Constructor
        public PhotoCommentsView()
        {
            InitializeComponent();

            // Events
            Anaconda.AnacondaCore.GetPhotoCommentsException += OnGetCommentsException;
            Cinderella.CinderellaCore.PhotoCommentsUpdated += OnCommentsUpdated;
            Cinderella.CinderellaCore.AddPhotoCommentCompleted += OnAddCommentComplete;
        }

        public void RemoveEventListeners()
        {
            Anaconda.AnacondaCore.GetPhotoCommentsException -= OnGetCommentsException;
            Cinderella.CinderellaCore.PhotoCommentsUpdated -= OnCommentsUpdated;
            Cinderella.CinderellaCore.AddPhotoCommentCompleted -= OnAddCommentComplete;
        }

        private void UpdateItemRenderers()
        {
            if (PhotoSource.Comments.Count == 0)
            {
                LoadingView.Text = "This photo don't have any comments";
            }
            else
            {
                LoadingView.Visibility = Visibility.Collapsed;

                if (PhotoSource.Comments.Count >= 1)
                {
                    renderer1.Visibility = Visibility.Visible;
                    renderer1.Comment = PhotoSource.Comments[0];
                }

                if (PhotoSource.Comments.Count >= 2)
                {
                    renderer2.Visibility = Visibility.Visible;
                    renderer2.Comment = PhotoSource.Comments[1];
                }

                if (PhotoSource.Comments.Count >= 3)
                {
                    renderer3.Visibility = Visibility.Visible;
                    renderer3.Comment = PhotoSource.Comments[0];
                }

                ViewAllButton.Visibility = Visibility.Visible;
            }

        }
    
        private void OnAddCommentComplete(object sender, AddPhotoCommentCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                UpdateItemRenderers();
            });
        }    

        private void OnGetCommentsException(object sender, GetPhotoCommentsExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                LoadingView.Text = "Cannot load comments";
            });
        }

        private void OnCommentsUpdated(object sender, PhotoCommentsUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                UpdateItemRenderers();
            });
        }

        private void ViewAllButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootVisual = System.Windows.Application.Current.RootVisual as Frame;
            PhoneApplicationPage currentPage = (PhoneApplicationPage)rootVisual.Content;
            currentPage.NavigationService.Navigate(new Uri("/Plugins/Detail/DetailCommentsPage.xaml?photo_id=" + PhotoSource.ResourceId, UriKind.Relative));

        }

    }
}
