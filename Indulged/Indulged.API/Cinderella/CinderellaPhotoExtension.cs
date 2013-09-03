using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Indulged.API.Utils;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Factories;
using Indulged.API.Cinderella.Events;

namespace Indulged.API.Cinderella
{
    public partial class Cinderella
    {
        private void OnEXIFReturned(object sender, GetEXIFEventArgs e)
        {
            if (!PhotoCache.ContainsKey(e.PhotoId))
                return;

            Photo photo = PhotoCache[e.PhotoId];
            JObject json = JObject.Parse(e.Response);
            photo.EXIF = PhotoEXIFFactory.EXIFWithJObject((JObject)json["photo"]);

            EXIFUpdatedEventArgs evt = new EXIFUpdatedEventArgs();
            evt.PhotoId = photo.ResourceId;
            EXIFUpdated.DispatchEvent(this, evt);
        }

        private void OnPhotoInfoReturned(object sender, GetPhotoInfoEventArgs e)
        {
            JObject json = JObject.Parse(e.Response);
            Photo photo = PhotoFactory.PhotoWithJObject((JObject)json["photo"]);

            // Should add this photo to stream?
            if (e.IsUploadedPhoto)
            {
                // Dispatch event
                UploadedPhotoInfoReturnedEventArgs evt = new UploadedPhotoInfoReturnedEventArgs();
                evt.PhotoId = e.PhotoId;
                UploadedPhotoInfoReturned.DispatchEvent(this, evt);
            }

            var updateEvt = new PhotoInfoUpdatedEventArgs();
            updateEvt.PhotoId = photo.ResourceId;
            PhotoInfoUpdated.DispatchEvent(this, updateEvt);
        }

        private void OnPhotoUploaded(object sender, UploadPhotoEventArgs e)
        {
            /*
            Photo newPhoto = new Photo();
            newPhoto.ResourceId = e.PhotoId;

            PhotoCache[e.PhotoId] = newPhoto;

            CurrentUser.Photos.Add(newPhoto);
            CurrentUser.PhotoCount++;

            // Dispatch event
            PhotoUploadCompleteEventArgs evt = new PhotoUploadCompleteEventArgs();
            evt.PhotoId = e.PhotoId;
            PhotoUploadCompleted.DispatchEvent(this, evt);
             * */
        }

        private void OnPhotoCommentsReturned(object sender, GetPhotoCommentsEventArgs e)
        {
            if (!PhotoCache.ContainsKey(e.PhotoId))
                return;

            Photo photo = PhotoCache[e.PhotoId];

            // Hack prevent evulation func timeout
            if (e.Response.Contains("_content"))
            {                
                JObject rawJson = JObject.Parse(e.Response);
                JObject rootJson = (JObject)rawJson["comments"];

                photo.Comments.Clear();

                foreach (var entry in rootJson["comment"])
                {
                    JObject commentJObject = (JObject)entry;
                    PhotoComment comment = PhotoCommentFactory.PhotoCommentWithJObject(commentJObject, photo);
                    photo.Comments.Add(comment);
                }

            }


            PhotoCommentsUpdatedEventArgs evt = new PhotoCommentsUpdatedEventArgs();
            evt.PhotoId = photo.ResourceId;
            PhotoCommentsUpdated.DispatchEvent(this, evt);
        }
    }
}
