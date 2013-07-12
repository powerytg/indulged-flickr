﻿using Indulged.API.Anaconda.Events;
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
        // Status keeper
        private List<string> exifQueue = new List<string>();

        public bool IsGettingEXIFInfo(string photoId)
        {
            return exifQueue.Contains(photoId);
        }

        public async void GetEXIFAsync(string photoId)
        {
            if (IsGettingEXIFInfo(photoId))
                return;

            exifQueue.Add(photoId);

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photos.getExif";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["photo_id"] = photoId;

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "http://api.flickr.com/services/rest", paramString);
            string requestUrl = "http://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                exifQueue.Remove(photoId);

                string jsonString = reader.ReadToEnd();

                GetEXIFEventArgs args = new GetEXIFEventArgs();
                args.PhotoId = photoId;
                args.Response = jsonString;
                EXIFReturned.DispatchEvent(this, args);
            }
        }
    }
}
