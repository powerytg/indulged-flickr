﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Indulged.PolKit;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;

namespace Indulged.Plugins.ProCam
{
    public partial class ImagePickerPage : PhoneApplicationPage
    {
        private PhotoChooserTask photoChooserTask;
        private CameraCaptureTask camTask;
        string uploadToSetId;

        // Constructor
        public ImagePickerPage()
        {
            InitializeComponent();
        }

        private bool executedOnce = false;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (executedOnce)
            {
                return;
            }

            executedOnce = true;

            if (NavigationContext.QueryString.ContainsKey("upload_to_set_id"))
            {
                // Upload to the photo set after editing
                uploadToSetId = NavigationContext.QueryString["upload_to_set_id"];
            }


            if (NavigationContext.QueryString.ContainsKey("is_from_library"))
            {
                // Choose from media library
                photoChooserTask = new PhotoChooserTask();
                photoChooserTask.Completed += OnCaptureTaskCompleted;
                photoChooserTask.Show();
            }
            else if (PolicyKit.ShouldUseProCamera)
            {
                // Use ProCam
                if (uploadToSetId != null)
                {
                    NavigationService.Navigate(new Uri("/Plugins/ProCam/ProCamPage.xaml?upload_to_set_id=" + uploadToSetId, UriKind.Relative));
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Plugins/ProCam/ProCamPage.xaml", UriKind.Relative));
                }
                
                NavigationService.RemoveBackEntry();
            }
            else
            {
                // Use system default camera
                camTask = new CameraCaptureTask();
                camTask.Completed += new EventHandler<PhotoResult>(OnCaptureTaskCompleted);
                camTask.Show();
            }
        }

        private void OnCaptureTaskCompleted(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage capturedImage = new BitmapImage();
                capturedImage.SetSource(e.ChosenPhoto);
                e.ChosenPhoto.Close();
                PhoneApplicationService.Current.State["ChosenPhoto"] = capturedImage;

                Dispatcher.BeginInvoke(() =>
                {
                    if (uploadToSetId != null)
                    {
                        NavigationService.Navigate(new Uri("/Plugins/ProFX/ProFXPage.xaml?upload_to_set_id=" + uploadToSetId, UriKind.Relative));
                    }
                    else
                    {
                        NavigationService.Navigate(new Uri("/Plugins/ProFX/ProFXPage.xaml", UriKind.Relative));
                    }
                    
                    NavigationService.RemoveBackEntry();
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                        NavigationService.RemoveBackEntry();
                    }

                });
            }
        }
    }
}