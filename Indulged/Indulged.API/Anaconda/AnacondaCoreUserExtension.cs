using Indulged.API.Anaconda.Events;
using Indulged.API.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        public async void GetPhotoStreamAsync(string userId, Dictionary<string, string> parameters = null)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.people.getPhotos";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";

            if (parameters != null)
            {
                foreach (var entry in parameters)
                {
                    paramDict[entry.Key] = entry.Value;
                }
            }

            if (userId == Cinderella.Cinderella.CinderellaCore.CurrentUser.ResourceId)
                paramDict["user_id"] = "me";
            else
                paramDict["user_id"] = Cinderella.Cinderella.CinderellaCore.CurrentUser.ResourceId;

            paramDict["extras"] = UrlHelper.Encode("description,views,tags");

            User user = null;
            if (Cinderella.Cinderella.CinderellaCore.UserCache.ContainsKey(userId))
            {
                user = Cinderella.Cinderella.CinderellaCore.UserCache[userId];
                user.IsLoadingPhotoStream = true;
            }

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "http://api.flickr.com/services/rest", paramString);
            string requestUrl = "http://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (user != null)
                {
                    user.IsLoadingPhotoStream = false;
                }

                string jsonString = reader.ReadToEnd();

                GetPhotoStreamEventArgs args = new GetPhotoStreamEventArgs();
                args.UserId = userId;
                args.Response = jsonString;
                PhotoStreamReturned.DispatchEvent(this, args);
            }
        }
    }
}
