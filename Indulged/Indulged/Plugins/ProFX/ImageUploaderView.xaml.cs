using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Indulged.API.Avarice.Controls;
using Windows.Storage.Streams;
using Indulged.Plugins.ProFX.Filters;
using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events; 

namespace Indulged.Plugins.ProFX
{
    public partial class ImageUploaderView : UserControl
    {
        public WriteableBitmap SampledBackgroundBitmap { get; set; }
        public BitmapImage OriginalImage { get; set; }

        private WriteableBitmap bitmapForUpload;
        private MemoryStream bitmapStream;
        private MemoryStream uploadStream;

        private UploadStatusView statusView;
        private ModalPopup statusDialog;
        private string sessionId;

        // Constructor
        public ImageUploaderView()
        {
            InitializeComponent();

            // Events
            Anaconda.AnacondaCore.PhotoUploadProgress += OnUploadProgress;
            Anaconda.AnacondaCore.PhotoUploaded += OnUploadComplete;
            Anaconda.AnacondaCore.PhotoUploadError += OnUploadFailed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ImageProcessingPage.RequestProcessorPage(this, null);
        }

        public void PrepareBackgroundImage()
        {
            double w = Application.Current.RootVisual.RenderSize.Width;
            double h = Application.Current.RootVisual.RenderSize.Height;

            WriteableBitmap backgroundBlurredImage = SampledBackgroundBitmap.Resize((int)w, (int)h, System.Windows.Media.Imaging.WriteableBitmapExtensions.Interpolation.Bilinear);
            BackgroundView.Source = backgroundBlurredImage;
        }

        public async void PrepareImageForUploadAsync()
        {
            WriteableBitmap bmp = new WriteableBitmap(OriginalImage);

            bitmapStream = new MemoryStream();
            bmp.SaveJpeg(bitmapStream, bmp.PixelWidth, bmp.PixelHeight, 0, 100);
            IBuffer bmpBuffer = bitmapStream.GetWindowsRuntimeBuffer();

            // Output buffer
            bitmapForUpload = new WriteableBitmap(bmp.PixelWidth, bmp.PixelHeight);

            using (EditingSession editsession = new EditingSession(bmpBuffer))
            {
                // First add an antique effect 
                foreach (FilterBase fx in ImageProcessingPage.AppliedFilters)
                {
                    editsession.AddFilter(fx.Filter);
                }                

                // Finally, execute the filtering and render to a bitmap
                await editsession.RenderToBitmapAsync(bitmapForUpload.AsBitmap());
                bitmapForUpload.Invalidate();

                bitmapStream.Close();
                bitmapStream = null;
            }

            Dispatcher.BeginInvoke(() => {
                BeginUpload();
            });
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            ContentView.IsHitTestVisible = false;

            statusView = new UploadStatusView();
            statusView.Height = 150;
            //settingsView.SetValue(Grid.RowProperty, 1);
            var returnButton = new Indulged.API.Avarice.Controls.Button();
            returnButton.Content = "Please Wait...";
            var buttons = new List<Indulged.API.Avarice.Controls.Button> {returnButton};
            statusDialog = ModalPopup.ShowWithButtons(statusView, "Uploading Photo", buttons);
            statusDialog.Buttons[0].IsEnabled = false;
            statusView.StatusLabel.Text = "Rendering image";
            statusDialog.DismissWithButtonClick += (s, args) =>
            {
                statusView = null;
                statusDialog = null;
                ContentView.IsHitTestVisible = true;

                uploadStream.Close();
                uploadStream = null;

                bitmapForUpload = null;

                // Dismiss the ProFX page
                ImageProcessingPage.RequestDismiss(this, null);
            };
            
            PrepareImageForUploadAsync();
        }

        private void BeginUpload()
        {
            statusView.StatusLabel.Text = "Uploading";
            statusView.ProgressView.IsIndeterminate = false;

            sessionId = Guid.NewGuid().ToString().Replace("-", null);
            string fileName = DateTime.Now.ToShortDateString();

            // Parameters
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (TitleTextBox.Text.Length > 0)
                parameters["title"] = TitleTextBox.Text;

            if (DescriptionTextBox.Text.Length > 0)
                parameters["description"] = DescriptionTextBox.Text;

            parameters["is_public"] = (PublicSwitch.IsChecked == true) ? "1" : "0";
            parameters["is_friend"] = (FriendSwitch.IsChecked == true) ? "1" : "0";
            parameters["is_family"] = (FamilySwitch.IsChecked == true) ? "1" : "0";

            // Create the upload stream
            uploadStream = new MemoryStream();
            bitmapForUpload.SaveJpeg(uploadStream, bitmapForUpload.PixelWidth, bitmapForUpload.PixelHeight, 0, 100);

            // Save to media library
            statusView.StatusLabel.Text = "Saving to photo library";
            bitmapForUpload.SaveToMediaLibrary(fileName, true);

            uploadStream.Seek(0, SeekOrigin.Begin);
            Anaconda.AnacondaCore.UploadPhoto(sessionId, fileName, uploadStream, parameters);
        }

        private void OnUploadProgress(object sender, UploadProgressEventArgs e)
        {
            if (e.SessionId != sessionId)
                return;

            if (statusView == null)
                return;

            statusView.ProgressView.Value = (float)e.UploadedBytes / (float)e.TotalBytes;
        }

        private void OnUploadComplete(object sender, UploadPhotoEventArgs e)
        {
            if (e.SessionId != sessionId)
                return;

            if (statusView == null)
                return;

            statusView.ProgressView.Value = 1;
            statusView.StatusLabel.Text = "Upload is complete";
            statusDialog.Buttons[0].IsEnabled = true;
            statusDialog.Buttons[0].Content = "Done";
        }

        private void OnUploadFailed(object sender, UploadPhotoErrorEventArgs e)
        {
            if (e.SessionId != sessionId)
                return;

            if (statusView == null)
                return;

            statusView.ProgressView.Value = 1;
            statusView.StatusLabel.Text = "There was an issue while uploading";
            statusDialog.Buttons[0].IsEnabled = true;
            statusDialog.Buttons[0].Content = "Done";
        }
    }
}
