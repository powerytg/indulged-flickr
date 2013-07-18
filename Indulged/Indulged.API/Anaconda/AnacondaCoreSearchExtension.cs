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
        public async void SearchPhotoAsync(string searchSessionId, string query = null, string tags = null, Dictionary<string, string> parameters = null)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photos.search";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["extras"] = UrlHelper.Encode(commonExtraParameters);

            if (query != null)
                paramDict["text"] = UrlHelper.Encode(query);

            if (tags != null)
                paramDict["tags"] = UrlHelper.Encode(tags);

            if (parameters != null)
            {
                foreach (var entry in parameters)
                {
                    paramDict[entry.Key] = entry.Value;
                }
            }

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "http://api.flickr.com/services/rest", paramString);
            string requestUrl = "http://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = reader.ReadToEnd();
                if (!TryHandleResponseException(jsonString, () => { SearchPhotoAsync(searchSessionId, query, tags, parameters); }))
                    return;

                PhotoSearchEventArgs evt = new PhotoSearchEventArgs();
                evt.SearchSessionId = searchSessionId;
                evt.Response = jsonString;
                PhotoSearchReturned.DispatchEvent(this, evt);

            }
        }

        public async void SearchGroupsAsync(string searchSessionId, string query = null, Dictionary<string, string> parameters = null)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.groups.search";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["text"] = UrlHelper.Encode(query);

            if (parameters != null)
            {
                foreach (var entry in parameters)
                {
                    paramDict[entry.Key] = entry.Value;
                }
            }

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "http://api.flickr.com/services/rest", paramString);
            string requestUrl = "http://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = reader.ReadToEnd();
                if (!TryHandleResponseException(jsonString, () => { SearchGroupsAsync(searchSessionId, query, parameters); }))
                    return;

                GroupSearchEventArgs evt = new GroupSearchEventArgs();
                evt.SearchSessionId = searchSessionId;
                evt.Response = jsonString;
                GroupSearchReturned.DispatchEvent(this, evt);

            }
        }
    }
}
