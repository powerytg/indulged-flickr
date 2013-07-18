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
        private void OnPhotoSearchReturned(object sender, PhotoSearchEventArgs e)
        {
            JObject json = JObject.Parse(e.Response);
            JObject photosetJson = (JObject)json["photos"];

            int page = int.Parse(photosetJson["page"].ToString());
            int perPage = int.Parse(photosetJson["perpage"].ToString());
            int numTotal = int.Parse(photosetJson["total"].ToString());

            List<Photo> photos = new List<Photo>();
            foreach (var photoJson in photosetJson["photo"])
            {
                Photo photo = PhotoFactory.PhotoWithJObject((JObject)photoJson);
                photos.Add(photo);
            }


            PhotoSearchResultEventArgs evt = new PhotoSearchResultEventArgs();
            evt.SearchSessionId = e.SearchSessionId;
            evt.Page = page;
            evt.PerPage = perPage;
            evt.TotalCount = numTotal;
            evt.Photos = photos;
            PhotoSearchCompleted.DispatchEvent(this, evt);
        }

        private void OnGroupSearchReturned(object sender, GroupSearchEventArgs e)
        {
            JObject json = JObject.Parse(e.Response);
            JObject groupJson = (JObject)json["groups"];

            int page = int.Parse(groupJson["page"].ToString());
            int perPage = int.Parse(groupJson["perpage"].ToString());
            int numTotal = int.Parse(groupJson["total"].ToString());

            List<FlickrGroup> groups = new List<FlickrGroup>();
            foreach (var js in groupJson["group"])
            {
                FlickrGroup group = FlickrGroupFactory.GroupWithJObject((JObject)js);
                groups.Add(group);
            }


            GroupSearchResultEventArgs evt = new GroupSearchResultEventArgs();
            evt.SearchSessionId = e.SearchSessionId;
            evt.Page = page;
            evt.PerPage = perPage;
            evt.TotalCount = numTotal;
            evt.Groups = groups;
            GroupSearchCompleted.DispatchEvent(this, evt);
        }
    }
}
