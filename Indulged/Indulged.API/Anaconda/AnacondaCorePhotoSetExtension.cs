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
    }
}
