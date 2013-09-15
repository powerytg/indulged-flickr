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
using Newtonsoft.Json.Linq;
using System.Diagnostics;

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
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                GetPhotoSetListExceptionEventArgs exceptionEvt = null;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);

                    exceptionEvt = new GetPhotoSetListExceptionEventArgs();
                    exceptionEvt.UserId = userId;
                    GetPhotoSetListException.DispatchEvent(this, exceptionEvt);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetPhotoSetListAsync(userId); }))
                {
                    exceptionEvt = new GetPhotoSetListExceptionEventArgs();
                    exceptionEvt.UserId = userId;
                    GetPhotoSetListException.DispatchEvent(this, exceptionEvt);

                    return;
                }

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
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if(setPhotoFetchingQueue.Contains(setId))
                    setPhotoFetchingQueue.Remove(setId);

                GetPhotoSetPhotosExceptionEventArgs exceptionEvt = null;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);

                    exceptionEvt = new GetPhotoSetPhotosExceptionEventArgs();
                    exceptionEvt.PhotoSetId = setId;
                    PhotoSetPhotosException(this, exceptionEvt);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetPhotoSetPhotosAsync(setId, parameters); }))
                {
                    exceptionEvt = new GetPhotoSetPhotosExceptionEventArgs();
                    exceptionEvt.PhotoSetId = setId;
                    PhotoSetPhotosException(this, exceptionEvt);
                    return;
                }

                GetPhotoSetPhotosEventArgs args = new GetPhotoSetPhotosEventArgs();
                args.PhotoSetId = setId;
                args.Response = jsonString;
                PhotoSetPhotosReturned.DispatchEvent(this, args);
            }
        }

        public void AddPhotoToSetAsync(string photoId, string setId)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photosets.addPhoto";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["photoset_id"] = setId;
            paramDict["photo_id"] = photoId;

            string signature = OAuthCalculateSignature("POST", "http://api.flickr.com/services/rest/", paramDict, AccessTokenSecret);
            paramDict["oauth_signature"] = signature;

            DispatchPostRequest("POST", "http://api.flickr.com/services/rest/", paramDict,
                (response) =>
                {
                    bool success = true;
                    string errorMessage = "";

                    try
                    {
                        JObject json = JObject.Parse(response);
                        string status = json["stat"].ToString();
                        if (status != "ok")
                        {
                            success = false;
                            errorMessage = json["message"].ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);

                        success = false;
                    }

                    if (!success)
                    {
                        AddPhotoToSetExceptionEventArgs exceptionArgs = new AddPhotoToSetExceptionEventArgs();
                        exceptionArgs.PhotoId = photoId;
                        exceptionArgs.SetId = setId;
                        exceptionArgs.ErrorMessage = errorMessage;
                        AddPhotoToSetException.DispatchEvent(this, exceptionArgs);
                    }
                    else
                    {
                        AddPhotoToSetEventArgs args = new AddPhotoToSetEventArgs();
                        args.PhotoId = photoId;
                        args.SetId = setId;
                        PhotoAddedToSet.DispatchEvent(this, args);
                    }


                }, (ex) =>
                {
                    AddPhotoToSetExceptionEventArgs exceptionArgs = new AddPhotoToSetExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    exceptionArgs.SetId = setId;
                    exceptionArgs.ErrorMessage = "Unknown network error";
                    AddPhotoToSetException.DispatchEvent(this, exceptionArgs);
                });
        }

        public void RemovePhotoFromSetAsync(string photoId, string setId)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photosets.removePhoto";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["photoset_id"] = setId;
            paramDict["photo_id"] = photoId;

            string signature = OAuthCalculateSignature("POST", "http://api.flickr.com/services/rest/", paramDict, AccessTokenSecret);
            paramDict["oauth_signature"] = signature;

            DispatchPostRequest("POST", "http://api.flickr.com/services/rest/", paramDict,
                (response) =>
                {
                    bool success = true;
                    string errorMessage = "";

                    try
                    {
                        JObject json = JObject.Parse(response);
                        string status = json["stat"].ToString();
                        if (status != "ok")
                        {
                            success = false;
                            errorMessage = json["message"].ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);

                        success = false;
                    }

                    if (!success)
                    {
                        RemovePhotoFromSetExceptionEventArgs exceptionArgs = new RemovePhotoFromSetExceptionEventArgs();
                        exceptionArgs.PhotoId = photoId;
                        exceptionArgs.SetId = setId;
                        exceptionArgs.ErrorMessage = errorMessage;
                        RemovePhotoFromSetException.DispatchEvent(this, exceptionArgs);
                    }
                    else
                    {
                        RemovePhotoFromSetEventArgs args = new RemovePhotoFromSetEventArgs();
                        args.PhotoId = photoId;
                        args.SetId = setId;
                        PhotoRemovedFromSet.DispatchEvent(this, args);
                    }


                }, (ex) =>
                {
                    RemovePhotoFromSetExceptionEventArgs exceptionArgs = new RemovePhotoFromSetExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    exceptionArgs.SetId = setId;
                    exceptionArgs.ErrorMessage = "Unknown network error";
                    RemovePhotoFromSetException.DispatchEvent(this, exceptionArgs);
                });
        }
    }
}
