using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella.Events;
using Indulged.API.Cinderella.Factories;
using Indulged.API.Cinderella.Models;
using Indulged.API.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella
{
    public partial class Cinderella
    {
        // Favourite stream returned
        private void OnFavouriteStreamReturned(object sender, GetFavouriteStreamEventArgs e)
        {
            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["photos"];
            TotalFavouritePhotosCount = int.Parse(rootJson["total"].ToString());
            int page = int.Parse(rootJson["page"].ToString());
            int numPages = int.Parse(rootJson["pages"].ToString());
            int perPage = int.Parse(rootJson["perpage"].ToString());

            List<Photo> newPhotos = new List<Photo>();
            foreach (var entry in rootJson["photo"])
            {
                JObject json = (JObject)entry;
                Photo photo = PhotoFactory.PhotoWithJObject(json);

                if (!FavouriteList.Contains(photo))
                {
                    FavouriteList.Add(photo);
                    newPhotos.Add(photo);
                }
            }

            // Dispatch event
            FavouriteStreamUpdatedEventArgs args = new FavouriteStreamUpdatedEventArgs();
            args.Page = page;
            args.PageCount = numPages;
            args.PerPage = perPage;
            args.NewPhotos = newPhotos;
            FavouriteStreamUpdated.DispatchEvent(this, args);
        }
    }
}
