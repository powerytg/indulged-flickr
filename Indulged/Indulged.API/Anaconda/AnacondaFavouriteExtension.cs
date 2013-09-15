using Indulged.API.Anaconda.Events;
using Indulged.API.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            paramDict["user_id"] = UrlHelper.Encode(Cinderella.Cinderella.CinderellaCore.CurrentUser.ResourceId);
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
                    FavouriteStreamException(this, null);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetFavouritePhotoStreamAsync(userId, parameters); }))
                {
                    FavouriteStreamException(this, null);
                    return;
                }

                GetFavouriteStreamEventArgs args = new GetFavouriteStreamEventArgs();
                args.UserId = userId;
                args.Response = jsonString;
                FavouriteStreamReturned.DispatchEvent(this, args);
            }
        }

        public void AddPhotoToFavouriteAsync(string photoId)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.favorites.add";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
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
                        AddFavouriteExceptionEventArgs exceptionArgs = new AddFavouriteExceptionEventArgs();
                        exceptionArgs.PhotoId = photoId;
                        exceptionArgs.Message = errorMessage;
                        AddPhotoAsFavouriteException.DispatchEvent(this, exceptionArgs);
                    }
                    else
                    {
                        AddFavouriteEventArgs args = new AddFavouriteEventArgs();
                        args.PhotoId = photoId;
                        AddedPhotoAsFavourite.DispatchEvent(this, args);
                    }


                }, (ex) =>
                {
                    AddFavouriteExceptionEventArgs exceptionArgs = new AddFavouriteExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    exceptionArgs.Message = "Unknown network error";
                    AddPhotoAsFavouriteException.DispatchEvent(this, exceptionArgs);
                });
        }

        public void RemovePhotoFromFavouriteAsync(string photoId)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.favorites.remove";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
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
                        RemoveFavouriteExceptionEventArgs exceptionArgs = new RemoveFavouriteExceptionEventArgs();
                        exceptionArgs.PhotoId = photoId;
                        exceptionArgs.Message = errorMessage;
                        RemovePhotoFromFavouriteException.DispatchEvent(this, exceptionArgs);
                    }
                    else
                    {
                        RemoveFavouriteEventArgs args = new RemoveFavouriteEventArgs();
                        args.PhotoId = photoId;
                        RemovePhotoFromFavourite.DispatchEvent(this, args);
                    }


                }, (ex) =>
                {
                    RemoveFavouriteExceptionEventArgs exceptionArgs = new RemoveFavouriteExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    exceptionArgs.Message = "Unknown network error";
                    RemovePhotoFromFavouriteException.DispatchEvent(this, exceptionArgs);
                });
        }
    }
}
