using Indulged.API.Anaconda;
using Indulged.API.Avarice.Controls;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Models;
using Indulged.Plugins.Common.PhotoGroupRenderers;
using Indulged.Resources;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Indulged.Plugins.PhotoCollection
{
    public partial class PhotoSetPage
    {
        public static EventHandler RequestAddPhotoDialog;
        public static EventHandler RequestCamera;
        public static EventHandler RequestUpload;

        private void InitializeEventListeners()
        {
            Cinderella.CinderellaCore.PhotoSetPhotosUpdated += OnPhotoStreamUpdated;
            Anaconda.AnacondaCore.PhotoSetPhotosException += OnPhotoStreamException;

            Cinderella.CinderellaCore.AddPhotoToSetCompleted += OnPhotoAddedToSet;
            Cinderella.CinderellaCore.RemovePhotoFromSetCompleted += OnPhotoRemovedFromSet;

            RequestAddPhotoDialog += OnRequestAddPhotoDialog;
            RequestCamera += OnCameraRequested;
            RequestUpload += OnUploadRequested;
        }

        private void RemoveEventListeners()
        {
            Cinderella.CinderellaCore.PhotoSetPhotosUpdated -= OnPhotoStreamUpdated;
            Cinderella.CinderellaCore.AddPhotoToSetCompleted -= OnPhotoAddedToSet;
            Cinderella.CinderellaCore.RemovePhotoFromSetCompleted -= OnPhotoRemovedFromSet;

            Anaconda.AnacondaCore.PhotoSetPhotosException -= OnPhotoStreamException;

            RequestAddPhotoDialog -= OnRequestAddPhotoDialog;
            RequestCamera -= OnCameraRequested;
        }

        private void RefreshPhotoListButton_Click(object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = AppResources.GroupLoadingPhotosText;

            Anaconda.AnacondaCore.GetPhotoSetPhotosAsync(PhotoSetSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

        private void OnCameraRequested(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/ProCam/ImagePickerPage.xaml?upload_to_set_id=" + PhotoSetSource.ResourceId, UriKind.Relative));
        }

        private void OnUploadRequested(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/ProCam/ImagePickerPage.xaml?is_from_library=true&upload_to_set_id=" + PhotoSetSource.ResourceId, UriKind.Relative));
        }

        #region Add Photos

        private PhotoSetAddPhotoView addPhotoView;
        private List<string> addedPhotoIds = new List<string>();

        private void OnRequestAddPhotoDialog(object sender, EventArgs e)
        {
            ShowAddPhotoDialog();
        }

        private void AddPhotoButton_Click(object sender, EventArgs e)
        {
            ShowAddPhotoDialog();
        }

        private void ShowAddPhotoDialog()
        {
            addedPhotoIds.Clear();

            addPhotoView = new PhotoSetAddPhotoView(PhotoSetSource);
            var addPhotoDialog = ModalPopup.Show(addPhotoView, AppResources.PhotoCollectionAddToSetText, new List<string> { "Done Adding Photos" });
            addPhotoDialog.DismissWithButtonClick += (s, args) =>
            {
                if (addedPhotoIds.Count > 0)
                {
                    List<Photo> newPhotos = new List<Photo>();
                    foreach (string photoId in addedPhotoIds)
                    {
                        newPhotos.Add(Cinderella.CinderellaCore.PhotoCache[photoId]);
                    }

                    List<PhotoGroup> newGroups = rendererFactory.GeneratePhotoGroups(newPhotos);

                    // Insert at position 1, leaving the primary photo on top
                    foreach (var group in newGroups)
                    {
                        PhotoCollection.Insert(1, group);
                    }
                }

                if (PhotoSetSource.Photos.Count > 0)
                {
                    StatusLabel.Visibility = Visibility.Collapsed;
                    PhotoStreamListView.Visibility = Visibility.Visible;
                }

                addPhotoDialog = null;
            };
        }

        private void OnPhotoAddedToSet(object sender, AddPhotoToSetCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.SetId != PhotoSetSource.ResourceId)
                    return;

                Photo newPhoto = Cinderella.CinderellaCore.PhotoCache[e.PhotoId];
                if (!addedPhotoIds.Contains(newPhoto.ResourceId))
                {
                    addedPhotoIds.Add(newPhoto.ResourceId);
                }
            });
        }

        private void OnPhotoRemovedFromSet(object sender, RemovePhotoFromSetCompleteEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.SetId != PhotoSetSource.ResourceId || PhotoSetSource.Photos.Count == 0)
                    return;

                if (addedPhotoIds.Contains(e.PhotoId))
                {
                    addedPhotoIds.Remove(e.PhotoId);
                }
                                
            });
        }

        #endregion

    }
}
