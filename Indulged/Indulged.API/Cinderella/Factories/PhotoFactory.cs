using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;

namespace Indulged.API.Cinderella.Factories
{
    public class PhotoFactory
    {
        public static Photo PhotoWithJObject(JObject json)
        {
            string photoId = json["id"].ToString();
            Photo photo = null;
            if (Cinderella.CinderellaCore.PhotoCache.ContainsKey(photoId))
            {
                photo = Cinderella.CinderellaCore.PhotoCache[photoId];
            }
            else
            {
                photo = new Photo();
                photo.ResourceId = photoId;
                Cinderella.CinderellaCore.PhotoCache[photoId] = photo;
            }

            JToken ownerValue;
            if (json.TryGetValue("owner", out ownerValue))
            {
                photo.UserId = json["owner"].ToString();

                // Parse user
                User user = UserFactory.UserWithJObject(json);
            }
            else
            {
            }
            
            photo.Secret = json["secret"].ToString();
            photo.Server = json["server"].ToString();
            photo.Farm = json["farm"].ToString();
            photo.ViewCount = int.Parse(json["views"].ToString());

            JToken commentCountValue;
            if(json.TryGetValue("comments", out commentCountValue))
            {
                photo.CommentCount = int.Parse(json["comments"]["_content"].ToString());
            }

            if(json["title"].GetType() == typeof(JValue))
                photo.Title = json["title"].ToString();
            else
                photo.Title = json["title"]["_content"].ToString();                
            
            if (photo.Title.Length > CinderellaConstants.MaxTitleLength)
                photo.Title = photo.Title.Substring(0, CinderellaConstants.MaxTitleLength) + "...";

            photo.Description = json["description"]["_content"].ToString();
            if (photo.Description.Length > CinderellaConstants.MaxDescriptionLength)
                photo.Description = photo.Description.Substring(0, CinderellaConstants.MaxDescriptionLength) + "...";

            JToken licenseValue;
            if (json.TryGetValue("license", out licenseValue))
            {
                photo.LicenseId = json["license"].ToString();
            }

            // Tags
            photo.Tags = new List<string>();
            JToken tagsValue;
            if (json.TryGetValue("tags", out tagsValue))
            {
                string tagsString = json["tags"].ToString();
                if (tagsString.Length != 0)
                {
                    photo.Tags = new List<string>(tagsString.Split(' '));
                }
                
            }

            // Favourite
            JToken favValue;
            if(json.TryGetValue("isfavorite", out favValue))
            {
                photo.IsFavourite = (json["isfavorite"].ToString() == "1");
            }

            return photo;

        }
    }
}
