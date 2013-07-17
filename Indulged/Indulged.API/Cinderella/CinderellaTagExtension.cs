using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Utils;
using Indulged.API.Cinderella.Events;
using Indulged.API.Anaconda.Events;
using Newtonsoft.Json.Linq;
using Indulged.API.Cinderella.Models;

namespace Indulged.API.Cinderella
{
    public partial class Cinderella
    {
        private void OnPopularTagListReturned(object sender, GetPopularTagListEventArgs e)
        {
            JObject rawJson = JObject.Parse(e.Response);
            JObject rootJson = (JObject)rawJson["hottags"];

            List<PhotoTag> result = new List<PhotoTag>();
            foreach (JObject json in rootJson["tag"])
            {
                PhotoTag tag = new PhotoTag();
                tag.Name = json["_content"].ToString();
                tag.Weight = int.Parse(json["score"].ToString());
                result.Add(tag);
            }

            PopularTagListUpdatedEventArgs evt = new PopularTagListUpdatedEventArgs();
            evt.Tags = result;
            PopularTagsUpdated.DispatchEvent(this, evt);

        }
    }
}
