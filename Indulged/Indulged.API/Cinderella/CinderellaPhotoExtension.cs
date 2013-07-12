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
    }
}
