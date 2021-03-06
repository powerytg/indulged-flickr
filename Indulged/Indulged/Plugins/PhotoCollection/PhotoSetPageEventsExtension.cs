﻿using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
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
        public static EventHandler RequestChangePrimaryPhoto;
        public static EventHandler RequestEditProperties;
        public static EventHandler RequestDeletePhotoSet;

        private void InitializeEventListeners()
        {
            Cinderella.CinderellaCore.PhotoSetPhotosUpdated += OnPhotoStreamUpdated;
            Anaconda.AnacondaCore.PhotoSetPhotosException += OnPhotoStreamException;

            Cinderella.CinderellaCore.AddPhotoToSetCompleted += OnPhotoAddedToSet;
            Cinderella.CinderellaCore.RemovePhotoFromSetCompleted += OnPhotoRemovedFromSet;

            Anaconda.AnacondaCore.PhotoSetDeleted += OnPhotoSetDeleted;
            Anaconda.AnacondaCore.PhotoSetDeleteException += OnPhotoSetDeleteException;

            RequestAddPhotoDialog += OnRequestAddPhotoDialog;
            RequestCamera += OnCameraRequested;
            RequestUpload += OnUploadRequested;
            RequestChangePrimaryPhoto += OnChangePrimaryPhotoRequested;
            RequestEditProperties += OnEditPropertiesRequested;
            RequestDeletePhotoSet += OnDeletePhotoSetRequested;
        }

        private void RemoveEventListeners()
        {
            Cinderella.CinderellaCore.PhotoSetPhotosUpdated -= OnPhotoStreamUpdated;
            Cinderella.CinderellaCore.AddPhotoToSetCompleted -= OnPhotoAddedToSet;
            Cinderella.CinderellaCore.RemovePhotoFromSetCompleted -= OnPhotoRemovedFromSet;

            Anaconda.AnacondaCore.PhotoSetPhotosException -= OnPhotoStreamException;
            Anaconda.AnacondaCore.PhotoSetDeleted -= OnPhotoSetDeleted;
            Anaconda.AnacondaCore.PhotoSetDeleteException -= OnPhotoSetDeleteException;

            RequestAddPhotoDialog -= OnRequestAddPhotoDialog;
            RequestCamera -= OnCameraRequested;
            RequestChangePrimaryPhoto -= OnChangePrimaryPhotoRequested;
            RequestEditProperties -= OnEditPropertiesRequested;
            RequestDeletePhotoSet -= OnDeletePhotoSetRequested;
        }

        private void RefreshMenuButton_Click(object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = AppResources.GroupLoadingPhotosText;

            Anaconda.AnacondaCore.GetPhotoSetPhotosAsync(PhotoSetSource.ResourceId, new Dictionary<string, string> { { "page", "1" }, { "per_page", Anaconda.DefaultItemsPerPage.ToString() } });
        }

        private void CameraMenuButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/ProCam/ImagePickerPage.xaml?upload_to_set_id=" + PhotoSetSource.ResourceId, UriKind.Relative));
        }

        private void OnCameraRequested(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/ProCam/ImagePickerPage.xaml?upload_to_set_id=" + PhotoSetSource.ResourceId, UriKind.Relative));
        }

        private void UploadMenuButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/ProCam/ImagePickerPage.xaml?is_from_library=true&upload_to_set_id=" + PhotoSetSource.ResourceId, UriKind.Relative));
        }

        private void OnUploadRequested(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Plugins/ProCam/ImagePickerPage.xaml?is_from_library=true&upload_to_set_id=" + PhotoSetSource.ResourceId, UriKind.Relative));
        }

        #region Delete photo set

        private DeletePhotoSetView deleteView;
        private ModalPopup deletePhotoSetDialog;
        private Indulged.API.Avarice.Controls.Button deletePhotoSetConfirmButton;

        private void DeleteSetMenuButton_Click(object sender, EventArgs e)
        {
            ShowDeletePhotoSetConfirmDialog();
        }

        private void OnDeletePhotoSetRequested(object sender, EventArgs e)
        {
            ShowDeletePhotoSetConfirmDialog();
        }

        private void ShowDeletePhotoSetConfirmDialog()
        {
            var dialog = ModalPopup.Show("Are you sure to delete this photo set?\n\nThis action cannot be undone", "Delete Set", new List<string> { AppResources.GenericConfirmText, AppResources.GenericCancelText });
            dialog.DismissWithButtonClick += (s, args) =>
            {
                if (args.ButtonIndex == 0)
                {
                    PerformDeletePhotoSet();
                }
            };
        }

        private void PerformDeletePhotoSet()
        {
            deletePhotoSetConfirmButton = new Button();
            deletePhotoSetConfirmButton.Content = AppResources.GenericConfirmText;
            deletePhotoSetConfirmButton.IsEnabled = false;

            deleteView = new DeletePhotoSetView();
            deletePhotoSetDialog = ModalPopup.ShowWithButtons(deleteView, "Sending Request", new List<Button> { deletePhotoSetConfirmButton });
            deletePhotoSetDialog.DismissWithButtonClick += (s, args) =>
            {
                deletePhotoSetDialog = null;
                deleteView = null;
                deletePhotoSetConfirmButton = null;

                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                    NavigationService.RemoveBackEntry();
                }
            };

            Anaconda.AnacondaCore.DeletePhotoSetAsync(PhotoSetSource.ResourceId);
        }

        private void OnPhotoSetDeleted(object sender, DeletePhotoSetEventArgs e)
        {
            if (deletePhotoSetDialog == null || e.SetId != PhotoSetSource.ResourceId)
            {
                return;
            }

            deleteView.ShowCompleteMessage("Complete. Please click on the confirm button to go back");
            deletePhotoSetConfirmButton.IsEnabled = true;
        }

        private void OnPhotoSetDeleteException(object sender, DeletePhotoSetExceptionEventArgs e)
        {
            if (deletePhotoSetDialog == null || e.SetId != PhotoSetSource.ResourceId)
            {
                return;
            }

            deleteView.ShowCompleteMessage("An error happened while attempting to delete this photo set. Please go back and retry");
            deletePhotoSetConfirmButton.IsEnabled = true;
        }

        #endregion

        #region Edit set properties

        private void EditMenuButton_Click(object sender, EventArgs e)
        {
            ShowPropertyEditorView();
        }

        private void OnEditPropertiesRequested(object sender, EventArgs e)
        {
            ShowPropertyEditorView();
        }

        #endregion

        #region Change primary photo

        private PhotoSetPrimaryChooserView primaryChooserView;
        private void OnChangePrimaryPhotoRequested(object sender, EventArgs e)
        {
            ShowPrimaryPhotoChooserDialog();
        }

        private void ShowPrimaryPhotoChooserDialog()
        {
            primaryChooserView = new PhotoSetPrimaryChooserView(PhotoSetSource);
            var dialog = ModalPopup.Show(primaryChooserView, "Choose Cover Photo", new List<string> { "Done" });
            dialog.DismissWithButtonClick += (s, args) =>
            {
                primaryChooserView = null;

                // Re-render all the items since the primary photo may have changed
                PhotoCollection.Clear();
                var newGroups = rendererFactory.GeneratePhotoGroupsWithHeadline(PhotoSetSource.Photos, PhotoSetSource.PrimaryPhoto);
                foreach (var group in newGroups)
                {
                    PhotoCollection.Add(group);
                }
            };
        }

        #endregion

        #region Add Photos

        private PhotoSetAddPhotoView addPhotoView;
        private List<string> addedPhotoIds = new List<string>();

        private void OnRequestAddPhotoDialog(object sender, EventArgs e)
        {
            ShowAddPhotoDialog();
        }

        private void AddMenuButton_Click(object sender, EventArgs e)
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
