using Indulged.API.Cinderella.Models;
using Indulged.API.Utils;
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

        public static User UserWithTopicJObject(JObject json)
        {
            User user = null;
            string userId = json["author"].ToString();
            if (Cinderella.CinderellaCore.UserCache.ContainsKey(userId))
                user = Cinderella.CinderellaCore.UserCache[userId];
            else
            {
                user = new User();
                user.ResourceId = userId;
                Cinderella.CinderellaCore.UserCache[user.ResourceId] = user;
            }

            JToken nameValue;
            if (json.TryGetValue("authorname", out nameValue))
            {
                user.Name = json["authorname"].ToString();
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

        public static User UserWithPhotoCommentJObject(JObject json)
        {
            User user = null;
            string userId = json["author"].ToString();
            if (Cinderella.CinderellaCore.UserCache.ContainsKey(userId))
                user = Cinderella.CinderellaCore.UserCache[userId];
            else
            {
                user = new User();
                user.ResourceId = userId;
                Cinderella.CinderellaCore.UserCache[user.ResourceId] = user;
            }

            JToken nameValue;
            if (json.TryGetValue("authorname", out nameValue))
            {
                user.Name = json["authorname"].ToString();
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

        public static User UserWithUserInfoJObject(JObject rootJson)
        {
            JObject json = (JObject)rootJson["person"];

            User user = null;
            string userId = json["id"].ToString();
            if (Cinderella.CinderellaCore.UserCache.ContainsKey(userId))
                user = Cinderella.CinderellaCore.UserCache[userId];
            else
            {
                user = new User();
                user.ResourceId = userId;
                Cinderella.CinderellaCore.UserCache[user.ResourceId] = user;
            }

            // Is pro user
            user.IsProUser = (json["ispro"].ToString() == "1");

            // User name
            JToken nameValue;
            if (json.TryGetValue("username", out nameValue))
            {
                user.Name = json["username"]["_content"].ToString();
            }

            JToken realNameValue;
            if (json.TryGetValue("realname", out realNameValue))
            {
                user.RealName = json["realname"]["_content"].ToString();
            }

            // Location
            JToken locationValue;
            if (json.TryGetValue("location", out locationValue))
            {
                user.Location = json["location"]["_content"].ToString();
            }

            // Description
            JToken descValue;
            if (json.TryGetValue("description", out descValue))
            {
                user.Description = json["description"]["_content"].ToString();
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

            // Photos section
            JObject photoJson = (JObject)json["photos"];

            JToken photoCountValue;
            if (photoJson.TryGetValue("count", out photoCountValue))
            {
                user.PhotoCount = int.Parse(photoJson["count"]["_content"].ToString());
            }

            JToken firstDateValue;
            if (photoJson.TryGetValue("firstdate", out firstDateValue))
            {
                string dateString = photoJson["firstdate"]["_content"].ToString();
                if (dateString.Length > 0)
                {
                    user.FirstDate = dateString.ToDateTime();
                    user.hasFirstDate = true;
                }
                else
                    user.hasFirstDate = false;
            }

            return user;
        }
    }
}
