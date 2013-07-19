using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Utils;
using Indulged.API.Cinderella;
using Indulged.API.Cinderella.Models;
using System.Net;
using System.IO;

using Indulged.API.Anaconda.Events;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        public async void GetPhotoSetListAsync(string userId)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            string paramString = "oauth_nonce=" + nonce;
            paramString += "&oauth_consumer_key=" + consumerKey;
            paramString += "&oauth_signature_method=HMAC-SHA1";
            paramString += "&oauth_timestamp=" + timestamp;
            paramString += "&oauth_version=1.0";
            paramString += "&oauth_token=" + AccessToken;
            paramString += "&format=json&nojsoncallback=1";
            paramString += "&user_id=" + userId;
            paramString += "&method=flickr.photosets.getList";

            string signature = GenerateSignature("GET", AccessTokenSecret, "http://api.flickr.com/services/rest/", paramString);
            string requestUrl = "http://api.flickr.com/services/rest/?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = reader.ReadToEnd();
                if (!TryHandleResponseException(jsonString, () => { GetPhotoSetListAsync(userId); }))
                    return;

                PhotoSetListEventArgs evt = new PhotoSetListEventArgs();
                evt.UserId = userId;
                evt.Response = jsonString;
                PhotoSetListReturned.DispatchEvent(this, evt);
            }

        }

        private List<string> setPhotoFetchingQueue = new List<string>();

        public async void GetPhotoSetPhotosAsync(string setId, Dictionary<string, string> parameters = null)
        {
            if (setPhotoFetchingQueue.Contains(setId))
                return;

            setPhotoFetchingQueue.Add(setId);

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photosets.getPhotos";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["photoset_id"] = setId;

            if (parameters != null)
            {
                foreach (var entry in parameters)
                {
                    paramDict[entry.Key] = entry.Value;
                }
            }

            paramDict["extras"] = UrlHelper.Encode(commonExtraParameters);

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "http://api.flickr.com/services/rest", paramString);
            string requestUrl = "http://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                setPhotoFetchingQueue.Remove(setId);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = reader.ReadToEnd();
                if (!TryHandleResponseException(jsonString, () => { GetPhotoSetPhotosAsync(setId, parameters); }))
                    return;

                GetPhotoSetPhotosEventArgs args = new GetPhotoSetPhotosEventArgs();
                args.PhotoSetId = setId;
                args.Response = jsonString;
                PhotoSetPhotosReturned.DispatchEvent(this, args);
            }
        }
    }
}
