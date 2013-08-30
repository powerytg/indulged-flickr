using Indulged.API.Anaconda.Events;
using Indulged.API.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        public bool isLoadingFavStream = false;
        public async void GetFavouritePhotoStreamAsync(string userId, Dictionary<string, string> parameters = null)
        {
            isLoadingFavStream = true;
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.favorites.getList";
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

            paramDict["user_id"] = Cinderella.Cinderella.CinderellaCore.CurrentUser.ResourceId;
            paramDict["extras"] = UrlHelper.Encode(commonExtraParameters);

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "http://api.flickr.com/services/rest", paramString);
            string requestUrl = "http://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                isLoadingFavStream = false;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetFavouritePhotoStreamAsync(userId, parameters); }))
                    return;

                GetFavouriteStreamEventArgs args = new GetFavouriteStreamEventArgs();
                args.UserId = userId;
                args.Response = jsonString;
                FavouriteStreamReturned.DispatchEvent(this, args);
            }
        }
    }
}
