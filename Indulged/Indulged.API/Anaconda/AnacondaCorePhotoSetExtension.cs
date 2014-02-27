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

        public void ChangePhotoSetPrimaryPhotoAsync(string setId, string photoId)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photosets.setPrimaryPhoto";
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
                        ChangePhotoSetPrimaryExceptionEventArgs exceptionArgs = new ChangePhotoSetPrimaryExceptionEventArgs();
                        exceptionArgs.PhotoId = photoId;
                        exceptionArgs.SetId = setId;
                        exceptionArgs.ErrorMessage = errorMessage;
                        PhotoSetChangePrimaryException.DispatchEvent(this, exceptionArgs);
                    }
                    else
                    {
                        ChangePhotoSetPrimaryEventArgs args = new ChangePhotoSetPrimaryEventArgs();
                        args.PhotoId = photoId;
                        args.SetId = setId;
                        PhotoSetChangedPrimary.DispatchEvent(this, args);
                    }


                }, (ex) =>
                {
                    ChangePhotoSetPrimaryExceptionEventArgs exceptionArgs = new ChangePhotoSetPrimaryExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    exceptionArgs.SetId = setId;
                    exceptionArgs.ErrorMessage = "Unknown network error";
                    PhotoSetChangePrimaryException.DispatchEvent(this, exceptionArgs);
                });
        }

        public void EditPhotoSetAsync(string setId, string title, string description = null)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photosets.editMeta";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["photoset_id"] = setId;
            paramDict["title"] = title;
            if (description != null)
            {
                paramDict["description"] = description;
            }

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
                        EditPhotoSetExceptionEventArgs exceptionArgs = new EditPhotoSetExceptionEventArgs();
                        exceptionArgs.SetId = setId;
                        exceptionArgs.ErrorMessage = errorMessage;
                        PhotoSetEditException.DispatchEvent(this, exceptionArgs);
                    }
                    else
                    {
                        EditPhotoSetEventArgs args = new EditPhotoSetEventArgs();
                        args.SetId = setId;
                        args.Title = title;
                        args.Description = description;
                        PhotoSetEdited.DispatchEvent(this, args);
                    }


                }, (ex) =>
                {
                    EditPhotoSetExceptionEventArgs exceptionArgs = new EditPhotoSetExceptionEventArgs();
                    exceptionArgs.SetId = setId;
                    exceptionArgs.ErrorMessage = "Unknown network error";
                    PhotoSetEditException.DispatchEvent(this, exceptionArgs);
                });
        }

        public void DeletePhotoSetAsync(string setId)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photosets.delete";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["photoset_id"] = setId;

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
                        DeletePhotoSetExceptionEventArgs exceptionArgs = new DeletePhotoSetExceptionEventArgs();
                        exceptionArgs.SetId = setId;
                        exceptionArgs.ErrorMessage = errorMessage;
                        PhotoSetDeleteException.DispatchEvent(this, exceptionArgs);
                    }
                    else
                    {
                        DeletePhotoSetEventArgs args = new DeletePhotoSetEventArgs();
                        args.SetId = setId;
                        PhotoSetDeleted.DispatchEvent(this, args);
                    }


                }, (ex) =>
                {
                    DeletePhotoSetExceptionEventArgs exceptionArgs = new DeletePhotoSetExceptionEventArgs();
                    exceptionArgs.SetId = setId;
                    exceptionArgs.ErrorMessage = "Unknown network error";
                    PhotoSetDeleteException.DispatchEvent(this, exceptionArgs);
                });
        }
    }
}
