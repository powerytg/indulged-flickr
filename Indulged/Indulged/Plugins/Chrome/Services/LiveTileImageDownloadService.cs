using Indulged.API.Cinderella.Models;
using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Indulged.Plugins.Chrome.Services
{
    public class LiveTileUpdateService
    {
        public static int MAX_TILE_IMAGE_COUNT = 7;

        private static volatile LiveTileUpdateService instance;
        private static object syncRoot = new Object();

        public static LiveTileUpdateService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new LiveTileUpdateService();
                    }
                }

                return instance;
            }
        }

        private List<String> tileImages = new List<string>();

        /// <summary>
        /// Constructor
        /// </summary>
        private LiveTileUpdateService()
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.DirectoryExists("/shared/transfers"))
                {
                    isoStore.CreateDirectory("/shared/transfers");
                }
            }
        }

        private void AddDownloadRequest(Photo photo, int index)
        {
            if (BackgroundTransferService.Requests.Count() >= MAX_TILE_IMAGE_COUNT)
            {
                return;
            }

            string filename = "tile" + index.ToString() + ".jpg";

            Uri transferUri = new Uri(photo.GetImageUrl(), UriKind.Absolute);

            // Create the new transfer request, passing in the URI of the file to 
            // be transferred.
            BackgroundTransferRequest transferRequest = new BackgroundTransferRequest(transferUri);

            // Set the transfer method. GET and POST are supported.
            transferRequest.Method = "GET";

            // Set download location uri
            transferRequest.DownloadLocation = new Uri("shared/transfers/" + filename, UriKind.RelativeOrAbsolute);
            transferRequest.TransferStatusChanged += new EventHandler<BackgroundTransferEventArgs>(transfer_TransferStatusChanged);
            transferRequest.Tag = filename;
            ProcessTransfer(transferRequest);

            // Start request
            try
            {
                BackgroundTransferService.Add(transferRequest);
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to add background transfer request. " + ex.Message);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Unable to add background transfer request.");
            }
        }

        public void StartNewRequests(List<Photo> photos)
        {
            ClearAll();

            int i = 0;
            foreach (var photo in photos)
            {
                AddDownloadRequest(photo, i);
                i++;
            }
        }

        public void ClearAll()
        {
            // Clear all tile images
            tileImages.Clear();

            foreach (var transfer in BackgroundTransferService.Requests)
            {
                RemoveTransferRequest(transfer.RequestId);
            }
        }

        private void ProcessTransfer(BackgroundTransferRequest transfer)
        {
            if (transfer.TransferStatus == TransferStatus.Completed)
            {
                // If the status code of a completed transfer is 200 or 206, the
                // transfer was successful
                if (transfer.StatusCode == 200 || transfer.StatusCode == 206)
                {
                    // Remove the transfer request in order to make room in the 
                    // queue for more transfers. Transfers are not automatically
                    // removed by the system.
                    RemoveTransferRequest(transfer.RequestId);
                    
                    // In this example, the downloaded file is moved into the root
                    // Isolated Storage directory
                    using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        string filename = transfer.Tag;
                        string tileFilename = "shared/shellcontent/" + filename;

                        if (isoStore.FileExists(tileFilename))
                        {
                            isoStore.DeleteFile(tileFilename);
                        }

                        isoStore.CopyFile(transfer.DownloadLocation.OriginalString, tileFilename);
                        //isoStore.MoveFile(transfer.DownloadLocation.OriginalString, tileFilename);
                        tileImages.Add(filename);
                    }
                }
                else
                {
                    // This is where you can handle whatever error is indicated by the
                    // StatusCode and then remove the transfer from the queue. 
                    RemoveTransferRequest(transfer.RequestId);
                }
            }

        }

        private void RemoveTransferRequest(string transferID)
        {
            // Use Find to retrieve the transfer request with the specified ID.
            BackgroundTransferRequest transferToRemove = BackgroundTransferService.Find(transferID);

            // Try to remove the transfer from the background transfer service.
            try
            {
                BackgroundTransferService.Remove(transferToRemove);
            }
            catch (Exception e)
            {
                // Handle the exception.
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private void transfer_TransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            ProcessTransfer(e.Request);
            
            // Update the tiles if complete
            if (tileImages.Count >= MAX_TILE_IMAGE_COUNT)
            {
                new Thread(() =>
                {
                    UpdateLiveTiles();
                }).Start();
            }
        }

        #region Tile updates

        public void UpdateLiveTiles()
        {
            var uriList = new List<Uri>();
            for (int i = 0; i < Math.Min(MAX_TILE_IMAGE_COUNT, tileImages.Count); i++)
            {
                string filename = tileImages[i];
                uriList.Add(new Uri("isostore:/Shared/ShellContent/" + filename, UriKind.Absolute));
            }

            ShellTile.ActiveTiles.First().Update(new CycleTileData()
                {
                    Title = "Indulged",
                    CycleImages = uriList.ToList<Uri>()
                });
        }

        #endregion

    }
}
