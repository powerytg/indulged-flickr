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
        // Activity stream retrieved for a user
        private void OnActivityStreamReturned(object sender, GetActivityStreamEventArgs e)
        {
            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["items"];
            ActivityItemsCount = int.Parse(rootJson["total"].ToString());

            ActivityList.Clear();
            foreach (var entry in rootJson["item"])
            {
                JObject json = (JObject)entry;
                PhotoActivity activity = ActivityFactory.ActivityWithJObject(json);

                if (activity == null)
                    continue;

                if (!ActivityList.Contains(activity))
                {
                    ActivityList.Add(activity);
                }
            }

            // Dispatch event
            if (ActivityStreamUpdated != null)
                ActivityStreamUpdated(this, null);
        }


    }
}
