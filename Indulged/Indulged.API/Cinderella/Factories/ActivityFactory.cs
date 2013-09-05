using Indulged.API.Utils;
using Indulged.API.Cinderella.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Factories
{
    public class ActivityFactory
    {
        public static PhotoActivity ActivityWithJObject(JObject json)
        {
            // Filter out unsupported item types
            string itemType = json["type"].ToString();
            if (itemType != "photo")
                return null;

            string itemId = json["id"].ToString();
            PhotoActivity activity = null;
            if (Cinderella.CinderellaCore.ActivityCache.ContainsKey(itemId))
            {
                activity = Cinderella.CinderellaCore.ActivityCache[itemId];
            }
            else
            {
                activity = new PhotoActivity();
                activity.ResourceId = itemId;
                Cinderella.CinderellaCore.ActivityCache[itemId] = activity;
            }

            // Title
            activity.Title = json["title"]["_content"].ToString();

            // Photo
            activity.TargetPhoto = PhotoFactory.PhotoWithPhotoActivityJObject(json);

            // Events
            activity.Events.Clear();
            foreach (JObject eventJson in json["activity"]["event"])
            {
                PhotoActivityEventBase evt = null;
                string eventType = eventJson["type"].ToString();
                if (eventType == "fave")
                {
                    evt = new PhotoActivityFaveEvent();
                }
                else if (eventType == "comment")
                {
                    evt = new PhotoActivityCommentEvent();
                    PhotoActivityCommentEvent commentEvt = evt as PhotoActivityCommentEvent;
                    commentEvt.Message = eventJson["_content"].ToString();
                }

                if (evt != null)
                {
                    evt.EventUser = UserFactory.UserWithActivityEventJObject(eventJson);
                    evt.CreationDate = eventJson["dateadded"].ToString().ToDateTime();
                    activity.Events.Add(evt);
                }
            }
            

            return activity;
        }
    }
}
