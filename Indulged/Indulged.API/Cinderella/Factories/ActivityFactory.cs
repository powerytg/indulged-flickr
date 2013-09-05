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

            // Events
            activity.Events.Clear();
            foreach (JObject eventJson in json["activity"]["event"])
            {
                PhotoActivityEventBase evt = null;
                string eventType = eventJson["type"].ToString();
                if (eventType == "fave")
                {
                    evt = new PhotoActivityFaveEvent();
                    evt.EventUser = UserFactory.UserWithJObject(eventJson);
                }
                else if (eventType == "comment")
                {
                    evt = new PhotoActivityCommentEvent();
                    PhotoActivityCommentEvent commentEvt = evt as PhotoActivityCommentEvent;
                    evt.EventUser = UserFactory.UserWithJObject(eventJson);
                    commentEvt.Message = eventJson["_content"].ToString();
                }

                if(evt != null)
                    activity.Events.Add(evt);
            }
            

            return activity;
        }
    }
}
