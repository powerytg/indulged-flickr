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
        public async void GetGroupInfoAsync(string groupId, Dictionary<string, string> parameters = null)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.groups.getInfo";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["group_id"] = groupId;

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
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetGroupInfoAsync(groupId, parameters); }))
                    return;

                GetGroupInfoEventArgs evt = new GetGroupInfoEventArgs();
                evt.GroupId = groupId;
                evt.Response = jsonString;
                GroupInfoReturned.DispatchEvent(this, evt);

            }
        }

        private List<string> groupPhotoFetchingQueue = new List<string>();

        public async void GetGroupPhotosAsync(string groupId, Dictionary<string, string> parameters = null)
        {
            if (groupPhotoFetchingQueue.Contains(groupId))
                return;

            groupPhotoFetchingQueue.Add(groupId);

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.groups.pools.getPhotos";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["group_id"] = groupId;
            
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
                groupPhotoFetchingQueue.Remove(groupId);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetGroupPhotosAsync(groupId, parameters); }))
                    return;

                GetGroupPhotosEventArgs args = new GetGroupPhotosEventArgs();
                args.GroupId = groupId;
                args.Response = jsonString;
                GroupPhotoReturned.DispatchEvent(this, args);
            }
        }

        private List<string> groupTopicsFetchingQueue = new List<string>();
        public async void GetGroupTopicsAsync(string groupId, Dictionary<string, string> parameters = null)
        {
            if (groupTopicsFetchingQueue.Contains(groupId))
                return;

            groupTopicsFetchingQueue.Add(groupId);

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.groups.discuss.topics.getList";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["group_id"] = groupId;

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
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                groupTopicsFetchingQueue.Remove(groupId);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetGroupTopicsAsync(groupId, parameters); }))
                    return;

                GetGroupTopicsEventArgs args = new GetGroupTopicsEventArgs();
                args.GroupId = groupId;
                args.Response = jsonString;
                GroupTopicsReturned.DispatchEvent(this, args);
            }
        }

        public void AddTopicAsync(string sessionId, string groupId, string subject, string message)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.groups.discuss.topics.add";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["group_id"] = groupId;
            paramDict["subject"] = subject;
            paramDict["message"] = message;

            string signature = OAuthCalculateSignature("POST", "http://api.flickr.com/services/rest/", paramDict, AccessTokenSecret);
            paramDict["oauth_signature"] = signature;

            DispatchPostRequest("POST", "http://api.flickr.com/services/rest/", paramDict, 
                (response) => {
                    AddTopicEventArgs args = new AddTopicEventArgs();
                    args.SessionId = sessionId;
                    args.GroupId = groupId;
                    args.Response = response;
                    args.Subject = subject;
                    args.Message = message;
                    TopicAdded.DispatchEvent(this, args);

                }, (ex) => {
                    AddTopicExceptionEventArgs exceptionArgs = new AddTopicExceptionEventArgs();
                    exceptionArgs.SessionId = sessionId;
                    AddTopicException.DispatchEvent(this, exceptionArgs);
                });

            
        }

        public void AddPhotoToGroupAsync(string photoId, string groupId)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.groups.pools.add";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["group_id"] = groupId;
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
                        AddPhotoToGroupExceptionEventArgs exceptionArgs = new AddPhotoToGroupExceptionEventArgs();
                        exceptionArgs.PhotoId = photoId;
                        exceptionArgs.GroupId = groupId;
                        exceptionArgs.ErrorMessage = errorMessage;
                        AddPhotoToGroupException.DispatchEvent(this, exceptionArgs);
                    }
                    else
                    {
                        AddPhotoToGroupEventArgs args = new AddPhotoToGroupEventArgs();
                        args.PhotoId = photoId;
                        args.GroupId = groupId;
                        PhotoAddedToGroup.DispatchEvent(this, args);
                    }


                }, (ex) =>
                {
                    AddPhotoToGroupExceptionEventArgs exceptionArgs = new AddPhotoToGroupExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    exceptionArgs.GroupId = groupId;
                    exceptionArgs.ErrorMessage = "Unknown network error";
                    AddPhotoToGroupException.DispatchEvent(this, exceptionArgs);
                });
        }

        public void RemovePhotoFromGroupAsync(string photoId, string groupId)
        {
            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.groups.pools.remove";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["group_id"] = groupId;
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
                        RemovePhotoFromGroupExceptionEventArgs exceptionArgs = new RemovePhotoFromGroupExceptionEventArgs();
                        exceptionArgs.PhotoId = photoId;
                        exceptionArgs.GroupId = groupId;
                        exceptionArgs.ErrorMessage = errorMessage;
                        RemovePhotoFromGroupException.DispatchEvent(this, exceptionArgs);
                    }
                    else
                    {
                        RemovePhotoFromGroupEventArgs args = new RemovePhotoFromGroupEventArgs();
                        args.PhotoId = photoId;
                        args.GroupId = groupId;
                        PhotoRemovedFromGroup.DispatchEvent(this, args);
                    }


                }, (ex) =>
                {
                    RemovePhotoFromGroupExceptionEventArgs exceptionArgs = new RemovePhotoFromGroupExceptionEventArgs();
                    exceptionArgs.PhotoId = photoId;
                    exceptionArgs.GroupId = groupId;
                    exceptionArgs.ErrorMessage = "Unknown network error";
                    RemovePhotoFromGroupException.DispatchEvent(this, exceptionArgs);
                });
        }

        private List<string> topicRepliesFetchingQueue = new List<string>();
        public async void GetTopicRepliesAsync(string topicId, string groupId, Dictionary<string, string> parameters = null)
        {
            if (topicRepliesFetchingQueue.Contains(topicId))
                return;

            topicRepliesFetchingQueue.Add(topicId);

            string timestamp = DateTimeUtils.GetTimestamp();
            string nonce = Guid.NewGuid().ToString().Replace("-", null);

            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict["method"] = "flickr.groups.discuss.replies.getList";
            paramDict["format"] = "json";
            paramDict["nojsoncallback"] = "1";
            paramDict["oauth_consumer_key"] = consumerKey;
            paramDict["oauth_nonce"] = nonce;
            paramDict["oauth_signature_method"] = "HMAC-SHA1";
            paramDict["oauth_timestamp"] = timestamp;
            paramDict["oauth_token"] = AccessToken;
            paramDict["oauth_version"] = "1.0";
            paramDict["topic_id"] = topicId;

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
            HttpWebResponse response = await DispatchRequest("GET", requestUrl, null).ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                topicRepliesFetchingQueue.Remove(topicId);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HandleHTTPException(response);
                    return;
                }

                string jsonString = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (!TryHandleResponseException(jsonString, () => { GetTopicRepliesAsync(topicId, groupId, parameters); }))
                    return;

                GetTopicRepliesEventArgs args = new GetTopicRepliesEventArgs();
                args.TopicId = topicId;
                args.Response = jsonString;
                args.GroupId = groupId;
                TopicRepliesReturned.DispatchEvent(this, args);
            }
        }
    }
}
