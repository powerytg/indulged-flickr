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
            paramDict["extras"] = UrlHelper.Encode(commonExtraParameters);

            string paramString = GenerateParamString(paramDict);
            string signature = GenerateSignature("GET", AccessTokenSecret, "http://api.flickr.com/services/rest", paramString);
            string requestUrl = "http://api.flickr.com/services/rest?" + paramString + "&oauth_signature=" + signature;
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if(exifQueue.Contains(photoId))
                    exifQueue.Remove(photoId);

                GetEXIFExceptionEventArgs exceptionArgs = null;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    exceptionArgs = new GetEXIFExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    EXIFException.DispatchEvent(this, exceptionArgs);

                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!IsResponseSuccess(jsonString))
                {
                    exceptionArgs = new GetEXIFExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    EXIFException.DispatchEvent(this, exceptionArgs);

                    return;
                }


                GetEXIFEventArgs args = new GetEXIFEventArgs();
                args.PhotoId = photoId;
                args.Response = jsonString;
                EXIFReturned.DispatchEvent(this, args);
            }
        }

        private List<string> infoQueue = new List<string>();
        public async void GetPhotoInfoAsync(string photoId, string OwnerId, bool isUploadedPhoto)
        {
            if (infoQueue.Contains(photoId))
                return;

            infoQueue.Add(photoId);

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photos.getInfo";
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
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (infoQueue.Contains(photoId))
                    infoQueue.Remove(photoId);

                GetPhotoInfoExceptionEventArgs exceptionArgs = null;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    exceptionArgs = new GetPhotoInfoExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    PhotoInfoException.DispatchEvent(this, exceptionArgs);

                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!IsResponseSuccess(jsonString))
                {
                    exceptionArgs = new GetPhotoInfoExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    PhotoInfoException.DispatchEvent(this, exceptionArgs);

                    return;
                }


                GetPhotoInfoEventArgs args = new GetPhotoInfoEventArgs();
                args.PhotoId = photoId;
                args.Response = jsonString;
                args.OwnerId = OwnerId;
                args.IsUploadedPhoto = isUploadedPhoto;
                PhotoInfoReturned.DispatchEvent(this, args);
            }
        }

        private List<string> commentQueue = new List<string>();
        public async void GetPhotoCommentsAsync(string photoId, Dictionary<string, string> parameters = null)
        {
            if (commentQueue.Contains(photoId))
                return;

            commentQueue.Add(photoId);

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photos.comments.getList";
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
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if(commentQueue.Contains(photoId))
                    commentQueue.Remove(photoId);

                GetPhotoCommentsExceptionEventArgs exceptionArgs = null;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    exceptionArgs = new GetPhotoCommentsExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    GetPhotoCommentsException.DispatchEvent(this, exceptionArgs);

                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!IsResponseSuccess(jsonString))
                {
                    exceptionArgs = new GetPhotoCommentsExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    GetPhotoCommentsException.DispatchEvent(this, exceptionArgs);

                    return;
                }


                GetPhotoCommentsEventArgs args = new GetPhotoCommentsEventArgs();
                args.PhotoId = photoId;
                args.Response = jsonString;
                PhotoCommentsReturned.DispatchEvent(this, args);
            }
        }

        public void AddCommentAsync(string sessionId, string photoId, string message)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.photos.comments.addComment";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["photo_id"] = photoId;
            paramDict["comment_text"] = message;

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
                        AddCommentExceptionEventArgs exceptionArgs = new AddCommentExceptionEventArgs();
                        exceptionArgs.SessionId = sessionId;
                        AddPhotoCommentException.DispatchEvent(this, exceptionArgs);
                    }
                    else
                    {
                        AddCommentEventArgs args = new AddCommentEventArgs();
                        args.SessionId = sessionId;
                        args.PhotoId = photoId;
                        args.Response = response;
                        args.Message = message;
                        PhotoCommentAdded.DispatchEvent(this, args);
                    }
                }, (ex) =>
                {
                    AddCommentExceptionEventArgs exceptionArgs = new AddCommentExceptionEventArgs();
                    exceptionArgs.SessionId = sessionId;
                    AddPhotoCommentException.DispatchEvent(this, exceptionArgs);
                });


        }
    }
}
