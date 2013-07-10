using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Indulged.API.Utils;
using Indulged.API.Anaconda;
using Indulged.API.Anaconda.Events;
using Indulged.API.Cinderella.Models;
using Indulged.API.Cinderella.Factories;
using Indulged.API.Cinderella.Events;

namespace Indulged.API.Cinderella
{
    public partial class Cinderella
    {
        // Discovery stream returned
        private void OnDiscoveryStreamReturned(object sender, GetDiscoveryStreamEventArgs e)
        {
            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["photos"];
            TotalDiscoveryPhotosCount = int.Parse(rootJson["total"].ToString());
            int page = int.Parse(rootJson["page"].ToString());
            int numPages = int.Parse(rootJson["pages"].ToString());
            int perPage = int.Parse(rootJson["perpage"].ToString());

            List<Photo> newPhotos = new List<Photo>();
            foreach (var entry in rootJson["photo"])
            {
                JObject json = (JObject)entry;
                Photo photo = PhotoFactory.PhotoWithJObject(json);

                if (!DiscoveryList.Contains(photo))
                {
                    DiscoveryList.Add(photo);
                    newPhotos.Add(photo);
                }
            }

            // Dispatch event
            DiscoveryStreamUpdatedEventArgs args = new DiscoveryStreamUpdatedEventArgs();
            args.Page = page;
            args.PageCount = numPages;
            args.PerPage = perPage;
            args.NewPhotos = newPhotos;
            DiscoveryStreamUpdated.DispatchEvent(this, args);
        }
    }
}
