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
        // Status
        public bool IsLoadingDiscoveryStream { get; set; }

        public async void GetDiscoveryStreamAsync(Dictionary<string, string> parameters = null)
        {
            IsLoadingDiscoveryStream = true;

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.interestingness.getList";
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

            paramDict["extras"] = UrlHelper.Encode(commonExtraParameters);
            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "http://api.flickr.com/services/rest", paramString);
            string requestUrl = "http://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                IsLoadingDiscoveryStream = false;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = reader.ReadToEnd();
                if (!TryHandleResponseException(jsonString, () => { GetDiscoveryStreamAsync(parameters); }))
                    return;


                GetDiscoveryStreamEventArgs args = new GetDiscoveryStreamEventArgs();
                args.Response = jsonString;
                DiscoveryStreamReturned.DispatchEvent(this, args);
            }
        }
    }
}
