using Indulged.API.Cinderella.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Cinderella.Factories
{
    public class UserFactory
    {
        public static User UserWithJObject(JObject json)
        {
            User user = null;
            string userId = json["owner"].ToString();
            if (Cinderella.CinderellaCore.UserCache.ContainsKey(userId))
                user = Cinderella.CinderellaCore.UserCache[userId];
            else
            {
                user = new User();
                user.ResourceId = userId;
                Cinderella.CinderellaCore.UserCache[user.ResourceId] = user;
            }

            JToken nameValue;
            if (json.TryGetValue("ownername", out nameValue))
            {
                user.Name = json["ownername"].ToString();
            }

            JToken farmValue;
            if (json.TryGetValue("iconfarm", out farmValue))
            {
                user.Farm = json["iconfarm"].ToString();
            }

            JToken serverValue;
            if (json.TryGetValue("iconserver", out serverValue))
            {
                user.Server = json["iconserver"].ToString();
            }


            return user;
        }
    }
}
