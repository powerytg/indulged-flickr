using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Indulged.API.Utils;
using Indulged.API.Anaconda.Events;
using System.Net;
using System.IO;

namespace Indulged.API.Anaconda
{
    public partial class AnacondaCore
    {
        // Status
        private bool isLoadingActivityStream;
        public async void GetActivityStreamAsync()
        {
            if (isLoadingActivityStream)
                return;

            isLoadingActivityStream = true;

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.activity.userPhotos";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["timeframe"] = "30d";
            paramDict["page"] = "1";
            paramDict["per_page"] = "25";

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "https://api.flickr.com/services/rest", paramString);
            string requestUrl = "https://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                isLoadingActivityStream = false;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetActivityStreamAsync(); }))
                    return;


                GetActivityStreamEventArgs args = new GetActivityStreamEventArgs();
                args.Response = jsonString;
                ActivityStreamReturned.DispatchEvent(this, args);
            }
        }
    }
}
