using Indulged.API.Anaconda.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Cinderella.Factories;
using Newtonsoft.Json.Linq;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Events;
using Indulged.API.Utils;

namespace Indulged.API.Cinderella
{
    public partial class Cinderella
    {
        private void OnGroupInfoReturned(object sender, GetGroupInfoEventArgs e)
        {
            
            JObject rootJson = JObject.Parse(e.Response);
            JObject json = (JObject)rootJson["group"];
            FlickrGroup group = FlickrGroupFactory.GroupWithJObject(json);
            if (group == null)
                return;

            group.IsInfoRetrieved = true;

            var evt = new GroupInfoUpdatedEventArgs();
            evt.GroupId = group.ResourceId;

            GroupInfoUpdated.DispatchEvent(this, evt);

        }

        private void OnGroupPhotosReturned(object sender, GetGroupPhotosEventArgs e)
        {
            if (!Cinderella.CinderellaCore.GroupCache.ContainsKey(e.GroupId))
                return;

            FlickrGroup group = Cinderella.CinderellaCore.GroupCache[e.GroupId];

            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["photos"];
            int TotalCount = int.Parse(rootJson["total"].ToString());
            int page = int.Parse(rootJson["page"].ToString());
            int numPages = int.Parse(rootJson["pages"].ToString());
            int perPage = int.Parse(rootJson["perpage"].ToString());

            List<Photo> newPhotos = new List<Photo>();
            foreach (var entry in rootJson["photo"])
            {
                JObject json = (JObject)entry;
                Photo photo = PhotoFactory.PhotoWithJObject(json);

                if (!group.Photos.Contains(photo))
                {
                    group.Photos.Add(photo);
                    newPhotos.Add(photo);
                }
            }

            // Dispatch event
            GroupPhotoListUpdatedEventArgs evt = new GroupPhotoListUpdatedEventArgs();
            evt.GroupId = group.ResourceId;
            evt.Page = page;
            evt.PageCount = numPages;
            evt.PerPage = perPage;
            evt.NewPhotos = newPhotos;
            GroupPhotoListUpdated.DispatchEvent(this, evt);
        }
    }
}
