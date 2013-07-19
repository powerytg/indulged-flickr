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
        private void PhotoListReturned(object sender, PhotoSetListEventArgs e)
        {
            JObject json = JObject.Parse(e.Response);
            JObject photosetJson = (JObject)json["photosets"];

            bool canCreate = photosetJson["cancreate"].ToString().ParseBool();
            int page = int.Parse(photosetJson["page"].ToString());
            int perPage = int.Parse(photosetJson["perpage"].ToString());
            int numTotal = int.Parse(photosetJson["total"].ToString());

            PhotoSetList.Clear();
            foreach (var ps in photosetJson["photoset"])
            {
                PhotoSet photoset = PhotoSetFactory.PhotoSetWithJObject((JObject)ps);
                PhotoSetList.Add(photoset);
            }

            // Dispatch event
            PhotoSetListUpdatedEventArgs args = new PhotoSetListUpdatedEventArgs();
            args.CanCreate = canCreate;
            args.Page = page;
            args.PerPage = perPage;
            args.TotalCount = numTotal;
            args.UserId = e.UserId;
            PhotoSetListUpdated.DispatchEvent(this, args);
        }

        private void OnPhotoSetPhotosReturned(object sender, GetPhotoSetPhotosEventArgs e)
        {
            if (!Cinderella.CinderellaCore.PhotoSetCache.ContainsKey(e.PhotoSetId))
                return;

            PhotoSet photoset = Cinderella.CinderellaCore.PhotoSetCache[e.PhotoSetId];

            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["photoset"];
            int TotalCount = int.Parse(rootJson["total"].ToString());
            int page = int.Parse(rootJson["page"].ToString());
            int numPages = int.Parse(rootJson["pages"].ToString());
            int perPage = int.Parse(rootJson["perpage"].ToString());

            List<Photo> newPhotos = new List<Photo>();
            foreach (var entry in rootJson["photo"])
            {
                JObject json = (JObject)entry;
                Photo photo = PhotoFactory.PhotoWithJObject(json);

                if (!photoset.Photos.Contains(photo))
                {
                    photoset.Photos.Add(photo);
                    newPhotos.Add(photo);
                }
            }

            // Dispatch event
            PhotoSetPhotosUpdatedEventArgs evt = new PhotoSetPhotosUpdatedEventArgs();
            evt.PhotoSetId = photoset.ResourceId;
            evt.Page = page;
            evt.PageCount = numPages;
            evt.PerPage = perPage;
            evt.NewPhotos = newPhotos;
            PhotoSetPhotosUpdated.DispatchEvent(this, evt);

        }
    }
}
