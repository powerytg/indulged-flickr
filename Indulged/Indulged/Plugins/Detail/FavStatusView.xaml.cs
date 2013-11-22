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
using Indulged.API.Avarice.Controls;
using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Avarice.Controls.SupportClasses;
using Indulged.Resources;

namespace Indulged.Plugins.Detail
{
    public partial class FavStatusView : UserControl, IModalPopupContent
    {
        public Photo PhotoSource { get; set; }

        // Reference to the modal popup container
        public ModalPopup PopupContainer { get; set; }

        private Indulged.API.Avarice.Controls.Button doneButton;
        public List<Indulged.API.Avarice.Controls.Button> Buttons { get; set; }

        private bool _passInfoDetection = false;
        public bool PassInfoDetection
        {
            get
            {
                return _passInfoDetection;
            }

            set
            {
                _passInfoDetection = value;
            }
        }

        public void BeginFavRequest()
        {
            doneButton.IsEnabled = false;

            if (_passInfoDetection)
            {
                if (PhotoSource.IsFavourite)
                {
                    RemoveFavourite();
                }
                else
                {
                    AddToFavourite();
                }
            }
            else
            {
                StatusLabel.Text = AppResources.DetailPageRetrievingPhotoInfoText;

                // Refresh photo info
                Anaconda.AnacondaCore.GetPhotoInfoAsync(PhotoSource.ResourceId, PhotoSource.UserId, false);
            }
        }

        // Constructor
        public FavStatusView()
        {
            InitializeComponent();

            Buttons = new List<API.Avarice.Controls.Button>();
            doneButton = new API.Avarice.Controls.Button();
            doneButton.Content = AppResources.GenericDoneText;
            doneButton.Click += (sender, e) =>
            {
                PopupContainer.Dismiss();
            };

            Buttons.Add(doneButton);

            // Events
            Anaconda.AnacondaCore.PhotoInfoException += OnPhotoInfoException;
            Anaconda.AnacondaCore.AddPhotoAsFavouriteException += OnAddFavouriteException;
            Anaconda.AnacondaCore.RemovePhotoFromFavouriteException += OnRemoveFavouriteException;
            
            Cinderella.CinderellaCore.PhotoInfoUpdated += OnPhotoInfoUpdated;
            Cinderella.CinderellaCore.PhotoAddedAsFavourite += OnAddedAsFavourite;
            Cinderella.CinderellaCore.PhotoRemovedFromFavourite += OnRemovedFromFavourite;
        }

        private bool eventsRemoved = false;
        public void OnPopupRemoved()
        {
            if (eventsRemoved)
                return;

            eventsRemoved = true;

            Anaconda.AnacondaCore.PhotoInfoException -= OnPhotoInfoException;
            Anaconda.AnacondaCore.AddPhotoAsFavouriteException -= OnAddFavouriteException;
            Anaconda.AnacondaCore.RemovePhotoFromFavouriteException -= OnRemoveFavouriteException;

            Cinderella.CinderellaCore.PhotoInfoUpdated -= OnPhotoInfoUpdated;
            Cinderella.CinderellaCore.PhotoAddedAsFavourite -= OnAddedAsFavourite;
            Cinderella.CinderellaCore.PhotoRemovedFromFavourite -= OnRemovedFromFavourite;
        }

        private void OnPhotoInfoException(object sender, GetPhotoInfoExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                ProgressView.Visibility = Visibility.Collapsed;
                StatusLabel.Text = AppResources.DetailPageRetrievePhotoInfoErrorText;
                doneButton.IsEnabled = true;
            });
        }

        private void OnPhotoInfoUpdated(object sender, PhotoInfoUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                if (PhotoSource.IsFavourite)
                {
                    RemoveFavourite();
                }
                else
                {
                    AddToFavourite();
                }
            });
        }

        private void AddToFavourite()
        {
            Dispatcher.BeginInvoke(() =>
            {
                StatusLabel.Text = AppResources.DetailPageMarkingAsFavText;
                Anaconda.AnacondaCore.AddPhotoToFavouriteAsync(PhotoSource.ResourceId);
            });
        }

        private void RemoveFavourite()
        {
            Dispatcher.BeginInvoke(() =>
            {
                StatusLabel.Text = AppResources.DetailPageRemovingFavText;
                Anaconda.AnacondaCore.RemovePhotoFromFavouriteAsync(PhotoSource.ResourceId);
            });
        }

        private void OnAddFavouriteException(object sender, AddFavouriteExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                ProgressView.Visibility = Visibility.Collapsed;
                StatusLabel.Text = e.Message;
                doneButton.IsEnabled = true;
            });
        }

        private void OnRemoveFavouriteException(object sender, RemoveFavouriteExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                ProgressView.Visibility = Visibility.Collapsed;
                StatusLabel.Text = e.Message;
                doneButton.IsEnabled = true;
            });
        }

        private void OnAddedAsFavourite(object sender, PhotoAddedAsFavouriteEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                ProgressView.Visibility = Visibility.Collapsed;
                StatusLabel.Text = AppResources.DetailPageAddedFavText;
                doneButton.IsEnabled = true;
            });
        }

        private void OnRemovedFromFavourite(object sender, PhotoRemovedFromFavouriteEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.PhotoId != PhotoSource.ResourceId)
                    return;

                ProgressView.Visibility = Visibility.Collapsed;
                StatusLabel.Text = AppResources.DetailPageRemovedFavText;
                doneButton.IsEnabled = true;
            });
        }
    }
}
