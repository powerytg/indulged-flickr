using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Indulged.API.Cinderella.Models;
using Indulged.API.Utils;

namespace Indulged.API.Cinderella.Factories
{
    public class PhotoSetFactory
    {
        public static PhotoSet PhotoSetWithJObject(JObject json)
        {
            string setId = json["id"].ToString();
            PhotoSet photoset;
            if (Cinderella.CinderellaCore.PhotoSetCache.ContainsKey(setId))
            {
                photoset = Cinderella.CinderellaCore.PhotoSetCache[setId];
            }
            else
            {
                photoset = new PhotoSet();
                photoset.ResourceId = setId;
                Cinderella.CinderellaCore.PhotoSetCache[setId] = photoset;
            }

            photoset.Primary = json["primary"].ToString();
            photoset.Secret = json["secret"].ToString();
            photoset.Server = json["server"].ToString();
            photoset.Farm = json["farm"].ToString();
            photoset.PhotoCount = int.Parse(json["photos"].ToString());
            photoset.VideoCount = int.Parse(json["videos"].ToString());
            photoset.Title = json["title"]["_content"].ToString();
            photoset.Description = json["description"]["_content"].ToString();
            photoset.IsVisible = json["visibility_can_see_set"].ToString().ParseBool();
            photoset.ViewCount = int.Parse(json["count_views"].ToString());
            photoset.CommentCount = int.Parse(json["count_comments"].ToString());
            photoset.CanComment = json["can_comment"].ToString().ParseBool();
            photoset.CreationDate = json["date_create"].ToString().ToDateTime();
            photoset.UpdatedDate = json["date_update"].ToString().ToDateTime();

            return photoset;

        }
    }
}
